using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using System.Xml;
using static GingerTestNgPlugin.NGProperties;

namespace GingerTestNgPlugin
{
    public class TestNGSuite
    {
        public readonly List<TestNGTest> Tests;
        public readonly List<TestNGTestParameter> Parameters;
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
            Tests = new List<TestNGTest>();
            Parameters = new List<TestNGTestParameter>();
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

                    TestNGTest Ngt = new TestNGTest
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
        private List<TestNGTestClass> GetNGClassesFromXmlElement(XmlElement test)
        {
            List<TestNGTestClass> NgParameters = new List<TestNGTestClass>();
            foreach (XmlElement NgClassXMl in test.GetElementsByTagName("class"))
            {
                TestNGTestClass NgC = new TestNGTestClass
                {
                    Name = NgClassXMl.Attributes.GetNamedItem("name").Value,
                    Parameters = GetNgParametersFromXmlElement(NgClassXMl),
                    Methods = GetNgMethods(NgClassXMl),
                };
                NgParameters.Add(NgC);
            }

            return NgParameters;
        }

        private List<TestNGTestMethod> GetNgMethods(XmlElement ngClassXMl)
        {
            List<TestNGTestMethod> NgMethods = new List<TestNGTestMethod>();

            foreach (XmlElement Xm in ngClassXMl.GetElementsByTagName("include"))
            {
                TestNGTestMethod Nmethod= new TestNGTestMethod
                {
                  Name=  Xm.Attributes.GetNamedItem("name").Value,
                };
                NgMethods.Add(Nmethod);
            }
            return NgMethods;
        }

