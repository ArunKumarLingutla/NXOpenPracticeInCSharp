using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

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

        
    }
}
