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
        public IGingerAction GingerAction;

        static string mCommandOutputBuffer = string.Empty;
        static string mCommandOutputErrorBuffer = string.Empty;

        public enum eExecutionMode { XML, Classes, Methods, Jar }
        public eExecutionMode ExecutionMode;

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
                catch (Exception ex)
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

        string mTestNGProjectBinFolderPath = null;
        public string TestNGProjectBinFolderPath
        {
            get
            {
                return mTestNGProjectBinFolderPath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    mTestNGProjectBinFolderPath = Path.GetFullPath(value);
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


        public bool ValidateAndPrepareConfigs()
        {
            //validate general inputes
            if (Directory.Exists(TempWorkingFolder) == false)
            {
                GingerAction.AddError(String.Format("Failed to create temp working folder at: '{0}'", TempWorkingFolder));
                return false;
            }
            else
            {
                GingerAction.AddExInfo(String.Format("Temp working folder path: '{0}'", TempWorkingFolder));
            }

            if (Path.GetFileName(JavaExeFullPath).ToLower() != "java.exe" || File.Exists(JavaExeFullPath) == false)
            {
                GingerAction.AddError(String.Format("Failed to find 'java.exe' at: '{0}'", JavaExeFullPath));
                return false;
            }
            else
            {
                GingerAction.AddExInfo(String.Format("'java.exe' path: '{0}'", JavaExeFullPath));
            }

            if (Directory.Exists(TestNGResourcesPath.Trim().TrimEnd('*')) == false)
            {
                GingerAction.AddError(String.Format("Failed to find the TestNG resources folder at: '{0}'", TestNGResourcesPath));
                return false;
            }
            else
            {
                if (TestNGResourcesPath.Contains('*') == false)
                {
                    TestNGResourcesPath = Path.Combine(TestNGResourcesPath, "*");
                }
                GingerAction.AddExInfo(String.Format("TestNG resources path: '{0}'", TestNGResourcesPath));
            }

            TestNGProjectBinFolderPath = TestNGProjectBinFolderPath.TrimEnd(new char[] { '\\', '/' });
            if (Path.GetFileName(TestNGProjectBinFolderPath).ToLower() != "bin")
            {
                TestNGProjectBinFolderPath = Path.Combine(TestNGProjectBinFolderPath, "\bin");
            }
            if (Directory.Exists(TestNGProjectBinFolderPath) == false)
            {
                GingerAction.AddError(String.Format("Failed to find the TestNG testing project Bin folder at: '{0}'", TestNGProjectBinFolderPath));
                return false;
            }
            else
            {                
                GingerAction.AddExInfo(String.Format("TestNG testing project Bin folder path: '{0}'", TestNGProjectBinFolderPath));
            }

            if (Directory.Exists(TestNGOutputReportFolderPath) == false)
            {
                GingerAction.AddError(String.Format("Failed to find the TestNG output report root folder at: '{0}'", TestNGOutputReportFolderPath));
                return false;
            }
            else
            {
                GingerAction.AddExInfo(String.Format("TestNG output report root folder path: '{0}'", TestNGOutputReportFolderPath));
            }


            switch (ExecutionMode)
            {
                case eExecutionMode.XML:
                    if (TestNgSuiteXML.LoadError != null)
                    {
                        GingerAction.AddError(TestNgSuiteXML.LoadError);
                        return false;
                    }
                    else
                    {
                        GingerAction.AddExInfo(String.Format("TestNG XML path: '{0}'", TestNgSuiteXML.XmlFilePath));
                    }

                    string suiteXmlString = TestNgSuiteXML.SuiteXmlString;

                    if (XmlParametersValuesToOverwrite != null && XmlParametersValuesToOverwrite.Count > 0)
                    {
                        string paramsListStr = "Parameters to overwrite: ";
                        foreach (TestNGTestParameter param in XmlParametersValuesToOverwrite)
                        {
                            param.Name = param.Name.Trim();
                            if (!TestNgSuiteXML.IsParameterExistInXML(param.Name))
                            {
                                GingerAction.AddError(string.Format("The Parameter '{0}' do not exist in the TestNG Suite XML", param.Name));
                                return false;
                            }
                            else
                            {
                                paramsListStr += string.Format("'{0}'='{1}', ", param.Name, param.Value);
                            }
                        }
                        paramsListStr.TrimEnd(',');
                        GingerAction.AddExInfo(paramsListStr);
                    }

                    if (XmlTestsToExecute != null && XmlTestsToExecute.Count > 0)
                    {
                        string testsListStr = "Tests to execute: ";
                        foreach (TestNGTest test in XmlTestsToExecute)
                        {
                            test.Name = test.Name.Trim();
                            if (!TestNgSuiteXML.IsTestExistInXML(test.Name))
                            {
                                GingerAction.AddError(string.Format("The Test '{0}' do not exist in the TestNG Suite XML", test.Name));
                                return false;
                            }
                            else
                            {
                                testsListStr += string.Format("'{0}', ", test.Name);
                            }
                        }
                        testsListStr.TrimEnd(',');
                        GingerAction.AddExInfo(testsListStr);
                    }
                    break;
                case eExecutionMode.Classes:
                    break;
                case eExecutionMode.Methods:
                    break;
                case eExecutionMode.Jar:
                    break;
            }

            if (TestGroupsToInclude != null && TestGroupsToInclude.Count > 0)
            {
                string groupsToIncludeListStr = "Tests Groups to include: ";
                foreach (TestNGTestGroup group in TestGroupsToInclude)
                {
                    groupsToIncludeListStr += string.Format("'{0}', ", group.Name);
                }
                groupsToIncludeListStr.TrimEnd(',');
                GingerAction.AddExInfo(groupsToIncludeListStr);
            }
            if (TestGroupsToExclude != null && TestGroupsToExclude.Count > 0)
            {
                string groupsToExcludeListStr = "Tests Groups to exclude: ";
                foreach (TestNGTestGroup group in TestGroupsToExclude)
                {
                    groupsToExcludeListStr += string.Format("'{0}', ", group.Name);
                }
                groupsToExcludeListStr.TrimEnd(',');
                GingerAction.AddExInfo(groupsToExcludeListStr);
            }

            return true;
        }

        public void Execute()
        {
            if (!ValidateAndPrepareConfigs())
            {
                return;
            }

            //prepare the command 
            CommandValues command = null;
            try
            {
                switch (ExecutionMode)
                {
                    case eExecutionMode.XML:
                        command = PrepareTestngXmlExecutionCommand();
                        break;
                    case eExecutionMode.Classes:
                        break;
                    case eExecutionMode.Methods:
                        break;
                    case eExecutionMode.Jar:
                        break;
                }
            }
            catch(Exception ex)
            {
                GingerAction.AddError(string.Format("Failed to prepare the command to execute, Error: '{0}'", ex.Message));
                return;
            }

            if (command != null)
            {
                //execute the command
                if (ExecuteCommand(command))
                {
                    //parse the Command output

                    //parse the TestNG output result XML 
                    TestNGReportXML ngReport = new TestNGReportXML(Path.Combine(TestNGOutputReportFolderPath, "testng-results.xml"));
                }
            }
            else
            {
                GingerAction.AddError("No command found to exeucte");
            }
        }

        private CommandValues PrepareTestngXmlExecutionCommand()
        {
            CommandValues command = new CommandValues();

            command.ExecuterFilePath = JavaExeFullPath;

            //class path
            command.Arguments = string.Format(" -cp \"{0}\";\"{1}\"", TestNGProjectBinFolderPath, TestNGResourcesPath);

            //testng test arguments
            command.Arguments += " org.testng.TestNG";

            //TestNG XML path
            command.Arguments += string.Format(" \"{0}\"", TestNgSuiteXML.XmlFilePath);

            //Report output path
            command.Arguments += string.Format(" -d \"{0}\"", TestNGOutputReportFolderPath);

            //TODO: add handling to rest of possible arguments


            return command;
        }

        private bool ExecuteCommand(CommandValues commandVals)
        {
            try
            {
                GingerAction.AddExInfo(string.Format("Executed command: '{0}'", commandVals.FullCommand));

                Process process = new Process();
                process.StartInfo.FileName = commandVals.ExecuterFilePath;
                //p.StartInfo.WorkingDirectory = mJavaWSEXEPath_Calc;
                process.StartInfo.Arguments = commandVals.Arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += (proc, outLine) => { AddCommandOutput(outLine.Data + "\n"); };
                process.ErrorDataReceived += (proc, outLine) => { AddCommandOutputError(outLine.Data + "\n"); };
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                //process.BeginOutputReadLine();
                //process.BeginErrorReadLine();
                process.WaitForExit(1000 * 60 * 180);//wait up tp 3 hours

                //string result = process.StandardOutput.ReadToEnd(); //to Parse for output values
                //string comanndExecError = process.StandardError.ReadToEnd();
                //if (!string.IsNullOrEmpty(comanndExecError))
                //{
                //    GingerAction.AddError(comanndExecError);
                //    return false;
                //}

                if (!string.IsNullOrEmpty(mCommandOutputErrorBuffer))
                {
                    GingerAction.AddError(mCommandOutputErrorBuffer);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                GingerAction.AddError(string.Format("Failed to execute the command, Error is: '{0}'", ex.Message));
                return false;
            }
        }

        static protected void AddCommandOutput(string output)
        {
            mCommandOutputBuffer += output;
            Console.WriteLine(output);
        }

        static protected void AddCommandOutputError(string error)
        {
            mCommandOutputErrorBuffer += error;
            Console.WriteLine(error);
        }

    }

    public class CommandValues
    {
        public string ExecuterFilePath;
        public string Arguments;

        public string FullCommand
        {
            get
            {
                return ExecuterFilePath + " " + Arguments;
            }
        }
    }
}
