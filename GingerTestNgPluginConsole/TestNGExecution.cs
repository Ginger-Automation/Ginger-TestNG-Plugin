using Amdocs.Ginger.Plugin.Core;
using GingerTestNgPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GingerTestNgPluginConsole
{
    public class TestNGExecution
    {
        IGingerAction mGA;

        public enum eExecutionMode { XML,Classes,Methods,Jar}

        string mTempWorkingFolder = null;
        public string TempWorkingFolder
        {
            get
            {
                if (mTempWorkingFolder == null)
                {
                    string folderName = "Ginger_TestNG_Execution_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    string path = Path.Combine(Path.GetTempPath(), folderName);
                    Directory.CreateDirectory(path);
                    mTempWorkingFolder = path;
                }
                return mTempWorkingFolder;
            }
        }

        string mJavaExeFullPath = null;
        public string JavaExeFullPath
        {
            get
            {
                return mJavaExeFullPath;
            }
            set
            {
                try
                {
                    mJavaExeFullPath = value;
                    if (string.IsNullOrEmpty(mJavaExeFullPath))
                    {
                        mJavaExeFullPath = Environment.GetEnvironmentVariable("JAVA_HOME");
                    }
                    if (string.IsNullOrEmpty(mJavaExeFullPath))
                    {
                        if (Path.GetFullPath(mJavaExeFullPath).ToLower().Contains(@"\bin\") == false)
                        {
                            mJavaExeFullPath = Path.Combine(mJavaExeFullPath, @"\bin");
                        }
                        if (Path.GetFullPath(mJavaExeFullPath).ToLower().Contains(@"\java.exe") == false)
                        {
                            mJavaExeFullPath = Path.Combine(mJavaExeFullPath, @"\java.exe");
                        }

                        mJavaExeFullPath = Path.GetFullPath(mJavaExeFullPath);
                    }
                }
                catch(Exception ex)
                {
                    //failed to init the Java Exe path
                }
            }
        }

        string mTestNGResourcesPath = null;
        public string TestNGResourcesPath
        {
            get
            {
                return mTestNGResourcesPath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    mTestNGResourcesPath = Path.GetFullPath(value);
                }
            }
        }

        string mTestNGProjectPath = null;
        public string TestNGProjectPath
        {
            get
            {
                return mTestNGProjectPath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    mTestNGProjectPath = Path.GetFullPath(value);
                }
            }
        }

        string mTestNGOutputReportFolderPath = null;
        public string TestNGOutputReportFolderPath
        {
            get
            {
                return mTestNGOutputReportFolderPath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    mTestNGOutputReportFolderPath = Path.GetFullPath(value);
                }
                else
                {
                    //add temp folder
                    mTestNGOutputReportFolderPath = TempWorkingFolder;
                }
            }
        }

        public bool ContinueExecutionOnTestFailure;

        public TestNGSuiteXML TestNgSuiteXML;

        public List<TestNGTestParameter> XmlParametersValuesToOverwrite = new List<TestNGTestParameter>();

        public List<TestNGTest> XmlTestsToExecute = new List<TestNGTest>();


        public List<TestNGTestGroup> TestGroupsToInclude = new List<TestNGTestGroup>();

        public List<TestNGTestGroup> TestGroupsToExclude = new List<TestNGTestGroup>();        

        public void SetXmlParametersValuesToOverwriteFromString(string xmlParams)
        {
            if (!string.IsNullOrEmpty(xmlParams.Trim()))
            {
                foreach (string param in xmlParams.Split(','))
                {
                    if (param.Contains('='))
                    {
                        string[] paramArgs = param.Split('=');
                        TestNGTestParameter paramObj = new TestNGTestParameter();
                        paramObj.Name = paramArgs[0].Trim(); 
                        paramObj.Value = paramArgs[1];
                        XmlParametersValuesToOverwrite.Add(paramObj);
                    }
                }
            }
        }

        public void SetXmlTestsToExecuteFromString(string xmlTests)
        {
            if (!string.IsNullOrEmpty(xmlTests.Trim()))
            {
                foreach (string test in xmlTests.Split(','))
                {
                    TestNGTest testObj = new TestNGTest();
                    testObj.Name = test;
                    XmlTestsToExecute.Add(testObj);
                }
            }
        }

        public void SetTestGroupsFromString(string groups, List<TestNGTestGroup> listToAddto)
        {
            if (!string.IsNullOrEmpty(groups.Trim()))
            {
                foreach (string group in groups.Split(','))
                {
                    TestNGTestGroup groupObj = new TestNGTestGroup();
                    groupObj.Name = group;
                    listToAddto.Add(groupObj);
                }
            }
        }


        public bool ValidateAndPrepareConfigs(IGingerAction GA, eExecutionMode execMode)        
        {
            //validate general inputes
            if (Directory.Exists(TempWorkingFolder) == false)
            {
                GA.AddError(String.Format("Failed to create temp working folder at: '{0}'", TempWorkingFolder));
                return false;
            }
            else
            {
                GA.AddExInfo(String.Format("Temp working folder path: '{0}'", TempWorkingFolder));
            }

            if (Path.GetFileName(JavaExeFullPath).ToLower() != "java.exe" || File.Exists(JavaExeFullPath) == false)
            {
                GA.AddError(String.Format("Failed to find 'java.exe' at: '{0}'", JavaExeFullPath));
                return false;
            }
            else
            {
                GA.AddExInfo(String.Format("'java.exe' path: '{0}'", JavaExeFullPath));
            }
            
            if (Directory.Exists(TestNGResourcesPath.Trim().TrimEnd('*')) == false)
            {
                GA.AddError(String.Format("Failed to find the TestNG resources folder at: '{0}'", TestNGResourcesPath));
                return false;
            }
            else
            {
                if (TestNGResourcesPath.Contains('*') == false)
                {
                    TestNGResourcesPath = Path.Combine(TestNGResourcesPath, "*");
                }
                GA.AddExInfo(String.Format("TestNG resources path: '{0}'", TestNGResourcesPath));
            }

            if (Directory.Exists(TestNGProjectPath.Trim().TrimEnd('*')) == false)
            {
                GA.AddError(String.Format("Failed to find the TestNG testing project folder at: '{0}'", TestNGProjectPath));
                return false;
            }
            else
            {
                if (TestNGProjectPath.Contains('*') == false)
                {
                    TestNGProjectPath = Path.Combine(TestNGProjectPath, "*");
                }
                GA.AddExInfo(String.Format("TestNG testing project path: '{0}'", TestNGProjectPath));
            }

            if (Directory.Exists(TestNGOutputReportFolderPath) == false)
            {
                GA.AddError(String.Format("Failed to find the TestNG output report root folder at: '{0}'", TestNGOutputReportFolderPath));
                return false;
            }
            else
            {
                GA.AddExInfo(String.Format("TestNG output report root folder path: '{0}'", TestNGOutputReportFolderPath));
            }


            switch (execMode)
            {
                case eExecutionMode.XML:                    
                    if (TestNgSuiteXML.LoadError != null)
                    {
                        GA.AddError(TestNgSuiteXML.LoadError);
                        return false;
                    }                    
                    else
                    {
                        GA.AddExInfo(String.Format("TestNG XML path: '{0}'", TestNgSuiteXML.XmlFilePath));
                    }

                    string suiteXmlString = TestNgSuiteXML.SuiteXmlString;

                    if (XmlParametersValuesToOverwrite.Count>0)
                    {
                        string paramsListStr= "Parameters to overwrite: ";
                        foreach(TestNGTestParameter param in XmlParametersValuesToOverwrite)
                        {
                            param.Name = param.Name.Trim();                           
                            if (!TestNgSuiteXML.IsParameterExistInXML(param.Name))
                            {
                                GA.AddError(string.Format("The Parameter '{0}' do not exist in the TestNG Suite XML", param.Name));
                                return false;
                            }
                            else
                            {
                                paramsListStr += string.Format("'{0}'='{1}', ", param.Name, param.Value);
                            }
                        }
                        paramsListStr.TrimEnd(',');
                        GA.AddExInfo(paramsListStr);
                    }

                    if (XmlTestsToExecute.Count > 0)
                    {
                        string testsListStr = "Tests to execute: ";
                        foreach (TestNGTest test in XmlTestsToExecute)
                        {
                            test.Name = test.Name.Trim();
                            if (!TestNgSuiteXML.IsTestExistInXML(test.Name))
                            {
                                GA.AddError(string.Format("The Test '{0}' do not exist in the TestNG Suite XML", test.Name));
                                return false;
                            }
                            else
                            {
                                testsListStr += string.Format("'{0}', ", test.Name);
                            }
                        }
                        testsListStr.TrimEnd(',');
                        GA.AddExInfo(testsListStr);
                    }
                    break;
                case eExecutionMode.Classes:
                    break;
                case eExecutionMode.Methods:
                    break;
                case eExecutionMode.Jar:
                    break;
            }

            if (TestGroupsToInclude.Count > 0)
            {
                string groupsToIncludeListStr = "Tests Groups to include: ";
                foreach (TestNGTestGroup group in TestGroupsToInclude)
                {
                    groupsToIncludeListStr += string.Format("'{0}', ", group.Name);
                }
                groupsToIncludeListStr.TrimEnd(',');
                GA.AddExInfo(groupsToIncludeListStr);
            }
            if (TestGroupsToExclude.Count > 0)
            {
                string groupsToExcludeListStr = "Tests Groups to exclude: ";
                foreach (TestNGTestGroup group in TestGroupsToExclude)
                {
                    groupsToExcludeListStr += string.Format("'{0}', ", group.Name);
                }
                groupsToExcludeListStr.TrimEnd(',');
                GA.AddExInfo(groupsToExcludeListStr);
            }

            return true;
        }

        public bool Execute(IGingerAction GA, eExecutionMode execMode)
        {
            mGA = GA;

            return true;
        }

        ////        //Execution
        //static string DataBuffer = "";
        //static string ErrorBuffer = "";


        //static protected void AddData(string outLine)
        //{
        //    DataBuffer += outLine;
        //    Console.WriteLine(outLine);
        //}
        //static protected void AddError(string outLine)
        //{
        //    Console.WriteLine(outLine);
        //    ErrorBuffer += outLine;
        //}

        ///// <summary>
        ///// Run Test NG Suites with TestNG XML
        ///// </summary>
        ///// <param name="TestNgXML"></param>
        ///// <param name="ProjectLocation"></param>
        ///// <param name="LibraryFolder"></param>
        ///// <param name="JavaLocation"></param>
        ///// <returns></returns>
        //public static TestNGReport Execute(string TestNgXML, string ProjectLocation, string LibraryFolder, string JavaLocation)
        //{
        //    if (LibraryFolder.EndsWith("\\"))
        //    {
        //        LibraryFolder = LibraryFolder + "*";
        //    }
        //    if (!JavaLocation.EndsWith("\\") && !string.IsNullOrEmpty(JavaLocation))
        //    {
        //        JavaLocation = JavaLocation + "\\";
        //    }
        //    else if (!LibraryFolder.EndsWith("\\*"))
        //    {
        //        LibraryFolder = LibraryFolder + "\\*";

        //    }
        //    string ProjectBin;

        //    string ReportFilePath;
        //    if (ProjectLocation.EndsWith("\\"))
        //    {
        //        ReportFilePath = ProjectLocation + @"test-output\testng-results.xml";
        //        ProjectBin = ProjectLocation + "bin";

        //    }
        //    else
        //    {
        //        ReportFilePath = ProjectLocation + @"\test-output\testng-results.xml";
        //        ProjectBin = ProjectLocation + "\\bin";

        //    }
        //    string CommandLineArguments;
        //    if (string.IsNullOrEmpty(JavaLocation))
        //    {
        //        CommandLineArguments = "java" + @" -cp " + LibraryFolder + ";" + ProjectBin + " org.testng.TestNG " + TestNgXML;
        //    }
        //    else
        //    { //TODO: Quick dirty solution to allow spliting filepath and arguments
        //        CommandLineArguments = JavaLocation + "java.exe;" + @" -cp " + LibraryFolder + ";" + ProjectBin + " org.testng.TestNG " + TestNgXML;
        //    }
        //    return Execute(CommandLineArguments, ProjectLocation);


        //}

        //public static TestNGReport Execute(string FreeCommand, string WorkingDirectory, string reportSUbdirectory = null)
        //{
        //    DateTime BeginTime = DateTime.Now;
        //    string ReportFilePath;
        //    if (WorkingDirectory.EndsWith("\\"))
        //    {
        //        ReportFilePath = string.IsNullOrEmpty(reportSUbdirectory) ? WorkingDirectory + @"test-output\testng-results.xml" : WorkingDirectory + reportSUbdirectory + @"\testng-results.xml";

        //    }
        //    else
        //    {
        //        ReportFilePath = string.IsNullOrEmpty(reportSUbdirectory) ? WorkingDirectory + @"\test-output\testng-results.xml" : WorkingDirectory + "\\" + reportSUbdirectory + @"\testng-results.xml";



        //    }
        //    ProcessStartInfo NgInfor = new ProcessStartInfo();
        //    NgInfor.RedirectStandardError = true;
        //    NgInfor.RedirectStandardOutput = true;


        //    NgInfor.WorkingDirectory = WorkingDirectory;
        //    string FilePath = FreeCommand;
        //    string[] FileCommand;
        //    //TODO: Quick dirty solution to allow spliting filepath and arguments
        //    char[] delimiters = { ';', };
        //    FileCommand = FreeCommand.Split(delimiters, 2);
        //    if (FileCommand.Length != 2)
        //    {

        //        char[] delimiters2 = { ' ', };
        //        FileCommand = FreeCommand.Split(delimiters2, 2);

        //    }
        //    FilePath = FileCommand[0];
        //    string Arguments = FileCommand[1];


        //    if (!File.Exists(FilePath))
        //    {
        //        foreach (string DirPath in Environment.GetEnvironmentVariable("PATH").Split(';'))
        //        {
        //            string Dir = DirPath.EndsWith("\\") ? DirPath : DirPath + "\\";
        //            string TempPath = Dir + FilePath;

        //            string extension = Path.GetExtension(TempPath);
        //            if (string.IsNullOrEmpty(extension))
        //            {
        //                if (File.Exists(TempPath + ".cmd"))
        //                {
        //                    TempPath = TempPath + ".cmd";


        //                }

        //                else if (File.Exists(TempPath + ".bat"))
        //                {
        //                    TempPath = TempPath + ".bat";

        //                    break;
        //                }
        //                else if (File.Exists(TempPath + ".exe"))
        //                {
        //                    TempPath = TempPath + ".exe";

        //                }
        //                if (File.Exists(TempPath))
        //                {
        //                    FilePath = TempPath;
        //                    break;
        //                }
        //            }

        //        }
        //    }
        //    NgInfor.Arguments = Arguments;




        //    NgInfor.FileName = FilePath;



        //    Process TestNgProcess = new Process();
        //    TestNgProcess.OutputDataReceived += (proc, outLine) => { AddData(outLine.Data + "\n"); };
        //    TestNgProcess.ErrorDataReceived += (proc, outLine) => { AddError(outLine.Data + "\n"); };
        //    TestNgProcess.StartInfo = NgInfor;
        //    TestNgProcess.Start();
        //    TestNgProcess.BeginOutputReadLine();
        //    TestNgProcess.BeginErrorReadLine();
        //    TestNgProcess.WaitForExit();
        //    DateTime Endtime = File.GetLastWriteTime(ReportFilePath);




        //    try
        //    {


        //        if ((Endtime - BeginTime).TotalMilliseconds < 0)
        //        {
        //            throw new InvalidOperationException("TestNg Execution Failed");
        //        }


        //    }

        //    catch (Exception ex)
        //    {
        //        if ((Endtime - BeginTime).TotalMilliseconds < 0)
        //        {
        //            throw new InvalidOperationException("TestNg Execution Failed", ex);
        //        }
        //    }



        //    return TestNGReport.LoadfromXMl(ReportFilePath);
        //}
        ////#endregion

    }
}
