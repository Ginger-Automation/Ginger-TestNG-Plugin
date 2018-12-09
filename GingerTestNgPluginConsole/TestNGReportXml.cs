using GingerTestNgPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Amdocs.Ginger.Plugin.Core;

namespace GingerTestNgPluginConsole
{
    public class TestNGReportXML
    {
        public string ReportXmlFilePath;
        XmlDocument ReportXml;
        public List<TestNGTestSuite> ReportSuites;
        public string LoadError = null;

        public Int32 PassedTestMethodsNum;
        public Int32 FailedTestMethodsNum;
        public Int32 SkippedTestMethodsNum;
        public Int32 IgnoredTestMethodsNum;
        public Int32 TotalTestMethodsNum;

        ///<summary>TestNGReportXML Object from TestNg Output Report XML file
        ///</summary>
        public TestNGReportXML(string xmlFilePath)
        {
            ReportXmlFilePath = xmlFilePath;
            if (LoadReportXmlFromFile())
            {
                LoadSuiteReportObjectFromXml();
            }
        }

        private bool LoadReportXmlFromFile()
        {
            try
            {
                ReportXmlFilePath = Path.GetFullPath(ReportXmlFilePath);
                if (File.Exists(ReportXmlFilePath) == false)
                {
                    LoadError = String.Format("Failed to find the TestNG Report XML file at: '{0}'", ReportXmlFilePath);
                    return false;
                }

                ReportXml = new XmlDocument();
                ReportXml.LoadXml(System.IO.File.ReadAllText(ReportXmlFilePath));

                return true;
            }
            catch (Exception ex)
            {
                LoadError = string.Format("Failed to load the TestNG Report XML from path: '{0}' due to the Error: '{1}'", ReportXmlFilePath, ex.Message);
                return false;
            }
        }

        private bool LoadSuiteReportObjectFromXml()
        {
            try
            {
                if (ReportXml == null)
                {
                    return false;
                }
                
                //General counters
                Int32.TryParse(ReportXml.DocumentElement.GetAttribute("passed").ToString(), out PassedTestMethodsNum);
                Int32.TryParse(ReportXml.DocumentElement.GetAttribute("failed").ToString(), out FailedTestMethodsNum);
                Int32.TryParse(ReportXml.DocumentElement.GetAttribute("skipped").ToString(), out SkippedTestMethodsNum);
                Int32.TryParse(ReportXml.DocumentElement.GetAttribute("ignored").ToString(), out IgnoredTestMethodsNum);
                Int32.TryParse(ReportXml.DocumentElement.GetAttribute("total").ToString(), out TotalTestMethodsNum);

                //Executed Suites details
                ReportSuites = new List<TestNGTestSuite>();
                foreach (XmlElement xmlReportSuite in ReportXml.GetElementsByTagName("suite"))
                {
                    TestNGTestSuite ngSuite = new TestNGTestSuite();
                    ngSuite.Name = xmlReportSuite.GetAttribute("name").ToString();
                    ngSuite.Parameters = GetTestParametersFromXmlElement(xmlReportSuite);
                    DateTime.TryParse(xmlReportSuite.GetAttribute("started-at").ToString(), out ngSuite.ExecutionStartTime);
                    DateTime.TryParse(xmlReportSuite.GetAttribute("finished-at").ToString(), out ngSuite.ExecutionEndTime);
                    Int32.TryParse(xmlReportSuite.GetAttribute("duration-ms").ToString(), out ngSuite.ExecutionDurationMS);

                    ngSuite.Tests = new List<TestNGTest>();
                    foreach (XmlElement xmlReportTest in xmlReportSuite.GetElementsByTagName("test"))
                    {
                        TestNGTest ngTest = new TestNGTest();
                        ngTest.Name = xmlReportTest.GetAttribute("name").ToString();
                        DateTime.TryParse(xmlReportTest.GetAttribute("started-at").ToString(), out ngTest.ExecutionStartTime);
                        DateTime.TryParse(xmlReportTest.GetAttribute("finished-at").ToString(), out ngTest.ExecutionEndTime);
                        Int32.TryParse(xmlReportTest.GetAttribute("duration-ms").ToString(), out ngTest.ExecutionDurationMS);
                        ngTest.Parameters = GetTestParametersFromXmlElement(xmlReportTest);
                        ngTest.Classes = GetTestClassesFromXmlElement(xmlReportTest);
                        ngSuite.Tests.Add(ngTest);
                    }
                    ReportSuites.Add(ngSuite);
                }                

                return true;
            }
            catch (Exception ex)
            {
                LoadError = string.Format("Failed to load the TestNG Suite Report Object from XML due to the Error: '{0}'", ex.Message);
                return false;
            }
        }

