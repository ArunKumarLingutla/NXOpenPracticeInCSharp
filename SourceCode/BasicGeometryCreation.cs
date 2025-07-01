using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static NXOpen.Display.CameraBuilder;

namespace NXOpenPracticeCSharp
{
    public class BasicGeometryCreation
    {
        public static int CreatePoint(double x, double y, double z)
        {
            int returnValue = 0;

            ////This only creates a dumb geometry point — not a feature.
            ////It is visible in the graphics window, but it will not appear in the Part Navigator, because it is not a feature.
            Point point = Session.GetSession().Parts.Work.Points.CreatePoint(new Point3d(x, y, z));
            point.SetVisibility(NXOpen.SmartObject.VisibilityOption.Visible);

            ////Feature builder is ised to add it to part naviogator.
            NXOpen.Features.PointFeatureBuilder pointFeatureBuilder = Session.GetSession().Parts.Work.BaseFeatures.CreatePointFeatureBuilder(null);
            pointFeatureBuilder.Point = point;
            NXOpen.NXObject nXObject1;
            nXObject1 = pointFeatureBuilder.Commit();
            pointFeatureBuilder.Destroy();

            if (point == null)
            {
                returnValue = 1;
                NXOpen.UI.GetUI().NXMessageBox.Show("Error", NXOpen.NXMessageBox.DialogType.Error, "Failed to create point.");
            }
            else
            {
                NXLogger.Instance.Log($"Point created at ({x}, {y}, {z})", LogLevel.Info);
            }

            return returnValue;

            ////We can also use NX/Open .NET API functions
            ////https://docs.sw.siemens.com/en-US/doc/209349590/PL20190701150722612.ugopen_doc
            ////UF_POINT_ask_point_output
            ////UF_POINT_create_3_scalars
            ////UF_POINT_create_along_curve
            ////UF_POINT_create_at_conic_center
            ////UF_POINT_create_at_intersection_of_two_curves
            ////UF_POINT_create_on_arc_angle
            ////UF_POINT_create_on_curve
            ////UF_POINT_create_on_surface
            ////UF_POINT_create_surface_curve_intersection
            ////UF_POINT_create_with_offset
        }

        public static int CreateLine(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            int returnValue = 0;
            //// Create the AssociativeLineBuilder
            AssociativeLine lineNothing = null;
            AssociativeLineBuilder builder = Session.GetSession().Parts.Work.BaseFeatures.CreateAssociativeLineBuilder(lineNothing);

            // Set it to be associative
            builder.Associative = true;

            // Define the start point
            Point3d p0 = new Point3d(x1, y1, z1);
            Point pt0 = Session.GetSession().Parts.Work.Points.CreatePoint(p0);
            builder.StartPointOptions = AssociativeLineBuilder.StartOption.Point;
            builder.StartPoint.Value = pt0;

            // Define the end point
            Point3d p1 = new Point3d(x2, y2, z2);
            Point pt1 = Session.GetSession().Parts.Work.Points.CreatePoint(p1);
            builder.EndPointOptions = AssociativeLineBuilder.EndOption.Point;
            builder.EndPoint.Value = pt1;

            // Commit the associative line feature
            NXObject result = builder.Commit();

            // Clean up
            builder.Destroy();
            if (result == null)
            {
                returnValue = 1;
                NXOpen.UI.GetUI().NXMessageBox.Show("Error", NXOpen.NXMessageBox.DialogType.Error, "Failed to create line.");
            }
            else
            {
                NXLogger.Instance.Log($"Line created from ({x1}, {y1}, {z1}) to ({x2}, {y2}, {z2})", LogLevel.Info);
            }

            return returnValue;

            ////We can also use workPart.Curves.CreateLine(pt0, pt1) but it wont visible in part features
            //Line line=Session.GetSession().Parts.Work.Curves.CreateLine(new Point3d(x1,y1,z1), new Point3d(x2,y2,z2));

            ////if we use point instead of point 3d, we have to set visibility to show the line
            ////CreateLine(p0 As NXOpen.Point, p1 As NXOpen.Point) 
            //line.SetVisibility(SmartObject.VisibilityOption.Visible);

            ////We can also use NX/Open .NET API functions
            ////https://docs.sw.siemens.com/en-US/doc/209349590/PL20190701150722612.ugopen_doc
            ////UF_CURVE_create_line
            ////UF_CURVE_create_line_arc
            ////UF_CURVE_create_line_point_angle
            ////UF_CURVE_create_line_point_point
            ////UF_CURVE_create_line_point_principal_axis
            ////UF_CURVE_create_line_point_tangent
            ////UF_CURVE_create_line_tangent_point
        }

