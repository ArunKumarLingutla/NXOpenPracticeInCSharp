using NXOpen;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXOpenSetUPCSharp
{
    public class NXOpenSetUPCsharp
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