        private List<TestNGTestClass> GetTestClassesFromXmlElement(XmlElement xmlTest)
        {
            List<TestNGTestClass> ngClasses = new List<TestNGTestClass>();
            foreach (XmlElement xmlClass in xmlTest.GetElementsByTagName("class"))
            {
                TestNGTestClass ngClass = new TestNGTestClass
                {
                    Name = xmlClass.Attributes.GetNamedItem("name").Value,
                    Parameters = GetTestParametersFromXmlElement(xmlClass),
                    Methods = GetTestMethods(xmlClass),
                };
                ngClasses.Add(ngClass);
            }

            return ngClasses;
        }

        private List<TestNGTestMethod> GetTestMethods(XmlElement xmlClass)
        {
            List<TestNGTestMethod> ngMethods = new List<TestNGTestMethod>();

            foreach (XmlElement xmlReportMethod in xmlClass.GetElementsByTagName("test-method"))
            {
                TestNGTestMethod ngReportMethod = new TestNGTestMethod();
                ngReportMethod.Name = xmlReportMethod.Attributes.GetNamedItem("name").Value;
                Enum.TryParse(xmlReportMethod.GetAttribute("status").ToString(), true, out ngReportMethod.ExecutionStatus);
                ngReportMethod.ExecutionSignature = xmlReportMethod.GetAttribute("signature").ToString();                
                DateTime.TryParse(xmlReportMethod.GetAttribute("started-at").ToString(), out ngReportMethod.ExecutionStartTime);
                DateTime.TryParse(xmlReportMethod.GetAttribute("finished-at").ToString(), out ngReportMethod.ExecutionEndTime);
                Int32.TryParse(xmlReportMethod.GetAttribute("duration-ms").ToString(), out ngReportMethod.ExecutionDurationMS);

                XmlNodeList exceptions = xmlReportMethod.GetElementsByTagName("exception");
                if (exceptions.Count > 0)
                {
                    TestNGTestException ngException = new TestNGTestException();
                    ngException.Class = exceptions[0].Attributes.GetNamedItem("class").Value;
                    foreach(XmlElement childNode in exceptions[0].ChildNodes)
                    {
                        if (childNode.LocalName == "message")
                        {
                            ngException.Message = childNode.InnerText;
                        }
                        else if (childNode.LocalName == "full-stacktrace")
                        {
                            ngException.StackTrace = childNode.InnerText;
                        }
                    }
                    ngReportMethod.ExecutionException = ngException;
                }

                ngMethods.Add(ngReportMethod);
            }
            return ngMethods;
        }

        private List<TestNGTestParameter> GetTestParametersFromXmlElement(XmlElement xmlElement)
        {
            List<TestNGTestParameter> ngParams = new List<TestNGTestParameter>();
            foreach (XmlElement xmlParam in xmlElement.GetElementsByTagName("parameter"))
            {
                if (xmlParam.ParentNode.Equals(xmlElement))
                {
                    TestNGTestParameter ngParam = new TestNGTestParameter
                    {
                        Name = xmlParam.Attributes.GetNamedItem("name").Value,
                        Value = xmlParam.Attributes.GetNamedItem("value").Value,
                    };
                    ngParams.Add(ngParam);
                }
            }

            return ngParams;
        }

        public void ParseTestNGReport(IGingerAction GA, bool AddFailuresToActionErrors)
        {
            GA.AddOutput("Total Test Methods", TotalTestMethodsNum);
            GA.AddOutput("Total Passed Test Methods", PassedTestMethodsNum);
            GA.AddOutput("Total Failed Test Methods", FailedTestMethodsNum);
            GA.AddOutput("Total Skipped Test Methods", SkippedTestMethodsNum);
            GA.AddOutput("Total Ignored Test Methods", IgnoredTestMethodsNum);

            foreach(TestNGTestSuite suiteReport in ReportSuites)
            {
                GA.AddOutput(string.Format("'{0}' Suite-Start Time", suiteReport.Name),suiteReport.ExecutionStartTime);
                GA.AddOutput(string.Format("'{0}' Suite-Finish Time", suiteReport.Name), suiteReport.ExecutionEndTime);
                GA.AddOutput(string.Format("'{0}' Suite-Duration (MS)", suiteReport.Name), suiteReport.ExecutionDurationMS);

                foreach (TestNGTest testReport in suiteReport.Tests)
                {
                    foreach (TestNGTestClass classReport in testReport.Classes)
                    {
                        foreach (TestNGTestMethod methodReport in classReport.Methods)
                        {
                           GA.AddOutput(string.Format("{0}\\{1}-Status", testReport.Name, methodReport.Name), methodReport.ExecutionStatus, string.Format("{0}\\{1}\\{2}", suiteReport.Name, testReport.Name, classReport.Name));
                           if (methodReport.ExecutionStatus == eTestExecutionStatus.FAIL)
                            {
                                if (methodReport.ExecutionException != null)
                                {
                                    GA.AddOutput(string.Format("{0}\\{1}-Error Message", testReport.Name, methodReport.Name), methodReport.ExecutionException.Message, string.Format("{0}\\{1}\\{2}", suiteReport.Name, testReport.Name, classReport.Name));
                                    if (AddFailuresToActionErrors)
                                    {
                                        GA.AddError(string.Format("The Test method '{0}\\{1}' failed with the error: '{2}'", testReport.Name, methodReport.Name, methodReport.ExecutionException.Message));
                                    }
                                }
                                else
                                {
                                    if (AddFailuresToActionErrors)
                                    {
                                        GA.AddError(string.Format("The Test method '{0}\\{1}' failed", testReport.Name, methodReport.Name));
                                    }
                                }                                
                            }
                        }
                    }
                }
            }
        }

