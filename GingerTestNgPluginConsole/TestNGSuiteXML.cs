using GingerTestNgPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace GingerTestNgPluginConsole
{
    public class TestNGSuiteXML
    {
        public string XmlFilePath;
        public XmlDocument SuiteXml;
        public TestNGTestSuite SuiteObject;
        public string LoadError = null;


        string mXmlDocumentType;
        public string XmlDocumentType
        {
            get
            {
                if (mXmlDocumentType == null && SuiteXml != null)
                {
                    mXmlDocumentType = SuiteXml.DocumentType.SystemId;
                }
                return mXmlDocumentType;
            }
        }

        public string SuiteXmlString
        {
            get
            {
                if (SuiteXml != null)
                {
                    StringWriter sw = new StringWriter();
                    XmlTextWriter xw = new XmlTextWriter(sw);
                    SuiteXml.WriteTo(xw);
                    return sw.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        ///<summary>
        /// Generate TestNGSuiteXML Object from TestNg Suite XML file
        ///</summary>
        public TestNGSuiteXML(string xmlFilePath)
        {
            XmlFilePath = xmlFilePath;
            if (LoadSuiteXmlFromFile())
            {
                LoadSuiteObjectFromXml();
            }
        }

        private bool LoadSuiteXmlFromFile()
        {
            try
            {
                XmlFilePath = Path.GetFullPath(XmlFilePath);
                if (File.Exists(XmlFilePath) == false)
                {
                    LoadError = String.Format("Failed to find the TestNG XML file at: '{0}'", XmlFilePath);
                    return false;
                }

                SuiteXml = new XmlDocument();
                SuiteXml.LoadXml(System.IO.File.ReadAllText(XmlFilePath));

                if (XmlDocumentType.Contains(@"http://testng.org/testng-1.0.dtd") == false)
                {
                    LoadError = String.Format("Failed to load the TestNG XML because it type is not '{0}'", @"http://testng.org/testng-1.0.dtd");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LoadError = string.Format("Failed to load the TestNG Suite XML from path: '{0}' due to the Error: '{1}'", XmlFilePath, ex.Message);
                return false;
            }
        }

        private bool LoadSuiteObjectFromXml()
        {
            try
            {
                if (SuiteXml == null)
                {
                    return false;
                }

                SuiteObject = new TestNGTestSuite();
                SuiteObject.Name = SuiteXml.DocumentElement.GetAttribute("name").ToString();
                SuiteObject.Parameters = GetTestParametersFromXmlElement(SuiteXml.DocumentElement);
                SuiteObject.Listeners = GetSuiteListners(SuiteXml.DocumentElement);

                SuiteObject.Tests = new List<TestNGTest>();
                foreach (XmlElement xmlTest in SuiteXml.GetElementsByTagName("test"))
                {
                    TestNGTest ngTest = new TestNGTest
                    {
                        Name = xmlTest.Attributes.GetNamedItem("name").Value,
                        Parameters = GetTestParametersFromXmlElement(xmlTest),
                        Classes = GetTestClassesFromXmlElement(xmlTest),
                    };
                    SuiteObject.Tests.Add(ngTest);
                }

                return true;
            }
            catch (Exception ex)
            {
                LoadError = string.Format("Failed to load the TestNG Suite Object from XML due to the Error: '{0}'", ex.Message);
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
                    Methods = GetClassMethods(xmlClass),
                };
                ngClasses.Add(ngClass);
            }

            return ngClasses;
        }

        private List<TestNGTestMethod> GetClassMethods(XmlElement xmlClass)
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

        private List<TestNGListener> GetSuiteListners(XmlElement xmlSuite)
        {
            List<TestNGListener> ngListners = new List<TestNGListener>();
            foreach (XmlElement xmlListener in xmlSuite.GetElementsByTagName("listener"))
            {
                TestNGListener ngListener = new TestNGListener
                {
                    ClassName = xmlListener.Attributes.GetNamedItem("class-name").Value

                };
                ngListners.Add(ngListener);
            }

            return ngListners;
        }

        public bool IsParameterExistInXML(string parameterName)
        {
            XmlNodeList paramsList = SuiteXml.GetElementsByTagName("parameter");
            for (int i = 0; i < paramsList.Count; i++)
            {
               if (paramsList[i].Attributes.GetNamedItem("name").Value == parameterName)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsTestExistInXML(string testName)
        {
            XmlNodeList testsList = SuiteXml.GetElementsByTagName("test");
            for (int i = 0; i < testsList.Count; i++)
            {
                if (testsList[i].Attributes.GetNamedItem("name").Value == testName)
                {
                    return true;
                }
            }

            return false;
        }


        public void OverrideXMLParameters(List<TestNGTestParameter> xmlParametersToOverwrite)
        {
            XmlNodeList xmlParamsList = SuiteXml.GetElementsByTagName("parameter");
            for (int i = 0; i < xmlParamsList.Count; i++)
            {
                TestNGTestParameter paramToOveride = xmlParametersToOverwrite.Where(x => x.Name.Trim() == xmlParamsList[i].Attributes.GetNamedItem("name").Value).FirstOrDefault();
                if (paramToOveride != null)
                {
                    xmlParamsList[i].Attributes.GetNamedItem("value").Value = paramToOveride.Value;
                }
            }
        }

        //string mSuiteName;
        //public string SuiteName
        //{
        //    get
        //    {
        //        if (mSuiteName == null && SuiteXml != null)
        //        {
        //            mSuiteName = SuiteXml.DocumentElement.GetAttribute("name").ToString();
        //        }
        //        return mSuiteName;
        //    }
        //}

        //List<TestNGTest> mTests;
        //public List<TestNGTest> Tests
        //{
        //    get
        //    {
        //        if (mTests == null && SuiteXml != null)
        //        {
        //            foreach (XmlElement xmlTest in SuiteXml.GetElementsByTagName("test"))
        //            {
        //                TestNGTest ngTest = new TestNGTest
        //                {
        //                    Name = xmlTest.Attributes.GetNamedItem("name").Value,
        //                    Parameters = GetTestParametersFromXmlElement(xmlTest),
        //                    Classes = GetTestClassesFromXmlElement(xmlTest),
        //                };
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
        //        if (mParameters == null && SuiteXml != null)
        //        {
        //            mParameters = GetTestParametersFromXmlElement(SuiteXml.DocumentElement);
        //        }
        //        return mParameters;
        //    }
        //}

        //List<TestNGListener> mListeners;
        //public List<TestNGListener> Listeners
        //{
        //    get
        //    {
        //        if (mListeners == null && SuiteXml != null)
        //        {
        //            mListeners = GetSuiteListners(SuiteXml.DocumentElement);
        //        }
        //        return mListeners;
        //    }
        //}

        //public string BuilTestNGSuiteXml()
        //{
        //    XmlDocument xml = new XmlDocument();
        //    XmlNode doctype = xml.ImportNode(DocumentType, false);
        //    xml.AppendChild(doctype);

        //    XmlElement root = xml.CreateElement("suite");
        //    root.SetAttribute("name", SuiteName);

        //    xml.AppendChild(root);
        //    AddParametersToXMl(Parameters, root, xml);

        //    if (Listeners.Count > 0)
        //    {
        //        XmlElement listenersXml = xml.CreateElement("listeners");
        //        foreach (string listener in Listeners)
        //        {
        //            XmlElement listenerXml = xml.CreateElement("listener");
        //            listenerXml.SetAttribute("class-name", listener);
        //            listenersXml.AppendChild(listenerXml);
        //        }
        //        root.AppendChild(listenersXml);
        //    }

        //    foreach (TestNGTest Test in Tests)
        //    {
        //        XmlElement NgTest = xml.CreateElement("test");
        //        NgTest.SetAttribute("name", Test.Name);

        //        AddParametersToXMl(Test.Parameters, NgTest, xml);

        //        XmlElement Classes = xml.CreateElement("classes");
        //        foreach (TestNGTestClass NGC in Test.Classes)
        //        {
        //            XmlElement NgClassElement = xml.CreateElement("class");
        //            NgClassElement.SetAttribute("name", NGC.Name);
        //            if (NGC.Methods.Count > 0)
        //            {
        //                XmlElement methods = xml.CreateElement("methods");
        //                foreach (TestNGTestMethod NgMethod in NGC.Methods)
        //                {
        //                    XmlElement method = xml.CreateElement("include");
        //                    method.SetAttribute("name", NgMethod.Name);
        //                    methods.AppendChild(method);
        //                }

        //                AddParametersToXMl(NGC.Parameters, NgClassElement, xml);
        //                NgClassElement.AppendChild(methods);
        //            }
        //            Classes.AppendChild(NgClassElement);
        //        }
        //        NgTest.AppendChild(Classes);
        //        root.AppendChild(NgTest);
        //    }

        //    return xml.OuterXml;
        //}

        //private void AddParametersToXMl(List<TestNGTestParameter> parameters, XmlElement Parent, XmlDocument xml)
        //{
        //    foreach (TestNGTestParameter Parameter in parameters)
        //    {
        //        XmlElement NgParam = xml.CreateElement("parameter");
        //        NgParam.SetAttribute("name", Parameter.Name);
        //        NgParam.SetAttribute("value", Parameter.Value);
        //        Parent.AppendChild(NgParam);
        //    }
        //}

        
            //static void Main(string[] args)
            //{
            //    XmlDocument doc = new XmlDocument();

            //    //(1) the xml declaration is recommended, but not mandatory
            //    XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            //    XmlElement root = doc.DocumentElement;
            //    doc.InsertBefore(xmlDeclaration, root);

            //    //(2) string.Empty makes cleaner code
            //    XmlElement element1 = doc.CreateElement(string.Empty, "body", string.Empty);
            //    doc.AppendChild(element1);

            //    XmlElement element2 = doc.CreateElement(string.Empty, "level1", string.Empty);
            //    element1.AppendChild(element2);

            //    XmlElement element3 = doc.CreateElement(string.Empty, "level2", string.Empty);
            //    XmlText text1 = doc.CreateTextNode("text");
            //    element3.AppendChild(text1);
            //    element2.AppendChild(element3);

            //    XmlElement element4 = doc.CreateElement(string.Empty, "level2", string.Empty);
            //    XmlText text2 = doc.CreateTextNode("other text");
            //    element4.AppendChild(text2);
            //    element2.AppendChild(element4);

            //    doc.Save("D:\\document.xml");
            //}
        

    }
}
