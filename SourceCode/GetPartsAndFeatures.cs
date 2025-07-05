using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.UF;
using NXOpen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXOpenPracticeCSharp
{
    public class GetPartsAndFeatures
    {
        /// <summary>
        /// Gets all components in the all levels of assembly.
        /// </summary>
        /// <returns>List of components</returns>
        public static List<Component> GetAllComponents()
        {
            //Get all parts open in the session
            //If it is an assembly, it gets all the components along with assembly, if ypu have 2 parts in an assembly it will give 3
            //PartCollection partList=theSession.Parts;
            //lw.WriteLine(Convert.ToString(partList.ToArray().Length));

            //Getting all the sub assemblies and components in the assembly
            List<NXOpen.Assemblies.Component> assemblies;
            List<NXOpen.Assemblies.Component> components;
            assemblies = new List<Component>();
            components = new List<Component>();

            try
            {
                //If the display part is not an assembly than root component will be null
                if (Session.GetSession().Parts.Display.ComponentAssembly.RootComponent is null)
                {
                    NXLogger.Instance.Log("Part is not an assembly",LogLevel.Warning);
                }
                else
                {
                    NXOpen.Assemblies.Component rootComponent = Session.GetSession().Parts.Display.ComponentAssembly.RootComponent;

                    //Going layer by level by level and getting all the child components
                    //storing current level components into "currentLevelComponents" list and traversing them to get their child components
                    //storing the child components in allChildComponents and making those child components into current level components and continuing the process
                    List<Component> currentLevelComponents = rootComponent.GetChildren().ToList();
                    List<Component> allChildComponents = rootComponent.GetChildren().ToList();
                    while (true)
                    {
                        //Getting child components of current level and adding it to allChildComponents, if there are no child components loop terminates
                        List<Component> childComponentsOfCurrentLevelComponents = currentLevelComponents.SelectMany(x => x.GetChildren()).ToList();
                        if (childComponentsOfCurrentLevelComponents.Count == 0) break;
                        allChildComponents.AddRange(childComponentsOfCurrentLevelComponents);
                        currentLevelComponents = childComponentsOfCurrentLevelComponents;
                    }

                    //saparating assemblies and individual components from allChildComponents
                    assemblies = allChildComponents.Where(x => x.GetChildren().Length != 0).ToList();
                    components = allChildComponents.Where(x => x.GetChildren().Length == 0).ToList();


                }
            }
            catch (Exception ex)
            {
                UI.GetUI().NXMessageBox.Show("Error", NXMessageBox.DialogType.Error, Convert.ToString(ex.Message));
            }
            return components;
        }

        /// <summary>
        /// Gets all the bodies from part or assembly, if it is assembly add GetAllComponents() method  
        /// if assembly first it will collect all components, gets its prototype and than collects the bodies, if the body is in assembly level it wont collect it
        /// /// </summary>
        /// <returns>List of bodies</returns>
        public static List<Body> GetBodies()
        {
            Part displayPart=Session.GetSession().Parts.Display;
            List<Body> bodyList = new List<Body>();
            try
            {
                //If it is part
                if (displayPart != null && displayPart.ComponentAssembly.RootComponent is null)
                {
                    bodyList.AddRange(displayPart.Bodies.ToArray());
                }

                //if it is assembly
                else
                {
                    foreach (Component component in GetAllComponents())
                    {
                        Part part = component.Prototype as Part;
                        bodyList.AddRange(part.Bodies.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                UI.GetUI().NXMessageBox.Show("Error", NXMessageBox.DialogType.Error, Convert.ToString(ex.Message));
            }
            return bodyList;
        }

        /// <summary>
        /// Gets all the bodies using UF API,
        /// This will collect all the solid bodies from the display doesn't matter wether it is in assembly or part
        /// </summary>
        /// <param name="bodies">output bodies</param>
        public static List<Body> GetBodiesFromAssembly()
        {
            //This will collect all the solid bodies from the display doesn't matter wether it is in assembly or part

            UFSession theUFSession = UFSession.GetUFSession();
            Part displayPart = Session.GetSession().Parts.Display;

            List<Body> bodies = new List<Body>();

            Tag body = Tag.Null;

            theUFSession.Obj.CycleObjsInPart(displayPart.Tag, UFConstants.UF_solid_type, ref body);
            while (body != Tag.Null)
            {
                NXObject nXObject = (NXObject)NXObjectManager.Get(body);
                if (nXObject is Body)
                {
                    int bodyType;
                    theUFSession.Modl.AskBodyType(body, out bodyType);
                    if (bodyType is UFConstants.UF_MODL_SOLID_BODY)
                    {
                        Body bdy = (Body)NXObjectManager.Get(body);
                        if (bdy != null)
                        {
                            bodies.Add(bdy);
                        }
                    }
                }
                theUFSession.Obj.CycleObjsInPart(displayPart.Tag, UFConstants.UF_solid_type, ref body);
            }
            //NXLogger.Instance.Log($"no of bodies: {bodies.Count}");
            //NXLogger.Instance.Log($"no of faces: {faces.Count}");

            return bodies;
        }

        /// <summary>
        /// Gets the feature by name in the part or assembly.
        /// </summary>
        /// <param name="featureName">Feature name provided in properties </param>
        /// <returns></returns>
        public static Feature FindFeatureByName(string featureName)
        {
            ////Get all the features in the part
            Part displayPart = Session.GetSession().Parts.Display;
            Feature[] features = displayPart.Features.ToArray();
            ////Find the feature by name
            ////Feature.Name --> Gets the name of the feature as defined in the properties --> pt
            ////Feature.GetFeatureName() --> Gets the name of the feature as defined in the feature itself-->Point(1)
            Feature foundFeature = features.FirstOrDefault(f => f.Name.Equals(featureName, StringComparison.OrdinalIgnoreCase));
            foreach (Feature feature in features)
            {
                NXLogger.Instance.Log($"Feature.Name: {feature.Name}, Feature.GetFeatureName: {feature.GetFeatureName()}, Type: {feature.FeatureType}", LogLevel.Debug);
            }
            if (foundFeature != null)
            {
                NXLogger.Instance.Log($"Feature '{featureName}' found.", LogLevel.Info);
                return foundFeature;
            }
            else
            {
                NXLogger.Instance.Log($"Feature '{featureName}' not found.", LogLevel.Warning);
                return null;
            }
        }

        /// <summary>
        /// Gets all faces from list of bodies
        /// </summary>
        /// <param name="lsBodies">List of bodies whose faces are needed </param>
        /// <returns></returns>
        public static List<Face> GetFaces(List<Body> lsBodies)
        {
            List<Face> faces = new List<Face>();

            foreach (Body body in lsBodies) 
            {
                if (body != null)
                {
                    faces.AddRange(body.GetFaces());
                }
            }
            return faces;
        }
    }
}