        //string mSuiteName;
        //public string SuiteName
        //{
        //    get
        //    {
        //        if (mSuiteName == null && SuiteReportXml != null)
        //        {
        //            mSuiteName = SuiteReportXml.DocumentElement.GetAttribute("name").ToString();                    
        //        }
        //        return mSuiteName;
        //    }
        //}

        //Int32 mSuiteExecutionDurationMS=-1;
        //public Int32 SuiteExecutionDurationMS
        //{
        //    get
        //    {
        //        if (mSuiteExecutionDurationMS == -1 && SuiteReportXml != null)
        //        {
        //            Int32.TryParse(SuiteReportXml.DocumentElement.GetAttribute("duration-ms").ToString(), out mSuiteExecutionDurationMS);
        //        }
        //        return mSuiteExecutionDurationMS;
        //    }
        //}

        //DateTime mSuiteExecutionStartTime;
        //public DateTime SuiteExecutionStartTime
        //{
        //    get
        //    {
        //        if (mSuiteExecutionStartTime == null && SuiteReportXml != null)
        //        {
        //            DateTime.TryParse(SuiteReportXml.DocumentElement.GetAttribute("started-at").ToString(), out mSuiteExecutionStartTime);
        //        }
        //        return mSuiteExecutionStartTime;
        //    }
        //}

        //DateTime mSuiteExecutionEndTime;
        //public DateTime SuiteExecutionEndTime
        //{
        //    get
        //    {
        //        if (mSuiteExecutionEndTime == null && SuiteReportXml != null)
        //        {
        //            DateTime.TryParse(SuiteReportXml.DocumentElement.GetAttribute("finished-at").ToString(), out mSuiteExecutionStartTime);
        //        }
        //        return mSuiteExecutionEndTime;
        //    }
        //}


        //List<TestNGTest> mTests;
        //public List<TestNGTest> Tests
        //{
        //    get
        //    {
        //        if (mTests == null && SuiteReportXml != null)
        //        {
        //            foreach (XmlElement xmlTest in SuiteReportXml.GetElementsByTagName("test"))
        //            {
        //                TestNGTest ngTest = new TestNGTest();
        //                ngTest.Name = xmlTest.GetAttribute("name").ToString();
        //                Int32.TryParse(xmlTest.GetAttribute("duration-ms").ToString(), out ngTest.ExecutionDurationMS);
        //                DateTime.TryParse(xmlTest.GetAttribute("started-at").ToString(), out ngTest.ExecutionStartTime);
        //                DateTime.TryParse(xmlTest.GetAttribute("finished-at").ToString(), out ngTest.ExecutionEndTime);

        //                ngTest.Parameters = GetTestParametersFromXmlElement(xmlTest);
        //                ngTest.Classes = GetTestClassesFromXmlElement(xmlTest);

        //                mTests.Add(ngTest);
        //            }
        //        }
        //        return mTests;
        //    }
        //}

        //List<TestNGTestParameter> mParameters;
        //public List<TestNGTestParameter> Parameters
        //{
        //    get
        //    {
        //        if (mParameters == null && SuiteReportXml != null)
        //        {
        //            mParameters = GetTestParametersFromXmlElement(SuiteReportXml.DocumentElement);
        //        }
        //        return mParameters;
        //    }
        //}

        //List<string> mListeners;
        //public List<string> Listeners
        //{
        //    get
        //    {
        //        if (mListeners == null && SuiteReportXml != null)
        //        {
        //            mListeners = GetSuiteListners(SuiteReportXml.DocumentElement);
        //        }
        //        return mListeners;
        //    }
        //}

        //private List<string> GetSuiteListners(XmlElement xmlSuite)
        //{
        //    List<string> ngListners = new List<string>();
        //    foreach (XmlElement parameter in xmlSuite.GetElementsByTagName("listener"))
        //    {
        //        ngListners.Add(parameter.Attributes.GetNamedItem("class-name").Value);
        //    }

        //    return ngListners;
        //}      
    }
}
