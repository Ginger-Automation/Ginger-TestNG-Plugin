﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using static GingerTestNgPlugin.NGProperties;

namespace GingerTestNgPlugin
{
    public class TestNGSuite
    {
        public readonly List<NGTest> Tests;
        public readonly List<NGParameter> Parameters;
        public readonly List<string> Listeners;
        public readonly string Name;
        public readonly bool IsCreatedFromReport;

        #region reportParams
        public readonly int Duration;
        public readonly DateTime StartedAt;
        public readonly DateTime FinishedAt;
        private XmlDocumentType DocumentType;
        #endregion

        ///<summary>
        /// Generate TestNgSuite Object From XML String of TestNgFile
        ///</summary>
        public TestNGSuite(string XmlString)
        {
            Tests = new List<NGTest>();
            Parameters = new List<NGParameter>();
            XmlDocument TestNgXml = new XmlDocument();
            TestNgXml.LoadXml(XmlString);
            if (TestNgXml.DocumentType.SystemId.Contains(@"http://testng.org/testng-1.0.dtd"))
            {
                DocumentType = TestNgXml.DocumentType;

                Name = TestNgXml.DocumentElement.GetAttribute("name").ToString();
                Parameters = GetNgParametersFromXmlElement(TestNgXml.DocumentElement);


                Listeners = GetListners(TestNgXml.DocumentElement);


                foreach (XmlElement Test in TestNgXml.GetElementsByTagName("test"))
                {

                    NGTest Ngt = new NGTest
                    {
                        Name = Test.Attributes.GetNamedItem("name").Value,
                        Parameters = GetNgParametersFromXmlElement(Test),
                        Classes = GetNGClassesFromXmlElement(Test),
                    };
                    Tests.Add(Ngt);
                }
            }
            else
            {
                throw new NotSupportedException("Not a valid/supported TestNg File. Please provide a valid TestNG XML File");
            }
        }
        #region Parsing
        private List<NGClass> GetNGClassesFromXmlElement(XmlElement test)
        {
            List<NGClass> NgParameters = new List<NGClass>();
            foreach (XmlElement NgClassXMl in test.GetElementsByTagName("class"))
            {
                NGClass NgC = new NGClass
                {
                    Name = NgClassXMl.Attributes.GetNamedItem("name").Value,
                    Parameters = GetNgParametersFromXmlElement(NgClassXMl),
                    Methods = GetNgMethods(NgClassXMl),
                };
                NgParameters.Add(NgC);
            }

            return NgParameters;
        }

        private List<NGMethod> GetNgMethods(XmlElement ngClassXMl)
        {
            List<NGMethod> NgMethods = new List<NGMethod>();

            foreach (XmlElement Xm in ngClassXMl.GetElementsByTagName("include"))
            {
                NGMethod Nmethod= new NGMethod
                {
                  Name=  Xm.Attributes.GetNamedItem("name").Value,
                };
                NgMethods.Add(Nmethod);
            }
            return NgMethods;
        }

        private List<NGParameter> GetNgParametersFromXmlElement(XmlElement test)
        {
            List<NGParameter> NgParameters = new List<NGParameter>();
            foreach (XmlElement parameter in test.GetElementsByTagName("parameter"))
            {
                if (parameter.ParentNode.Equals(test))
                {
                    NGParameter NgP = new NGParameter
                    {
                        Name = parameter.Attributes.GetNamedItem("name").Value,
                        Value = parameter.Attributes.GetNamedItem("value").Value,
                    };
                    NgParameters.Add(NgP);
                }
            }

            return NgParameters;
        }

        private List<string> GetListners(XmlElement test)
        {
            List<string> listeners = new List<string>();
            foreach (XmlElement parameter in test.GetElementsByTagName("listener"))
            {

                listeners.Add(parameter.Attributes.GetNamedItem("class-name").Value);

            }

            return listeners;
        }

        #endregion


        #region GenerateXml

        public string GetTextNGXml()

