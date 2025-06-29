using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NXOpenPracticeCSharp;

namespace NXOpenPracticeCSharpTest
{
    [TestFixture]
    public class NXOpenPracticeCSharpTest
    {
        [OneTimeSetUp]
        public void Test_ToolInitialization()
        {
            string rootDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            //string workingdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            //string rootDirectory1 = TestContext.CurrentContext.TestDirectory;
        }

    }
}
