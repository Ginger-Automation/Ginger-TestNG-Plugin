using Amdocs.Ginger.Plugin.Core;
using GingerTestNgPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GingerTestNgPluginConsole
{
    [GingerService("TestNGExecuter", "Trigger TestNG Tests Execution by Ginger")]
    public class TestNGExecuterService
    {
        /// <summary>
        /// Execute TestNG tests using TestNG XML
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="JavaExeFullPath"></param>
        /// <param name="JavaProjectBinFolderPath"></param>
        /// <param name="JavaProjectResourcesPath"></param>
        /// <param name="TestNGXMLPath"></param>
        /// <param name="XmlParametersToOverwrite"></param>
        /// <param name="XmlTestsToExecute"></param>
        /// <param name="TestGroupsToInclude"></param>
        /// <param name="TestGroupsToExclude"></param>
        /// <param name="ContinueExecutionOnTestFailure"></param>
        /// <param name="TestNGOutputReportFolderPath"></param>
        [GingerAction("ExecuteTestNGXML", "Execute TestNG tests using TestNG XML")]
        public void ExecuteTestNGXML(IGingerAction GA, string JavaExeFullPath, string JavaProjectBinFolderPath,
                                string JavaProjectResourcesPath, string TestNGXMLPath, List<TestNGTestParameter> XmlParametersToOverwrite,
                                List<TestNGTest> XmlTestsToExecute, List<TestNGTestGroup> TestGroupsToInclude,
                                List<TestNGTestGroup> TestGroupsToExclude, string ContinueExecutionOnTestFailure, string TestNGOutputReportFolderPath)
        {
            //Set execution configurations
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;
            testNgExecuter.GingerAction = GA;

            testNgExecuter.JavaExeFullPath = JavaExeFullPath;
            testNgExecuter.JavaProjectBinFolderPath = JavaProjectBinFolderPath;
            testNgExecuter.JavaProjectResourcesPath = JavaProjectResourcesPath;
            testNgExecuter.TestNGOutputReportFolderPath = TestNGOutputReportFolderPath;
            bool.TryParse(ContinueExecutionOnTestFailure, out testNgExecuter.ContinueExecutionOnTestFailure);

            testNgExecuter.TestNgSuiteXML = new TestNGSuiteXML(TestNGXMLPath);
            testNgExecuter.XmlParametersToOverwrite = XmlParametersToOverwrite;
            testNgExecuter.XmlTestsToExecute = XmlTestsToExecute;

            testNgExecuter.TestGroupsToInclude = TestGroupsToInclude;
            testNgExecuter.TestGroupsToExclude = TestGroupsToExclude;

            testNgExecuter.Execute();
        }

        /// <summary>
        /// Execute Maven project TestNG tests using TestNG XML
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="MavenCmdFullPath"></param>
        /// <param name="MavenProjectFolderPath"></param>
        /// <param name="PerformMavenInstall"></param>
        /// <param name="TestNGXMLPath"></param>
        /// <param name="XmlParametersToOverwrite"></param>
        /// <param name="TestNGOutputReportFolderPath"></param>
        [GingerAction("ExecuteMavenProjectTestNGXML", "Execute Maven project TestNG tests using TestNG XML")]
        public void ExecuteMavenProjectTestNGXML(IGingerAction GA, string MavenCmdFullPath, string MavenProjectFolderPath, 
                        bool PerformMavenInstall, List<MavenCommandParameter> MavenCommandParameters, string TestNGXMLPath, List<TestNGTestParameter> XmlParametersToOverwrite,
                        string TestNGOutputReportFolderPath)
        {
            //Set execution configurations
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;
            testNgExecuter.GingerAction = GA;

            testNgExecuter.MavenCmdFullPath = MavenCmdFullPath;
            testNgExecuter.MavenProjectFolderPath = MavenProjectFolderPath;            
            testNgExecuter.TestNGOutputReportFolderPath = TestNGOutputReportFolderPath;
            testNgExecuter.PerformMavenInstall= PerformMavenInstall;

            testNgExecuter.MavenCommandParameters = MavenCommandParameters;

            testNgExecuter.TestNgSuiteXML = new TestNGSuiteXML(TestNGXMLPath);
            testNgExecuter.XmlParametersToOverwrite = XmlParametersToOverwrite;

            testNgExecuter.Execute();
        }

        public void ExecuteMavenCommand(IGingerAction GA, string MavenCmdFullPath, string MavenProjectFolderPath,
                string MavenCommandArguments, List<MavenCommandParameter> MavenCommandParameters)
        {
            //Set execution configurations
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.FreeCommand;
            testNgExecuter.GingerAction = GA;

            testNgExecuter.MavenCmdFullPath = MavenCmdFullPath;
            testNgExecuter.MavenProjectFolderPath = MavenProjectFolderPath;
            testNgExecuter.MavenCommandArguments = MavenCommandArguments;
            testNgExecuter.MavenCommandParameters = MavenCommandParameters;

            testNgExecuter.Execute();
        }

        //    /// <summary>
        //    /// Execute TestNG Suite based on TestNG XML
        //    /// </summary>
        //    /// <param name="GA"></param>
        //    /// <param name="JavaExeFullPath"></param>
        //    /// <param name="TestNGProjectPath"></param>
        //    /// <param name="TestNGResourcesPath"></param>
        //    /// <param name="TestNGXMLPath"></param>
        //    /// <param name="XmlParametersValuesToOverwrite"></param>
        //    /// <param name="XmlTestsToExecute"></param>
        //    /// <param name="TestGroupsToInclude"></param>
        //    /// <param name="TestGroupsToExclude"></param>
        //    /// <param name="ContinueExecutionOnTestFailure"></param>
        //    /// <param name="TestNGOutputReportFolderPath"></param>
        //    [GingerAction("ExecuteTestNGXMLStringInputs", "Execute TestNG Suite by TestNG XML using String Inputes")]                
        //    public void ExecuteTestNGXMLStringInputs(IGingerAction GA, string JavaExeFullPath, string TestNGProjectPath, 
        //                        string TestNGResourcesPath, string TestNGXMLPath, string XmlParametersValuesToOverwrite, 
        //                        string XmlTestsToExecute, string TestGroupsToInclude, string TestGroupsToExclude, 
        //                        string ContinueExecutionOnTestFailure, string TestNGOutputReportFolderPath)
        //    {
        //        //Set execution configurations
        //        TestNGExecution testNgExecuter = new TestNGExecution();
        //        testNgExecuter.JavaExeFullPath = JavaExeFullPath;
        //        testNgExecuter.TestNGProjectBinFolderPath = TestNGProjectPath;
        //        testNgExecuter.TestNGResourcesPath = TestNGResourcesPath;
        //        testNgExecuter.TestNGOutputReportFolderPath = TestNGOutputReportFolderPath;
        //        bool.TryParse(ContinueExecutionOnTestFailure, out testNgExecuter.ContinueExecutionOnTestFailure);

        //        testNgExecuter.TestNgSuiteXML = new TestNGSuiteXML(Path.GetFullPath(TestNGXMLPath));
        //        testNgExecuter.SetXmlParametersValuesToOverwriteFromString(XmlParametersValuesToOverwrite);
        //        testNgExecuter.SetXmlTestsToExecuteFromString(XmlTestsToExecute);

        //        testNgExecuter.SetTestGroupsFromString(TestGroupsToInclude, testNgExecuter.TestGroupsToInclude);
        //        testNgExecuter.SetTestGroupsFromString(TestGroupsToExclude, testNgExecuter.TestGroupsToExclude);                         
        //    }




        //[GingerAction("Play", "111")]
        //public void PlayAAA(IGingerAction GA, string JavaExeFullPath, List<TestNGTestParameter> XmlParametersValuesToOverwrite)
        //{
        //    GA.AddOutput("aa", "11");
        //    GA.AddExInfo("Extra 222");
        //    if (JavaExeFullPath == "fail")
        //    {
        //        GA.AddError("some error details 333");
        //    }
        //}
    }
}
