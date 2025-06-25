using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXOpenSetUPCSharp
{
    public class ToolSetup
    {
        public static void InitializeTool()
        {
            string BaseDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            ToolVariables.InputDirectory = Path.Combine(BaseDirectory, "Input");
            ToolVariables.OutputDirectory = Path.Combine(BaseDirectory, "Output");
            NXOpen.UI.GetUI().NXMessageBox.Show("Input Directory", NXOpen.NXMessageBox.DialogType.Information, "Input Directory: " + ToolVariables.InputDirectory);
            NXOpen.UI.GetUI().NXMessageBox.Show("Input Directory", NXOpen.NXMessageBox.DialogType.Information, "output Directory: " + ToolVariables.OutputDirectory);
            
            if (!Directory.Exists(ToolVariables.OutputDirectory))
            {
                Directory.CreateDirectory(ToolVariables.OutputDirectory);
            }
            else
            {
                FileManager.DeleteFilesInDirectory(ToolVariables.OutputDirectory);
            }
            NXLogger.Init(System.IO.Path.Combine(ToolVariables.OutputDirectory, "NXLog"), true, true);
            NXLogger.Instance.Log("Tool initialized with input directory: " + ToolVariables.InputDirectory, LogLevel.Info);
            NXLogger.Instance.Log("Output directory set to: " + ToolVariables.OutputDirectory, LogLevel.Info);
        }
    }
}
