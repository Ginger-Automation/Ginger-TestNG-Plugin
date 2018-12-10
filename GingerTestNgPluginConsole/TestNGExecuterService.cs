using Amdocs.Ginger.Plugin.Core;
using GingerTestNgPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GingerTestNgPluginConsole
{
    [GingerService("TestNGExecuter", "Trigger TestNG Tests Execution via Ginger")]
    public class TestNGExecuterService
    {
        /// <summary>
        /// Execute TestNG tests by TestNG XML
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="OverrideJavaHomePath"></param>
        /// <param name="JavaProjectBinPath"></param>
        /// <param name="JavaProjectResourcesPath"></param>
        /// <param name="TestngXmlPath"></param>
        /// <param name="TestngXmlParametersToOverride"></param>
        /// <param name="ParseConsoleOutputs"></param>
        /// <param name="FailActionDueToConsoleErrors"></param>
        /// <param name="ParseTestngResultsXml"></param>
        /// <param name="OverrideTestngResultsXmlDefaultFolderPath"></param>
        /// <param name="FailActionDueToTestngResultsFailures"></param>
        [GingerAction("ExecuteTestNGXML", "Execute TestNG Tests by TestNG XML")]
        public void ExecuteTestNGXML(IGingerAction GA, string OverrideJavaHomePath, string JavaProjectBinPath, string JavaProjectResourcesPath,
                                 string TestngXmlPath, List<TestNGTestParameter> TestngXmlParametersToOverride,
                                 bool ParseConsoleOutputs, bool FailActionDueToConsoleErrors, 
                                 bool ParseTestngResultsXml, string OverrideTestngResultsXmlDefaultFolderPath, bool FailActionDueToTestngResultsFailures)
        {
            //Set execution configurations
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;
            testNgExecuter.GingerAction = GA;

            testNgExecuter.JavaExeFullPath = OverrideJavaHomePath;
            testNgExecuter.JavaProjectBinPath = JavaProjectBinPath;
            testNgExecuter.JavaProjectResourcesPath = JavaProjectResourcesPath;

            testNgExecuter.TestngXmlPath = TestngXmlPath;
            testNgExecuter.TestngXmlParametersToOverride = TestngXmlParametersToOverride;

            testNgExecuter.ParseConsoleOutputs = ParseConsoleOutputs;
            testNgExecuter.FailActionDueToConsoleErrors = FailActionDueToConsoleErrors;
            testNgExecuter.ParseTestngResultsXml = ParseTestngResultsXml;
            testNgExecuter.TestngResultsXmlFolderPath = OverrideTestngResultsXmlDefaultFolderPath;
            testNgExecuter.FailActionDueToTestngResultsFailures = FailActionDueToTestngResultsFailures;

            testNgExecuter.Execute();
        }

        /// <summary>
        /// Execute Maven project TestNG tests by TestNG XML
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="OverrideMavenHomePath"></param>
        /// <param name="MavenProjectFolderPath"></param>
        /// <param name="PerformMavenInstall"></param>
        /// <param name="TestngXmlPath"></param>
        /// <param name="TestngXmlParametersToOverride"></param>
        /// <param name="ParseConsoleOutputs"></param>
        /// <param name="FailActionDueToConsoleErrors"></param>
        /// <param name="ParseTestngResultsXml"></param>
        /// <param name="OverrideTestngResultsXmlDefaultFolderPath"></param>
        /// <param name="FailActionDueToTestngResultsFailures"></param>
        [GingerAction("ExecuteMavenProjectTestNGXML", "Execute Maven Project TestNG Tests by TestNG XML")]
        public void ExecuteMavenProjectTestNGXML(IGingerAction GA, string OverrideMavenHomePath, string MavenProjectFolderPath, bool PerformMavenInstall,
                        string TestngXmlPath, List<TestNGTestParameter> TestngXmlParametersToOverride,
                        bool ParseConsoleOutputs, bool FailActionDueToConsoleErrors,
                        bool ParseTestngResultsXml, string OverrideTestngResultsXmlDefaultFolderPath, bool FailActionDueToTestngResultsFailures)
        {
            //Set execution configurations
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Maven;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;
            testNgExecuter.GingerAction = GA;

            testNgExecuter.MavenCmdFullPath = OverrideMavenHomePath;
            testNgExecuter.MavenProjectFolderPath = MavenProjectFolderPath;                        
            testNgExecuter.PerformMavenInstall = PerformMavenInstall;

            testNgExecuter.ParseConsoleOutputs = ParseConsoleOutputs;

            testNgExecuter.TestngXmlPath = TestngXmlPath;            
            testNgExecuter.TestngXmlParametersToOverride = TestngXmlParametersToOverride;

            testNgExecuter.ParseConsoleOutputs = ParseConsoleOutputs;
            testNgExecuter.FailActionDueToConsoleErrors = FailActionDueToConsoleErrors;
            testNgExecuter.ParseTestngResultsXml = ParseTestngResultsXml;
            testNgExecuter.TestngResultsXmlFolderPath = OverrideTestngResultsXmlDefaultFolderPath;
            testNgExecuter.FailActionDueToTestngResultsFailures = FailActionDueToTestngResultsFailures;

            testNgExecuter.Execute();
        }


        /// <summary>
        /// Execute Maven fully customized command
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="OverrideMavenHomePath"></param>
        /// <param name="MavenProjectFolderPath"></param>
        /// <param name="FreeCommandArguments"></param>
        /// <param name="CommandParametersToOverride"></param>
        /// <param name="ParseConsoleOutputs"></param>
        /// <param name="FailActionDueToConsoleErrors"></param>
        /// <param name="ParseTestngResultsXml"></param>
        /// <param name="OverrideTestngResultsXmlDefaultFolderPath"></param>
        /// <param name="FailActionDueToTestngResultsFailures"></param>
        [GingerAction("ExecuteMavenFreeCommand", "Execute Maven Free Command")]
        public void ExecuteMavenFreeCommand(IGingerAction GA, string OverrideMavenHomePath, string MavenProjectFolderPath,
                       string FreeCommandArguments, List<CommandParameter> CommandParametersToOverride,
                       bool ParseConsoleOutputs, bool FailActionDueToConsoleErrors,
                       bool ParseTestngResultsXml, string OverrideTestngResultsXmlDefaultFolderPath, bool FailActionDueToTestngResultsFailures)
        {
            //Set execution configurations
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Maven;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.FreeCommand;
            testNgExecuter.GingerAction = GA;

            testNgExecuter.MavenCmdFullPath = OverrideMavenHomePath;
            testNgExecuter.MavenProjectFolderPath = MavenProjectFolderPath;

            testNgExecuter.FreeCommandArguments = FreeCommandArguments;
            testNgExecuter.CommandParametersToOverride = CommandParametersToOverride;            

            testNgExecuter.ParseConsoleOutputs = ParseConsoleOutputs;
            testNgExecuter.FailActionDueToConsoleErrors = FailActionDueToConsoleErrors;
            testNgExecuter.ParseTestngResultsXml = ParseTestngResultsXml;
            testNgExecuter.TestngResultsXmlFolderPath = OverrideTestngResultsXmlDefaultFolderPath;
            testNgExecuter.FailActionDueToTestngResultsFailures = FailActionDueToTestngResultsFailures;

            testNgExecuter.Execute();
        }
    }
}
