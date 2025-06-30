using NXOpen;
using NXOpen.CAE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NXOpen.CAE.Post;

namespace NXOpenPracticeCSharp
{
    public class PartFileOperations
    {
        /// <summary>
        /// Creates a new part file with the specified name and sets it as the active work part.
        /// </summary>
        /// <remarks>The new part file is created using millimeter units. If an error occurs during the
        /// creation process,  an error message is displayed in a message box.</remarks>
        /// <param name="partName">The name of the part file to create. Must not be null or empty.</param>
        public static int CreatePartFile(string partName)
        {
            int returnValue = 0;
            NXOpen.Session theSession = NXOpen.Session.GetSession();
            try
            {
                NXOpen.BasePart part = (NXOpen.Part)theSession.Parts.NewBaseDisplay(partName, NXOpen.BasePart.Units.Millimeters);
                theSession.Parts.SetWork(part);
                NXLogger.Instance.Log($"Part file created: {partName}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                returnValue = 1;
                NXLogger.Instance.LogException(ex);
                NXOpen.UI.GetUI().NXMessageBox.Show("Error", NXOpen.NXMessageBox.DialogType.Error, ex.Message);
            }

            ////**********Journal***************
            //NXOpen.FileNew fileNew1;
            //fileNew1 = theSession.Parts.FileNew();

            //fileNew1.TemplateFileName = "model-plain-1-mm-template.prt";

            //fileNew1.UseBlankTemplate = false;

            //fileNew1.ApplicationName = "ModelTemplate";

            //fileNew1.Units = NXOpen.Part.Units.Millimeters;

            //fileNew1.RelationType = "";

            //fileNew1.UsesMasterModel = "No";

            //fileNew1.TemplateType = NXOpen.FileNewTemplateType.Item;

            //fileNew1.TemplatePresentationName = "Model";

            //fileNew1.ItemType = "";

            //fileNew1.Specialization = "";

            //fileNew1.SetCanCreateAltrep(false);

            //fileNew1.NewFileName = partName;

            //fileNew1.MasterFileName = "";

            //fileNew1.MakeDisplayedPart = true;

            //fileNew1.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;

            //NXOpen.NXObject nXObject1;
            //nXObject1 = fileNew1.Commit();

            //fileNew1.Destroy();


            return returnValue;
        }

        /// <summary>
        /// Opens a part file in NX and returns the load status of the operation.
        /// </summary>
        /// <remarks>This method attempts to open the specified part file in NX. If Teamcenter integration
        /// is active, the method resolves the latest revision of the part before attempting to load it. If the part is
        /// already loaded, it will be set as the active display part.</remarks>
        /// <param name="partName">The name of the part to open. If Teamcenter integration is active, the part name should include the part
        /// identifier, and the method will resolve the latest revision automatically.</param>
        /// <returns>A <see cref="NXOpen.PartLoadStatus"/> object containing information about the result of the load operation.
        /// Returns <see langword="null"/> if the part could not be loaded.</returns>
        public static NXOpen.PartLoadStatus OpenPart(string partName)
        {
            NXOpen.UF.UFSession theUFSession = NXOpen.UF.UFSession.GetUFSession();
            string fileName = partName;
            NXOpen.PartLoadStatus partLoadStatus = null;

            theUFSession.UF.IsUgmanagerActive(out bool isUgmanagerActive);
            if (isUgmanagerActive)
            {
                string[] SlashSplit = partName.Split('/');
                NXOpen.Tag tag; 
                theUFSession.Ugmgr.AskPartTag(SlashSplit[0], out tag);
                theUFSession.Ugmgr.ListPartRevisions(tag, out int noOfRev, out NXOpen.Tag[] revArray);
                theUFSession.Ugmgr.AskPartRevisionId(revArray[revArray.Length - 1], out string revId);
                fileName = "@DB/" + SlashSplit[0] + "/" + revId;
                //fileName = "@DB/" + partName;

                NXLogger.Instance.Log($"Opening part from Teamcenter: {fileName}", LogLevel.Info);
            }

            try
            {
                NXOpen.BasePart part = NXOpen.Session.GetSession().Parts.FindObject(fileName);
                if (part != null)
                {
                    NXOpen.Session.GetSession().Parts.SetActiveDisplay(part, NXOpen.DisplayPartOption.AllowAdditional, NXOpen.PartDisplayPartWorkPartOption.SameAsDisplay, out partLoadStatus);
                    NXLogger.Instance.Log($"Part already loaded: {fileName}", LogLevel.Info);
                }
            }
            catch (Exception)
            {
                try
                {
                    NXOpen.BasePart part = NXOpen.Session.GetSession().Parts.OpenActiveDisplay(fileName, NXOpen.DisplayPartOption.AllowAdditional, out partLoadStatus);

                    NXOpen.UF.PartLoadState partLoadState;
                    theUFSession.UF.AskLoadStateForPartFile(fileName, out partLoadState);
                    if (partLoadState == NXOpen.UF.PartLoadState.PartNotLoaded)
                    {
                        NXLogger.Instance.Log($"Part not loaded: {fileName}", LogLevel.Warning);
                        partLoadStatus = null;
                    }
                    else
                    {
                        NXLogger.Instance.Log($"Part loaded successfully: {fileName}", LogLevel.Info);
                    }
                }
                catch (Exception ex)
                {
                    NXLogger.Instance.LogException(ex);
                    partLoadStatus = null;
                }
            }

            return partLoadStatus;
        }

        /// <summary>
        /// Saves the currently active part in the NX theSession.
        /// </summary>
        /// <remarks>This method attempts to save the active part in the NX theSession. If the save operation
        /// is successful,ensures referenced components are also saved, keeps the part open after saving..</remarks>
        public static int SavePart()
        {
            int returnValue = 0;
            NXOpen.Session session = NXOpen.Session.GetSession();
            NXOpen.PartSaveStatus saveStatus = session.Parts.Display.Save(NXOpen.BasePart.SaveComponents.True, NXOpen.BasePart.CloseAfterSave.False);
            if (saveStatus.NumberUnsavedParts==0)
            {
                NXLogger.Instance.Log("Part saved successfully.", LogLevel.Info);
            }
            else
            {
                returnValue = 1;
                NXLogger.Instance.Log("Failed to save part.", LogLevel.Error);
            }
            saveStatus.Dispose();
            return returnValue;
        }

        /// <summary>
        /// Saves the current work part to the specified file path.
        /// </summary>
        /// <remarks>If the current work part is null, the method does nothing. If the save operation
        /// fails, an error is logged.</remarks>
        /// <param name="newFilePath">The full file path where the current work part should be saved. This must be a valid and writable file path.</param>
        public static int SaveAs(string newFilePath)
        {
            int returnValue = 0;
            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part displayPart = theSession.Parts.Display;

            if (displayPart != null)
            {
                NXOpen.PartSaveStatus partSaveStatus = displayPart.SaveAs(newFilePath);

                if (partSaveStatus.NumberUnsavedParts==0)
                {
                    NXLogger.Instance.Log($"Part Saved at : {newFilePath}", LogLevel.Info);
                }
                else
                {
                    returnValue = 1;
                    NXLogger.Instance.Log("Failed to save part.", LogLevel.Error);
                }
                partSaveStatus.Dispose();
            }
            return returnValue;
        }
    }
}
