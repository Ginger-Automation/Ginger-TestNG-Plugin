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
            service.ExecuteTestNGXML(GA, OverwriteJavaHomePath:@"C:\Program Files\Java\jdk1.8.0_191\bin\java.exe", JavaProjectBinPath:@"C:\Users\menik\eclipse-workspace\Learn-TestNG\bin", JavaProjectResourcesPath:@"C:\Users\menik\.p2\pool\plugins\*",
                                   TestngXmlPath:@"C:\Users\menik\eclipse-workspace\Learn-TestNG\src\Groups\testng.xml", TestngXmlParametersToOverride:null, OverwriteOriginalTestngXML:false,
                                   ParseConsoleOutputs:false, FailActionDueToConsoleErrors:false,
                                   ParseTestngResultsXml:true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: false);

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
            service.ExecuteMavenProjectTestNGXML(GA, OverwriteMavenHomePath: @"C:\Program Files (x86)\apache-maven-3.5.3\bin\mvn.cmd", MavenProjectFolderPath: @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test", PerformMavenInstall:true, 
                                   TestngXmlPath: @"C:\TestNG_WORKSPACE\PBG Flows\CustomeXMLs\Dynamic Device from CouchBase.xml",TestngXmlParametersToOverride: null, OverwriteOriginalTestngXML:false,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

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
            service.ExecuteTestNGXML(GA, OverwriteJavaHomePath: @"C:\Program Files\Java\jdk1.8.0_191\bin\java.exe", JavaProjectBinPath: @"C:\Users\menik\eclipse-workspace\Learn-TestNG\bin", JavaProjectResourcesPath: @"C:\Users\menik\.p2\pool\plugins\*",
                                   TestngXmlPath: @"C:\Users\menik\eclipse-workspace\Learn-TestNG\src\Calculator\testng.xml", TestngXmlParametersToOverride: paramsToOveride, OverwriteOriginalTestngXML: false,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

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
            service.ExecuteMavenFreeCommand(GA, OverwriteMavenHomePath: @"C:\Program Files (x86)\apache-maven-3.5.3\bin\mvn.cmd", MavenProjectFolderPath: @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test",
                                   FreeCommandArguments: "clean install test -Dsurefire.suiteXmlFiles=\"C:\\TestNG_WORKSPACE\\PBG Flows\\CustomeXMLs\\Dynamic Device from CouchBase.xml\"",
                                   TestngXmlPath: null, TestngXmlParametersToOverride: null, OverwriteOriginalTestngXML: false,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert           
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found");
        }

        [TestMethod]
        public void MavenFreeCommandXmlParametersOverrideExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            List<TestNGTestParameter> paramsToOveride = new List<TestNGTestParameter>();
            paramsToOveride.Add(new TestNGTestParameter() { Name = "scid", ParentNodeName= "Dynamic Device from DB", Value = "55" });
            paramsToOveride.Add(new TestNGTestParameter() { Name = "customerSubType", ParentNodeName= "Create UserIndividualCC",  Value = "66" });
            paramsToOveride.Add(new TestNGTestParameter() { Name = "executionLevel",  Value = "77" });

            //Act
            service.ExecuteMavenFreeCommand(GA, OverwriteMavenHomePath: @"C:\Program Files (x86)\apache-maven-3.5.3\bin\mvn.cmd", MavenProjectFolderPath: @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test",
                                   FreeCommandArguments: @"clean install test -Dsurefire.suiteXmlFiles="+"\"C:\\TestNG_WORKSPACE\\PBG Flows\\CustomeXMLs\\Dynamic Device from CouchBase.xml\"",
                                   TestngXmlPath: @"C:\TestNG_WORKSPACE\PBG Flows\CustomeXMLs\Dynamic Device from CouchBase.xml", TestngXmlParametersToOverride: paramsToOveride, OverwriteOriginalTestngXML: false,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

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
            service.ExecuteTestNGXML(GA, OverwriteJavaHomePath: @"C:\Program Files\Java\jdk1.8.0_191\bin\java.exe", JavaProjectBinPath: @"C:\Users\menik\eclipse-workspace\Learn-TestNG\bin", JavaProjectResourcesPath: @"C:\Users\menik\.p2\pool\plugins\*",
                                   TestngXmlPath: @"C:\Users\menik\eclipse-workspace\Learn-TestNG\src\Calculator\testng.xml", TestngXmlParametersToOverride: paramsToOveride, OverwriteOriginalTestngXML:false,
                                   ParseConsoleOutputs: true, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert           
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found");
            Assert.AreEqual((GA.Output.OutputValues.Where(x => ((x.Param == "SUM result") && (x.Value.ToString() == "10"))).FirstOrDefault() != null), true, "Result Console output value exist");
            Assert.AreEqual((GA.Output.OutputValues.Where(x => ((x.Param == "Moltipy result") && (x.Value.ToString() == "21"))).FirstOrDefault() != null), true, "Result Console output value exist");
        }

    }
}