        {

            if (IsCreatedFromReport)
            {
                throw new NotSupportedException("TestNg XML cannot be created from Report file");
            }

            XmlDocument xml = new XmlDocument();
            XmlNode doctype = xml.ImportNode(DocumentType, false);
            xml.AppendChild(doctype);

            XmlElement root = xml.CreateElement("suite");
            root.SetAttribute("name", Name);

            xml.AppendChild(root);
            AddParametersToXMl(Parameters, root, xml);

            if (Listeners.Count > 0)
            {
                XmlElement listenersXml = xml.CreateElement("listeners");
                foreach (string listener in Listeners)
                {
                    XmlElement listenerXml = xml.CreateElement("listener");
                    listenerXml.SetAttribute("class-name", listener);
                    listenersXml.AppendChild(listenerXml);
                }
                root.AppendChild(listenersXml);

            }


            foreach (NGTest Test in Tests)
            {
                XmlElement NgTest = xml.CreateElement("test");
                NgTest.SetAttribute("name", Test.Name);

                AddParametersToXMl(Test.Parameters, NgTest, xml);




                XmlElement Classes = xml.CreateElement("classes");
                foreach (NGClass NGC in Test.Classes)
                {
                    XmlElement NgClassElement = xml.CreateElement("class");
                    NgClassElement.SetAttribute("name", NGC.Name);
                    if (NGC.Methods.Count > 0)
                    {
                        XmlElement methods = xml.CreateElement("methods");
                        foreach (NGMethod NgMethod in NGC.Methods)
                        {
                            XmlElement method = xml.CreateElement("include");
                            method.SetAttribute("name", NgMethod.Name);
                            methods.AppendChild(method);
                        }

                        AddParametersToXMl(NGC.Parameters, NgClassElement, xml);
                        NgClassElement.AppendChild(methods);
                    }

                    Classes.AppendChild(NgClassElement);
                }
                NgTest.AppendChild(Classes);
                root.AppendChild(NgTest);
            }



            return xml.OuterXml;
        }

        private void AddParametersToXMl(List<NGParameter> parameters, XmlElement Parent, XmlDocument xml)
        {
            foreach (NGParameter Parameter in parameters)
            {
                XmlElement NgParam = xml.CreateElement("parameter");
                NgParam.SetAttribute("name", Parameter.Name);
                NgParam.SetAttribute("value", Parameter.Value);
                Parent.AppendChild(NgParam);
            }
        }



        #endregion

        #region Execution
        string DataBuffer = "";
        string ErrorBuffer = "";
        public TestNGReport Execute(string TestNgXML, string ProjectLocation, string LibraryFolder, string JavaLocation)
        {

            DateTime BeginTime = DateTime.Now;
            string ReportFilePath;
            ProcessStartInfo NgInfor = new ProcessStartInfo();
            NgInfor.WorkingDirectory = @"C:\Users\mohdkhan\eclipse-workspace\SeleniumTestNg";

            NgInfor.RedirectStandardError = true;
            NgInfor.RedirectStandardOutput = true;


            if (LibraryFolder.EndsWith("\\"))
            {
                LibraryFolder = LibraryFolder + "*";
            }
            else if(!LibraryFolder.EndsWith("\\*"))
            {
                LibraryFolder = LibraryFolder + "\\*";

            }

            //report file needed to be set first
            if(ProjectLocation.EndsWith("\\"))
            {
                ReportFilePath = ProjectLocation + @"test-output\testng-results.xml";
                ProjectLocation = ProjectLocation + "bin";

            }
            else
            {
                ReportFilePath = ProjectLocation + @"\test-output\testng-results.xml";
                ProjectLocation = ProjectLocation + "\\bin";
  
            }


            

            NgInfor.Arguments = @"-cp " + LibraryFolder + ";" + ProjectLocation + " org.testng.TestNG " + TestNgXML;

            NgInfor.FileName = System.Environment.GetEnvironmentVariable("JAVA_HOME") + @"\bin\java.exe";

            Process TestNgProcess = new Process();

            TestNgProcess.OutputDataReceived += (proc, outLine) => { AddData(outLine.Data + "\n"); };
            TestNgProcess.ErrorDataReceived += (proc, outLine) => { AddError(outLine.Data + "\n"); };
            TestNgProcess.StartInfo = NgInfor;
            TestNgProcess.Start();
            TestNgProcess.BeginOutputReadLine();
            TestNgProcess.BeginErrorReadLine();
            TestNgProcess.WaitForExit();
            DateTime Endtime = File.GetLastWriteTime(ReportFilePath);
            try
            {
             

                if ((Endtime - BeginTime).TotalMilliseconds < 0)
                {
                    throw new InvalidOperationException("TestNg Execution Failed");
                }
               

            }

            catch(Exception ex)
            {
                if ((Endtime - BeginTime).TotalMilliseconds < 0)
                {
                    throw new InvalidOperationException("TestNg Execution Failed",ex);
                }
            }
            return TestNGReport.LoadfromXMl(ReportFilePath);

        }

