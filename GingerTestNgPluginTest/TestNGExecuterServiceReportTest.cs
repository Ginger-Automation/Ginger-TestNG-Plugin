using Amdocs.Ginger.Plugin.Core;
using GingerTestHelper;
using GingerTestNgPlugin;
using GingerTestNgPluginConsole;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GingerTestNgPluginTest
{
    [TestClass]
    public class TestNGExecuterServiceReportTest
    {
        [TestMethod]
        public void TestNGExecutionResultsXMlToObjectValidation()
        {
            //Arrange
            GingerAction GA = new GingerAction();
            string resultsXmlPath = TestResources.GetTestResourcesFile("testng-results.xml");

            //Act
            TestNGReportXML ngReport = new TestNGReportXML(resultsXmlPath);            

            //Assert              
            Assert.AreEqual(string.IsNullOrEmpty(ngReport.LoadError), true, "No report parsing errors validation");
            Assert.AreEqual(ngReport.TotalTestMethodsNum, 101, "Report tests Total statistics validation");            
            Assert.AreEqual(ngReport.FailedTestMethodsNum, 1, "Report tests Failed statistics validation");
            Assert.AreEqual(ngReport.PassedTestMethodsNum, 3, "Report tests Passed statistics validation");
            Assert.AreEqual(ngReport.SkippedTestMethodsNum, 14, "Report tests Skipped statistics validation");
            Assert.AreEqual(ngReport.IgnoredTestMethodsNum, 83, "Report tests Ignored statistics validation");
            Assert.AreEqual(ngReport.ReportSuites.Count, 1, "Report Suites number validation");
            Assert.AreEqual(ngReport.ReportSuites[0].Name, "Dynamic Accessory from Search", "Report Suite Name validation");
            Assert.AreEqual(ngReport.ReportSuites[0].Tests.Count, 4, "Report Suite Tests Number validation");
        }

        [TestMethod]
        public void TestNGExecutionResultsParsingValidation()
        {
            //Arrange
            GingerAction GA = new GingerAction();
            string resultsXmlPath = TestResources.GetTestResourcesFile("testng-results.xml");

            //Act
            TestNGReportXML ngReport = new TestNGReportXML(resultsXmlPath);
            ngReport.ParseTestNGReport(GA, true);

            //Assert              
            Assert.AreEqual(string.IsNullOrEmpty(ngReport.LoadError), true, "No report parsing errors validation");
            Assert.AreEqual(GA.Errors, @"The Test method 'Dynamic Accessory from Search\getAllCategories' failed with the error: 'Connection reset'", "Error been added correctly to Ginger Action validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Total Test Methods", "101"), true, "Output Value- '' validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Total Passed Test Methods", "3"), true, "Output Value- 'Total Passed Test Methods' validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Total Failed Test Methods", "1"), true, "Output Value- 'Total Failed Test Methods' validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Total Skipped Test Methods", "14"), true, "Output Value- 'Total Skipped Test Methods' validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Total Ignored Test Methods", "83"), true, "Output Value- 'Total Ignored Test Methods' validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Dynamic Accessory from Search- Suite Start Time", "03-Dec-18 3:03:19 PM"), true, "Output Value- 'Dynamic Accessory from Search- Suite Start Time' validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Dynamic Accessory from Search- Suite Finish Time", "03-Dec-18 3:03:40 PM"), true, "Output Value- 'Dynamic Accessory from Search- Suite Finish Time' validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Dynamic Accessory from Search- Suite Duration (MS)", "21069"), true, "Output Value- 'Dynamic Accessory from Search- Suite Duration (MS)' validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Automation_Setup\\initialAutomationSetup-Test Status", "PASS"), true, "Output Value- PASS test validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Dynamic Accessory from Search\\selectProductOfferingDynamic-Test Status", "SKIP"), true, "Output Value- SKIP test validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Dynamic Accessory from Search\\selectProductOfferingDynamic-Error Message", "Attribute: categoryIds is not exist in context."), true, "Output Value- Skipped test Error Message validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Dynamic Accessory from Search\\getAllCategories-Test Status", "FAIL"), true, "Output Value- FAIL test validation");
            Assert.AreEqual(General.OutputParamExist(GA, "Dynamic Accessory from Search\\getAllCategories-Error Message", "Connection reset"), true, "Output Value- Failed test Error Message validation");
        }

    }
}
