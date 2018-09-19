using Amdocs.Ginger.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using GingerTestNgPlugin;

namespace StandAloneActions
{
    [GingerService("TestNG", "Execute TestNG scripts from Ginger")]
    public class TestNgAction : IGingerService, IStandAloneAction
    {
        [GingerAction("RunTestNgSuite", "Run TestNG Suite from XMl")]
 
        public void RunTestNgSuite(GingerAction GA,string TestNgXMlName,string ProjectLocation,string LibraryFolder,string JavaLocation,Dictionary<string,string>UserParameters)
        {
            string TestNgxmlPath;
            if(ProjectLocation.EndsWith("\\"))
            {
                TestNgxmlPath = ProjectLocation + TestNgXMlName;
            }
            else
            {

                TestNgxmlPath = ProjectLocation + "\\"+TestNgXMlName;
            }
            string TestNGXML = System.IO.File.ReadAllText(TestNgxmlPath);
            TestNGSuite Suite = new TestNGSuite(TestNGXML);
            TestNGReport Report = Suite.Execute(TestNgXMlName,ProjectLocation,LibraryFolder,"");

        }
    }
}
