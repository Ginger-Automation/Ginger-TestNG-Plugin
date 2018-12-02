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
        /// Execute TestNG Suite based on TestNG XML
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="JavaExeFullPath"></param>
        /// <param name="TestNGProjectPath"></param>
        /// <param name="TestNGResourcesPath"></param>
        /// <param name="TestNGXMLPath"></param>
        /// <param name="XmlParametersValuesToOverwrite"></param>
        /// <param name="XmlTestsToExecute"></param>
        /// <param name="TestGroupsToInclude"></param>
        /// <param name="TestGroupsToExclude"></param>
        /// <param name="ContinueExecutionOnTestFailure"></param>
        /// <param name="TestNGOutputReportFolderPath"></param>
        [GingerAction("ExecuteTestNGXMLStringInputs", "Execute TestNG Suite by TestNG XML using String Inputes")]                
        public void ExecuteTestNGXMLStringInputs(IGingerAction GA, string JavaExeFullPath, string TestNGProjectPath, 
                            string TestNGResourcesPath, string TestNGXMLPath, string XmlParametersValuesToOverwrite, 
                            string XmlTestsToExecute, string TestGroupsToInclude, string TestGroupsToExclude, 
                            string ContinueExecutionOnTestFailure, string TestNGOutputReportFolderPath)
        {
            //Set execution configurations
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.JavaExeFullPath = JavaExeFullPath;
            testNgExecuter.TestNGProjectPath = TestNGProjectPath;
            testNgExecuter.TestNGResourcesPath = TestNGResourcesPath;
            testNgExecuter.TestNGOutputReportFolderPath = TestNGOutputReportFolderPath;
            bool.TryParse(ContinueExecutionOnTestFailure, out testNgExecuter.ContinueExecutionOnTestFailure);

            testNgExecuter.TestNgSuiteXML = new TestNGSuiteXML(Path.GetFullPath(TestNGXMLPath));
            testNgExecuter.SetXmlParametersValuesToOverwriteFromString(XmlParametersValuesToOverwrite);
            testNgExecuter.SetXmlTestsToExecuteFromString(XmlTestsToExecute);

            testNgExecuter.SetTestGroupsFromString(TestGroupsToInclude, testNgExecuter.TestGroupsToInclude);
            testNgExecuter.SetTestGroupsFromString(TestGroupsToExclude, testNgExecuter.TestGroupsToExclude);           
              
        }


        /// <summary>
        /// Execute TestNG Suite based on TestNG XML
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="JavaExeFullPath"></param>
        /// <param name="TestNGProjectPath"></param>
        /// <param name="TestNGResourcesPath"></param>
        /// <param name="TestNGXMLPath"></param>
        /// <param name="XmlParametersValuesToOverwrite"></param>
        /// <param name="XmlTestsToExecute"></param>
        /// <param name="TestGroupsToInclude"></param>
        /// <param name="TestGroupsToExclude"></param>
        /// <param name="ContinueExecutionOnTestFailure"></param>
        /// <param name="TestNGOutputReportFolderPath"></param>
        [GingerAction("ExecuteTestNGXMLListInputs", "Execute TestNG Suite by TestNG XML using List Inputes")]
        public void ExecuteTestNGXMLListInputs(IGingerAction GA, string JavaExeFullPath, string TestNGProjectPath,
                                    string TestNGResourcesPath, string TestNGXMLPath, List<TestNGTestParameter> XmlParametersValuesToOverwrite, 
                                    List<TestNGTest> XmlTestsToExecute, List<TestNGTestGroup> TestGroupsToInclude, 
                                    List<TestNGTestGroup> TestGroupsToExclude, string ContinueExecutionOnTestFailure, string TestNGOutputReportFolderPath)
        {
            //Set execution configurations
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.JavaExeFullPath = JavaExeFullPath;
            testNgExecuter.TestNGProjectPath = TestNGProjectPath;
            testNgExecuter.TestNGResourcesPath = TestNGResourcesPath;
            testNgExecuter.TestNGOutputReportFolderPath = TestNGOutputReportFolderPath;
            bool.TryParse(ContinueExecutionOnTestFailure, out testNgExecuter.ContinueExecutionOnTestFailure);

            testNgExecuter.TestNgSuiteXML = new TestNGSuiteXML(Path.GetFullPath(TestNGXMLPath));
            testNgExecuter.XmlParametersValuesToOverwrite= XmlParametersValuesToOverwrite;
            testNgExecuter.XmlTestsToExecute= XmlTestsToExecute;

            testNgExecuter.TestGroupsToInclude = TestGroupsToInclude;
            testNgExecuter.TestGroupsToExclude= TestGroupsToExclude;
        }
    }
}
