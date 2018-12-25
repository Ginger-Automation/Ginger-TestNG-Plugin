using Amdocs.Ginger.Plugin.Core;
using GingerTestHelper;
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
        //---For below tests to pass below prerequisites are needed:
        //Java installtion + JAVA_HOME system variable configuration
        //Maven installtion + MAVEN_HOME system variable configuration



        [TestMethod]
        public void SimpleTestngXmlExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            //Act
            service.ExecuteTestNGXML(GA, OverwriteJavaHomePath:null, JavaProjectBinPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\bin"), JavaProjectResourcesPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\Resources"),
                                   TestngXmlPath: TestResources.GetTestResourcesFile(@"JavaTestNG\bin\Calculator\testng.xml"), TestngXmlParametersToOverwrite:null, OverwriteOriginalTestngXml:false,
                                   ParseConsoleOutputs:false, FailActionDueToConsoleErrors:false,
                                   ParseTestngResultsXml:true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert     
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");           
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
        }

        [TestMethod]
        public void TestNGXmlParametersOverrideExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            List<TestNGTestParameter> paramsToOveride = new List<TestNGTestParameter>();
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num1", Value = "5" });
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num2", Value = "6" });

            //Act
            service.ExecuteTestNGXML(GA, OverwriteJavaHomePath: null, JavaProjectBinPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\bin"), JavaProjectResourcesPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\Resources"),
                                   TestngXmlPath: TestResources.GetTestResourcesFile(@"JavaTestNG\bin\Calculator\testng.xml"), TestngXmlParametersToOverwrite: paramsToOveride, OverwriteOriginalTestngXml: false,
                                   ParseConsoleOutputs: true, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert              
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            Assert.AreEqual(OutputParamExist(GA, "Multipliy Result", "30"), true, "Console Multipliy Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Sum Result", "11"), true, "Console Sum Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Total Passed Test Methods", "2"), true, "TestNg Report XML Statistics Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
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
            service.ExecuteTestNGXML(GA, OverwriteJavaHomePath: null, JavaProjectBinPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\bin"), JavaProjectResourcesPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\Resources"),
                                   TestngXmlPath: TestResources.GetTestResourcesFile(@"JavaTestNG\bin\Calculator\testng.xml"), TestngXmlParametersToOverwrite: paramsToOveride, OverwriteOriginalTestngXml: false,
                                   ParseConsoleOutputs: true, FailActionDueToConsoleErrors: true,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert              
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            Assert.AreEqual(OutputParamExist(GA, "Multipliy Result", "21"), true, "Console Multipliy Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Sum Result", "10"), true, "Console Sum Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Total Passed Test Methods", "2"), true, "TestNg Report XML Statistics Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
        }

        [TestMethod]
        public void OverwriteOriginalTestngXMLExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            List<TestNGTestParameter> paramsToOveride = new List<TestNGTestParameter>();
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num1", Value = "9" });
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num2", Value = "10" });

            //Act
            service.ExecuteTestNGXML(GA, OverwriteJavaHomePath: null, JavaProjectBinPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\bin"), JavaProjectResourcesPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\Resources"),
                                   TestngXmlPath: TestResources.GetTestResourcesFile(@"JavaTestNG\bin\Calculator\testng.xml"), TestngXmlParametersToOverwrite: paramsToOveride, OverwriteOriginalTestngXml: true,
                                   ParseConsoleOutputs: true, FailActionDueToConsoleErrors: true,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert              
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual(GA.ExInfo.Contains(string.Format("TestNG XML path: '{0}'", TestResources.GetTestResourcesFile(@"JavaTestNG\bin\Calculator\testng.xml"))), true, "Using original TestNG XML validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            Assert.AreEqual(OutputParamExist(GA, "Multipliy Result", "90"), true, "Console Multipliy Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Sum Result", "19"), true, "Console Sum Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Total Passed Test Methods", "2"), true, "TestNg Report XML Statistics Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
        }

        [TestMethod]
        public void SimpleJavaFreeCommandExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            //Act
            service.ExecuteJavaFreeCommand(GA, OverwriteJavaHomePath: null, JavaWorkingFolderPath: TestResources.GetTestResourcesFolder(@"JavaTestNG"),
                                   FreeCommandArguments: string.Format("java -cp \"{0};{1}\\*\" org.testng.TestNG \"src\\Calculator\\testng.xml\"", TestResources.GetTestResourcesFolder(@"JavaTestNG\bin"), TestResources.GetTestResourcesFolder(@"JavaTestNG\Resources")),
                                   TestngXmlPath: null, TestngXmlParametersToOverwrite: null, OverwriteOriginalTestngXml: false,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\test-output"), FailActionDueToTestngResultsXmlFailures: true);

            //Assert     
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
        }

        [TestMethod]
        public void JavaFreeCommandOverrideXMLExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            List<TestNGTestParameter> paramsToOveride = new List<TestNGTestParameter>();
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num1", Value = "7" });
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num2", Value = "4" });

            //Act
            service.ExecuteJavaFreeCommand(GA, OverwriteJavaHomePath: null, JavaWorkingFolderPath: TestResources.GetTestResourcesFolder(@"JavaTestNG"),
                                   FreeCommandArguments: string.Format("java -cp \"{0};{1}\\*\" org.testng.TestNG \"src\\Calculator\\testng.xml\"", TestResources.GetTestResourcesFolder(@"JavaTestNG\bin"), TestResources.GetTestResourcesFolder(@"JavaTestNG\Resources")),
                                   TestngXmlPath: @"src\Calculator\testng.xml", TestngXmlParametersToOverwrite: paramsToOveride, OverwriteOriginalTestngXml: false,
                                   ParseConsoleOutputs: true, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: TestResources.GetTestResourcesFolder(@"JavaTestNG\test-output"), FailActionDueToTestngResultsXmlFailures: true);

            //Assert                      
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            Assert.AreEqual(OutputParamExist(GA, "Multipliy Result", "28"), true, "Console Multipliy Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Sum Result", "11"), true, "Console Sum Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Total Passed Test Methods", "2"), true, "TestNg Report XML Statistics Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
        }

        [TestMethod]
        public void SimpleMavenTestngXmlExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            //Act
            service.ExecuteMavenProjectTestNGXML(GA, OverwriteMavenHomePath: null, MavenProjectFolderPath: TestResources.GetTestResourcesFolder(@"MavenTestNG"), PerformMavenInstall:true, 
                                   TestngXmlPath: TestResources.GetTestResourcesFile(@"MavenTestNG\src\main\java\com\Calculator\testng.xml"), TestngXmlParametersToOverwrite: null, OverwriteOriginalTestngXml:false,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert     
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
        }

        [TestMethod] 
        public void SimpleMavenFreeCommandExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            //Act
            service.ExecuteMavenFreeCommand(GA, OverwriteMavenHomePath: null, MavenProjectFolderPath: TestResources.GetTestResourcesFolder(@"MavenTestNG"),
                                   FreeCommandArguments: "clean install test -DsuiteXmlFile=\"src/main/java/com/Calculator/testng.xml\"",
                                   TestngXmlPath: null, TestngXmlParametersToOverwrite: null, OverwriteOriginalTestngXml: false,
                                   ParseConsoleOutputs: false, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert     
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
        }

        [TestMethod]
        public void MavenFreeCommandXmlParametersOverrideExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            List<TestNGTestParameter> paramsToOveride = new List<TestNGTestParameter>();
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num1", Value = "5" });
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num2", Value = "4" });

            //Act
            service.ExecuteMavenFreeCommand(GA, OverwriteMavenHomePath: null, MavenProjectFolderPath: TestResources.GetTestResourcesFolder(@"MavenTestNG"),
                                   FreeCommandArguments: "clean install test -DsuiteXmlFile=\"src/main/java/com/Calculator/testng.xml\"",
                                   TestngXmlPath: TestResources.GetTestResourcesFile(@"MavenTestNG\src\main\java\com\Calculator\testng.xml"), TestngXmlParametersToOverwrite: paramsToOveride, OverwriteOriginalTestngXml: false,
                                   ParseConsoleOutputs: true, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert                      
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            Assert.AreEqual(OutputParamExist(GA, "Multipliy Result", "20"), true, "Console Multipliy Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Sum Result", "9"), true, "Console Sum Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Total Passed Test Methods", "2"), true, "TestNg Report XML Statistics Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
        }

        [TestMethod]
        public void MavenFreeCommandXmlParametersOverrideOnOriginalXmlExecutionTest()
        {
            //Arrange
            TestNGExecuterService service = new TestNGExecuterService();
            GingerAction GA = new GingerAction();

            List<TestNGTestParameter> paramsToOveride = new List<TestNGTestParameter>();
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num1", Value = "6" });
            paramsToOveride.Add(new TestNGTestParameter() { Name = "Num2", Value = "7" });

            //Act
            service.ExecuteMavenFreeCommand(GA, OverwriteMavenHomePath: null, MavenProjectFolderPath: TestResources.GetTestResourcesFolder(@"MavenTestNG"),
                                   FreeCommandArguments: "clean install test -DsuiteXmlFile=\"src/main/java/com/Calculator/testng.xml\"",
                                   TestngXmlPath: TestResources.GetTestResourcesFile(@"MavenTestNG\src\main\java\com\Calculator\testng.xml"), TestngXmlParametersToOverwrite: paramsToOveride, OverwriteOriginalTestngXml: true,
                                   ParseConsoleOutputs: true, FailActionDueToConsoleErrors: false,
                                   ParseTestngResultsXml: true, OverwriteTestngResultsXmlDefaultFolderPath: null, FailActionDueToTestngResultsXmlFailures: true);

            //Assert                      
            Assert.AreEqual((GA.Errors == null || GA.Errors.Count() == 0), true, "No Execution Errors validation");
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            Assert.AreEqual(OutputParamExist(GA, "Multipliy Result", "42"), true, "Console Multipliy Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Sum Result", "13"), true, "Console Sum Output captured validation");
            Assert.AreEqual(OutputParamExist(GA, "Total Passed Test Methods", "2"), true, "TestNg Report XML Statistics Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, "CalculatorTests- Suite Start Time"), true, "TestNg Report XML Suite details Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testMoltiple-Test Status", "PASS"), true, "TestNg Report XML Test 1 Status Parsing validation");
            Assert.AreEqual(OutputParamExist(GA, @"test1\testSum-Test Status", "PASS"), true, "TestNg Report XML Test 2 Status Parsing validation");
        }


        private bool OutputParamExist(GingerAction GA, string paramName, string paramValue=null)
        {
            IGingerActionOutputValue val = null;
            if (paramValue == null)
            {
                val = GA.Output.OutputValues.Where(x => x.Param == paramName).FirstOrDefault();
            }
            else
            {
                val = GA.Output.OutputValues.Where(x => x.Param == paramName && x.Value.ToString() == paramValue).FirstOrDefault();
            }

            if (val == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
