using NXOpen;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXOpenPracticeCSharp
{
    public class BasicGeometryCreation
    {
        public static int CreatePoint(double x,double y,double z)
        {
            int returnValue = 0;

            ////This only creates a dumb geometry point — not a feature.
            ////It is visible in the graphics window, but it will not appear in the Part Navigator, because it is not a feature.
            Point point =Session.GetSession().Parts.Work.Points.CreatePoint(new Point3d(x,y,z));
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
            // Create the AssociativeLineBuilder
            AssociativeLine lineNothing = null;
            AssociativeLineBuilder builder = Session.GetSession().Parts.Work.BaseFeatures.CreateAssociativeLineBuilder(lineNothing);

            // Set it to be associative
            builder.Associative = true;

            // Define the start point
            Point3d p0 = new Point3d(x1,y1,z1);
            Point pt0 = Session.GetSession().Parts.Work.Points.CreatePoint(p0);
            builder.StartPointOptions = AssociativeLineBuilder.StartOption.Point;
            builder.StartPoint.Value = pt0;

            // Define the end point
            Point3d p1 = new Point3d(x2,y2,z2);
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
    }
}