        /// <summary>
        /// create arc or circle
        /// </summary>
        /// <param name="cx">x coordinate of center point</param>
        /// <param name="cy">y coordinate of center point</param>
        /// <param name="cz">z coordinate of center point</param>
        /// <param name="radius">circle radius</param>
        /// <param name="startLimit">start limit (angle) of ARC, ignore if it is 0 or circle</param>
        /// <param name="endLimit">end limit (angle), ignore if it is 360 or circle</param>
        public static void CreateAssociativeArc(double cx, double cy, double cz, double radius, double startLimit = 0, double endLimit = 360)
        {
            // Create an AssociativeArcBuilder
            AssociativeArc arcNothing = null;
            AssociativeArcBuilder builder = Session.GetSession().Parts.Work.BaseFeatures.CreateAssociativeArcBuilder(arcNothing);

            // Set it as associative
            builder.Associative = true;

            // Set the type of arc to ArcFromCenter
            builder.Type = AssociativeArcBuilder.Types.ArcFromCenter;

            // Define the arc center point
            Point centerPoint = Session.GetSession().Parts.Work.Points.CreatePoint(new Point3d(cx, cy, cz));
            builder.CenterPoint.Value = centerPoint;

            // Define the arc radius
            builder.EndPointOptions = AssociativeArcBuilder.EndOption.Radius;
            builder.Radius.RightHandSide = radius.ToString();

            // Define the angular limits (start and end angles)
            builder.Limits.StartLimit.LimitOption = CurveExtendData.LimitOptions.Value;
            builder.Limits.EndLimit.LimitOption = CurveExtendData.LimitOptions.Value;
            builder.Limits.StartLimit.Distance.RightHandSide = startLimit.ToString(); // in degrees
            builder.Limits.EndLimit.Distance.RightHandSide = endLimit.ToString();   // in degrees

            // Commit the feature
            AssociativeArc myArcFeature = (AssociativeArc)builder.Commit();

            // Clean up
            builder.Destroy();

            // Get the created arc
            Arc myArc = (Arc)myArcFeature.GetEntities()[0];

            ////We can also use create arc function
            //var curves = Session.GetSession().Parts.Work.Curves;
            //var curve= curves.CreateArc(new Point3d(cx, cy, cz), new Vector3d(0, 1, 0), new Vector3d(0, 0, 1), radius, (Math.PI / 180.0) * startLimit, (Math.PI / 180.0) *endLimit);
            //// CreateArc(startPoint As Point3d, pointOn As Point3d, endPoint As Point3d, alternateSolution As Boolean, ByRef flipped As Boolean)
            //// CreateArc( center As Point3d, matrix As NXMatrix, radius As Double, startAngle As Double, endAngle As Double)


             ////we can use .net API's
             ////https://docs.sw.siemens.com/en-US/doc/209349590/PL20190701150722612.ugopen_doc
             ////UF_CURVE_create_arc
             ////UF_CURVE_create_arc_3point
             ////UF_CURVE_create_arc_3tangent
             ////UF_CURVE_create_arc_center_radius
             ////UF_CURVE_create_arc_center_tangent
             ////UF_CURVE_create_arc_point_center
             ////UF_CURVE_create_arc_point_point_radius
             ////UF_CURVE_create_arc_point_point_tangent
             ////UF_CURVE_create_arc_point_tangent_point
             ////UF_CURVE_create_arc_point_tangent_radius
             ////UF_CURVE_create_arc_point_tangent_tangent
             ////UF_CURVE_create_arc_tangent_point_point
             ////UF_CURVE_create_arc_tangent_point_tangent
             ////UF_CURVE_create_arc_tangent_tangent_point
             ////UF_CURVE_create_arc_tangent_tangent_radius
             ////UF_CURVE_create_arc_thru_3pts
        }

    }
}
