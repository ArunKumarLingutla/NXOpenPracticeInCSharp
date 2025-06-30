using NXOpen;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXOpenPracticeCSharp
{
    public class NXOpenPracticeCSharp
    {
        //class members
        private static NXOpen.Session theSession = null;
        private static NXOpen.UF.UFSession theUFSession = null;
        private static NXOpen.UI theUI = null;

        public static void Main(string[] args)
        {
            try
            {
                theSession = Session.GetSession();
                theUFSession = NXOpen.UF.UFSession.GetUFSession();
                theUI = NXOpen.UI.GetUI();
                NXOpen.Part workPart = theSession.Parts.Work;
                NXOpen.Part displayPart = theSession.Parts.Display;
                ToolSetup.InitializeTool();

                ////*******Create a part file********
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string inputFilePath = Path.Combine(ToolVariables.InputDirectory, "SamplePart.prt");
                PartFileOperations.CreatePartFile(inputFilePath);

                ////********Open a part file********
                //NXOpen.PartLoadStatus loadStatus = PartFileOperations.OpenPart(inputFilePath);
                //NXLogger.Instance.Log($"number of unloaded parts : {loadStatus.NumberUnloadedParts}", LogLevel.Info);

                ////********Save the part file********
                PartFileOperations.SavePart();
                //PartFileOperations.SaveAs(Path.Combine(desktopPath, "SaveAsPart1.prt"));


                ////********Create a point********
                BasicGeometryCreation.CreatePoint(10.0, 20.0, 30.0);

                ////********Create a line********
                BasicGeometryCreation.CreateLine(0.0, 0.0, 0.0, 10.0, 10.0, 10.0);

            }
            catch (Exception)
            {
                NXLogger.Instance.Dispose();
            }
        }

        public static int GetUnloadOption(string arg)
        {
            //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
            return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }

        //------------------------------------------------------------------------------
        // Following method cleanup any housekeeping chores that may be needed.
        // This method is automatically called by NX.
        //------------------------------------------------------------------------------
        public static void UnloadLibrary(string arg)
        {
            try
            {
                //---- Enter your code here -----
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Main", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }
    }
}
