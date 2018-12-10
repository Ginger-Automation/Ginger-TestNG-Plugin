using Amdocs.Ginger.Plugin.Core;
using GingerTestNgPlugin;
using GingerTestNgPluginConsole;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GingerTestNgPluginTest
{
    [TestClass]
    public class TestNGExecuterServiceExecutionTest
    {
        [TestMethod]
        public void SimpleTestngXmlExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            //Act
            service.ExecuteTestNGXML(GA, OverrideJavaHomePath:@"C:\Program Files\Java\jdk1.8.0_191\bin\java.exe", JavaProjectBinPath:@"C:\Users\menik\eclipse-workspace\Learn-TestNG\bin", JavaProjectResourcesPath:@"C:\Users\menik\.p2\pool\plugins\*",
                                   TestngXmlPath:@"C:\Users\menik\eclipse-workspace\Learn-TestNG\src\Groups\testng.xml", TestngXmlParametersToOverride:null,
                                   ParseConsoleOutputs:false, FailActionDueToConsoleErrors:false,
                                   ParseTestngResultsXml:true, OverrideTestngResultsXmlDefaultFolderPath:null, FailActionDueToTestngResultsFailures:false);

            //Assert           
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found");
        }

        [TestMethod]
        public void SimpleMavenTestngXmlExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            //Act
            service.ExecuteMavenProjectTestNGXML(GA, OverrideMavenHomePath: @"C:\Program Files (x86)\apache-maven-3.5.3\bin\mvn.cmd", MavenProjectFolderPath: @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test", PerformMavenInstall:true, 
                                   TestngXmlPath: @"C:\TestNG_WORKSPACE\PBG Flows\CustomeXMLs\Dynamic Device from CouchBase.xml",TestngXmlParametersToOverride: null,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverrideTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsFailures: true);

            //Assert           
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count>0), true, "Execution Output values found");
        }

        [TestMethod]
        public void TestNGXmlParametersOverrideExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            List<TestNGTestParameter> paramsToOveride = new List<TestNGTestParameter>();
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num1", Value = "55" });
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num2", Value = "66" });

            //Act
            service.ExecuteTestNGXML(GA, OverrideJavaHomePath: @"C:\Program Files\Java\jdk1.8.0_191\bin\java.exe", JavaProjectBinPath: @"C:\Users\menik\eclipse-workspace\Learn-TestNG\bin", JavaProjectResourcesPath: @"C:\Users\menik\.p2\pool\plugins\*",
                                   TestngXmlPath: @"C:\Users\menik\eclipse-workspace\Learn-TestNG\src\Calculator\testng.xml", TestngXmlParametersToOverride: paramsToOveride,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverrideTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsFailures: true);

            //Assert           
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found");
        }

        [TestMethod] 
        public void SimpleMavenFreeCommandExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            //Act
            service.ExecuteMavenFreeCommand(GA, OverrideMavenHomePath: @"C:\Program Files (x86)\apache-maven-3.5.3\bin\mvn.cmd", MavenProjectFolderPath: @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test",
                                   FreeCommandArguments: "clean install test -Dsurefire.suiteXmlFiles=\"C:\\TestNG_WORKSPACE\\PBG Flows\\CustomeXMLs\\Dynamic Device from CouchBase.xml\"",
                                   CommandParametersToOverride: null,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverrideTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsFailures: true);

            //Assert           
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found");
        }

        [TestMethod]
        public void MavenFreeCommandParametersOverrideExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            List<CommandParameter> paramsToOveride = new List<CommandParameter>();
            paramsToOveride.Add(new CommandParameter() { Name = "surefire.suiteXmlFiles", Value = "\"C:\\TestNG_WORKSPACE\\PBG Flows\\CustomeXMLs\\Dynamic Device from CouchBase.xml\"" });
            paramsToOveride.Add(new CommandParameter() { Name = "-DthreadPoolSize", Value = "2" });

            //Act
            service.ExecuteMavenFreeCommand(GA, OverrideMavenHomePath: @"C:\Program Files (x86)\apache-maven-3.5.3\bin\mvn.cmd", MavenProjectFolderPath: @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test",
                                   FreeCommandArguments: "clean install test -Dsurefire.suiteXmlFiles=\"xx bb cc33\testng.xml\" -DthreadPoolSize=1",
                                   CommandParametersToOverride: paramsToOveride,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverrideTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsFailures: true);

            //Assert           
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found");
        }



        [TestMethod]
        public void ConsoleOutputsReadExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            List<TestNGTestParameter> paramsToOveride = new List<TestNGTestParameter>();
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num1", Value = "7" });
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num2", Value = "3" });

            //Act
            service.ExecuteTestNGXML(GA, OverrideJavaHomePath: @"C:\Program Files\Java\jdk1.8.0_191\bin\java.exe", JavaProjectBinPath: @"C:\Users\menik\eclipse-workspace\Learn-TestNG\bin", JavaProjectResourcesPath: @"C:\Users\menik\.p2\pool\plugins\*",
                                   TestngXmlPath: @"C:\Users\menik\eclipse-workspace\Learn-TestNG\src\Calculator\testng.xml", TestngXmlParametersToOverride: paramsToOveride,
                                   ParseConsoleOutputs: true, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverrideTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsFailures: true);

            //Assert           
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found");
            Assert.AreEqual((GA.Output.OutputValues.Where(x => ((x.Param == "SUM result") && (x.Value.ToString() == "10"))).FirstOrDefault() != null), true, "Result Console output value exist");
            Assert.AreEqual((GA.Output.OutputValues.Where(x => ((x.Param == "Moltipy result") && (x.Value.ToString() == "21"))).FirstOrDefault() != null), true, "Result Console output value exist");
        }

    }
}