        private List<TestNGTestParameter> GetNgParametersFromXmlElement(XmlElement test)
        {
            List<TestNGTestParameter> NgParameters = new List<TestNGTestParameter>();
            foreach (XmlElement parameter in test.GetElementsByTagName("parameter"))
            {
                if (parameter.ParentNode.Equals(test))
                {
                    TestNGTestParameter NgP = new TestNGTestParameter
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


            foreach (TestNGTest Test in Tests)
            {
                XmlElement NgTest = xml.CreateElement("test");
                NgTest.SetAttribute("name", Test.Name);

                AddParametersToXMl(Test.Parameters, NgTest, xml);




                XmlElement Classes = xml.CreateElement("classes");
                foreach (TestNGTestClass NGC in Test.Classes)
                {
                    XmlElement NgClassElement = xml.CreateElement("class");
                    NgClassElement.SetAttribute("name", NGC.Name);
                    if (NGC.Methods.Count > 0)
                    {
                        XmlElement methods = xml.CreateElement("methods");
                        foreach (TestNGTestMethod NgMethod in NGC.Methods)
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

        private void AddParametersToXMl(List<TestNGTestParameter> parameters, XmlElement Parent, XmlDocument xml)
        {
            foreach (TestNGTestParameter Parameter in parameters)
            {
                XmlElement NgParam = xml.CreateElement("parameter");
                NgParam.SetAttribute("name", Parameter.Name);
                NgParam.SetAttribute("value", Parameter.Value);
                Parent.AppendChild(NgParam);
            }
        }



        #endregion

        #region Execution
        static string DataBuffer = "";
        static string ErrorBuffer = "";
   

        static protected void AddData(string outLine)
        {
            DataBuffer += outLine;
            Console.WriteLine(outLine);
        }
      static  protected void AddError(string outLine)
        {
            Console.WriteLine(outLine);
            ErrorBuffer += outLine;
        }

        /// <summary>
        /// Run Test NG Suites with TestNG XML
        /// </summary>
        /// <param name="TestNgXML"></param>
        /// <param name="ProjectLocation"></param>
        /// <param name="LibraryFolder"></param>
        /// <param name="JavaLocation"></param>
        /// <returns></returns>
        public static TestNGReport Execute(string TestNgXML, string ProjectLocation, string LibraryFolder, string JavaLocation)
        {
            if (LibraryFolder.EndsWith("\\"))
            {
                LibraryFolder = LibraryFolder + "*";
            }
            if (!JavaLocation.EndsWith("\\")&& !string.IsNullOrEmpty(JavaLocation))
            {
                JavaLocation = JavaLocation + "\\";
            }
            else if (!LibraryFolder.EndsWith("\\*"))
            {
                LibraryFolder = LibraryFolder + "\\*";

            }
            string ProjectBin;

            string ReportFilePath;
            if (ProjectLocation.EndsWith("\\"))
            {
                ReportFilePath = ProjectLocation + @"test-output\testng-results.xml";
                ProjectBin = ProjectLocation + "bin";

            }
            else
            {
                ReportFilePath = ProjectLocation + @"\test-output\testng-results.xml";
                ProjectBin = ProjectLocation + "\\bin";

            }
            string CommandLineArguments;
            if (string.IsNullOrEmpty(JavaLocation))
            {
                CommandLineArguments = "java" + @" -cp " + LibraryFolder + ";" + ProjectBin + " org.testng.TestNG " + TestNgXML;
            }
            else
            { //TODO: Quick dirty solution to allow spliting filepath and arguments
                CommandLineArguments =JavaLocation+ "java.exe;" + @" -cp " + LibraryFolder + ";" + ProjectBin + " org.testng.TestNG " + TestNgXML;
            }
            return Execute(CommandLineArguments, ProjectLocation);
          

        }

        public static TestNGReport Execute(string FreeCommand,string WorkingDirectory,string reportSUbdirectory=null)
        {
            DateTime BeginTime = DateTime.Now;
            string ReportFilePath;
            if (WorkingDirectory.EndsWith("\\"))
            {
                ReportFilePath = string.IsNullOrEmpty(reportSUbdirectory) ? WorkingDirectory + @"test-output\testng-results.xml" : WorkingDirectory + reportSUbdirectory + @"\testng-results.xml";

            }
            else
            {
                ReportFilePath = string.IsNullOrEmpty(reportSUbdirectory) ? WorkingDirectory + @"\test-output\testng-results.xml" : WorkingDirectory + "\\"+reportSUbdirectory + @"\testng-results.xml";



            }
            ProcessStartInfo NgInfor = new ProcessStartInfo();
            NgInfor.RedirectStandardError = true;
            NgInfor.RedirectStandardOutput = true;


            NgInfor.WorkingDirectory = WorkingDirectory;
            string FilePath = FreeCommand;
            string[] FileCommand;
            //TODO: Quick dirty solution to allow spliting filepath and arguments
            char[] delimiters = { ';', };
            FileCommand = FreeCommand.Split(delimiters, 2);
            if (FileCommand.Length != 2)
            {

                char[] delimiters2 = { ' ', };
                FileCommand = FreeCommand.Split(delimiters2, 2);

            }
                FilePath = FileCommand[0];
            string Arguments = FileCommand[1];


            if (!File.Exists(FilePath))
            {
                foreach (string DirPath in Environment.GetEnvironmentVariable("PATH").Split(';'))
                {
                    string Dir = DirPath.EndsWith("\\") ? DirPath : DirPath + "\\";
                    string TempPath = Dir + FilePath;
      
                      string extension= Path.GetExtension(TempPath);
                        if (string.IsNullOrEmpty(extension))
                        {
                            if (File.Exists(TempPath + ".cmd"))
                            {
                                TempPath = TempPath + ".cmd";

                           
                        }

                           else   if (File.Exists(TempPath + ".bat"))
                                {
                                TempPath = TempPath + ".bat";

                            break;
                        }
                            else if (File.Exists(TempPath + ".exe"))
                            {
                                TempPath = TempPath + ".exe";

                        }
                        if (File.Exists(TempPath))
                        {
                            FilePath = TempPath;
                            break;
                        }
                    }

                }
            }
            NgInfor.Arguments = Arguments;




            NgInfor.FileName = FilePath;



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

            catch (Exception ex)
            {
                if ((Endtime - BeginTime).TotalMilliseconds < 0)
                {
                    throw new InvalidOperationException("TestNg Execution Failed", ex);
                }
            }



            return TestNGReport.LoadfromXMl(ReportFilePath);
        }
        #endregion

        #region ReportMethods

        public static TestNGSuite LoadFromReport(string ReportXMl)
        {

            return new TestNGSuite(ReportXMl, true);
        }

        private TestNGSuite(string XmlString, bool FromReportXMl)
        {
            Tests = new List<TestNGTest>();
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
                List<TestNGTestClass> NgClasses = new List<TestNGTestClass>();
                TestNGTest Test = new TestNGTest
                {
                    Name = NTest.GetAttribute("name").ToString(),


                    ExecutionDurationMS = Int32.Parse(NTest.GetAttribute("duration-ms").ToString()),
                    ExecutionStartTime = DateTime.Parse(NTest.GetAttribute("started-at").ToString()),
                    ExecutionEndTime = DateTime.Parse(NTest.GetAttribute("finished-at").ToString()),

                };


                foreach (XmlElement NClass in NTest.GetElementsByTagName("class"))
                {


                    TestNGTestClass TestNgClass = new TestNGTestClass
                    {
                        Name = NClass.GetAttribute("name").ToString(),
                    };
                    List<TestNGTestMethod> TestNGMethods = new List<TestNGTestMethod>();
                    foreach (XmlElement NGTestM in Suite.GetElementsByTagName("test-method"))
                    {
                        TestNGTestMethod NM = new TestNGTestMethod
                        {
                            Name = NGTestM.GetAttribute("name").ToString(),
                            ExecutionStatus = (eTestExecutionStatus)Enum.Parse(typeof(eNGStatus), NGTestM.GetAttribute("status").ToString(), true),
                            ExecutionSignature = NGTestM.GetAttribute("signature-at").ToString(),
                            ExecutionDurationMS = Int32.Parse(NGTestM.GetAttribute("duration-ms").ToString()),
                            ExecutionStartTime = DateTime.Parse(NGTestM.GetAttribute("started-at").ToString()),
                            ExecutionEndTime = DateTime.Parse(NGTestM.GetAttribute("finished-at").ToString()),
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

 