        protected void AddData(string outLine)
        {
            DataBuffer += outLine;
        }
        protected void AddError(string outLine)
        {
            ErrorBuffer += outLine;
        }

        #endregion

        #region ReportMethods

        public static TestNGSuite LoadFromReport(string ReportXMl)
        {

            return new TestNGSuite(ReportXMl, true);
        }

        private TestNGSuite(string XmlString, bool FromReportXMl)
        {
            Tests = new List<NGTest>();
            if (!FromReportXMl)
            {
                throw new NotSupportedException("Please use other constructor");
            }
            IsCreatedFromReport = true;
            XmlDocument Suite = new XmlDocument();
            Suite.LoadXml(XmlString);

            Name = Suite.DocumentElement.GetAttribute("name").ToString();
            Duration = Int32.Parse(Suite.DocumentElement.GetAttribute("duration-ms").ToString());
            StartedAt = DateTime.Parse(Suite.DocumentElement.GetAttribute("started-at").ToString());
            FinishedAt = DateTime.Parse(Suite.DocumentElement.GetAttribute("finished-at").ToString());
            foreach (XmlElement NTest in Suite.GetElementsByTagName("test"))
            {
                List<NGClass> NgClasses = new List<NGClass>();
                NGTest Test = new NGTest
                {
                    Name = NTest.GetAttribute("name").ToString(),


                    Duration = Int32.Parse(NTest.GetAttribute("duration-ms").ToString()),
                    StartedAt = DateTime.Parse(NTest.GetAttribute("started-at").ToString()),
                    FinishedAt = DateTime.Parse(NTest.GetAttribute("finished-at").ToString()),

                };


                foreach (XmlElement NClass in NTest.GetElementsByTagName("class"))
                {


                    NGClass TestNgClass = new NGClass
                    {
                        Name = NClass.GetAttribute("name").ToString(),
                    };
                    List<NGMethod> TestNGMethods = new List<NGMethod>();
                    foreach (XmlElement NGTestM in Suite.GetElementsByTagName("test-method"))
                    {
                        NGMethod NM = new NGMethod
                        {
                            Name = NGTestM.GetAttribute("name").ToString(),
                            Status = (eNGStatus)Enum.Parse(typeof(eNGStatus), NGTestM.GetAttribute("status").ToString(), true),
                            Signature = NGTestM.GetAttribute("signature-at").ToString(),
                            Duration = Int32.Parse(NGTestM.GetAttribute("duration-ms").ToString()),
                            StartedAt = DateTime.Parse(NGTestM.GetAttribute("started-at").ToString()),
                            FinishedAt = DateTime.Parse(NGTestM.GetAttribute("finished-at").ToString()),
                        };
                        TestNGMethods.Add(NM);
                    }
                    TestNgClass.Methods = TestNGMethods;
                    NgClasses.Add(TestNgClass);



                }

                Test.Classes = NgClasses;
                Tests.Add(Test);
            }
        }




















        #endregion
    }
}

 
