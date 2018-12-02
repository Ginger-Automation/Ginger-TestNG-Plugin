using GingerTestNgPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GingerTestNgPluginConsole
{
    public class TestNGReportXML
    {
        public string ReportXmlFilePath;
        XmlDocument SuiteReportXml;
        public TestNGTestSuite SuiteReportObject;
        public string LoadError = null;

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
                if (File.Exists(ReportXmlFilePath) == false)
                {
                    LoadError = String.Format("Failed to find the TestNG Report XML file at: '{0}'", ReportXmlFilePath);
                    return false;
                }

                SuiteReportXml = new XmlDocument();
                SuiteReportXml.LoadXml(System.IO.File.ReadAllText(ReportXmlFilePath));

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
                if (SuiteReportXml == null)
                {
                    return false;
                }

                SuiteReportObject = new TestNGTestSuite();
                SuiteReportObject.Name = SuiteReportXml.DocumentElement.GetAttribute("name").ToString();
                SuiteReportObject.Parameters = GetTestParametersFromXmlElement(SuiteReportXml.DocumentElement);                
                DateTime.TryParse(SuiteReportXml.DocumentElement.GetAttribute("started-at").ToString(), out SuiteReportObject.ExecutionStartTime);
                DateTime.TryParse(SuiteReportXml.DocumentElement.GetAttribute("finished-at").ToString(), out SuiteReportObject.ExecutionStartTime);
                Int32.TryParse(SuiteReportXml.DocumentElement.GetAttribute("duration-ms").ToString(), out SuiteReportObject.ExecutionDurationMS);

                foreach (XmlElement xmlReportTest in SuiteReportXml.GetElementsByTagName("test"))
                {
                    TestNGTest ngTest = new TestNGTest();
                    ngTest.Name = xmlReportTest.GetAttribute("name").ToString();                    
                    DateTime.TryParse(xmlReportTest.GetAttribute("started-at").ToString(), out ngTest.ExecutionStartTime);
                    DateTime.TryParse(xmlReportTest.GetAttribute("finished-at").ToString(), out ngTest.ExecutionEndTime);
                    Int32.TryParse(xmlReportTest.GetAttribute("duration-ms").ToString(), out ngTest.ExecutionDurationMS);
                    ngTest.Parameters = GetTestParametersFromXmlElement(xmlReportTest);
                    ngTest.Classes = GetTestClassesFromXmlElement(xmlReportTest);
                    SuiteReportObject.Tests.Add(ngTest);
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

            foreach (XmlElement xmlMethod in xmlClass.GetElementsByTagName("include"))
            {
                TestNGTestMethod ngMethod = new TestNGTestMethod
                {
                    Name = xmlMethod.Attributes.GetNamedItem("name").Value,
                };
                ngMethods.Add(ngMethod);
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
