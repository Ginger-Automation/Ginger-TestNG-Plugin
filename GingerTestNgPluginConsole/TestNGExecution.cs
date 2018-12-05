using Amdocs.Ginger.Plugin.Core;
using GingerTestNgPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace GingerTestNgPluginConsole
{
    public class TestNGExecution
    {
        public IGingerAction GingerAction;

        static string mCommandOutputBuffer = string.Empty;
        static string mCommandOutputErrorBuffer = string.Empty;

        public enum eJavaProjectType { Regular, Maven}
        public eJavaProjectType JavaProjectType;
        public enum eExecutionMode { XML, Classes, Methods, Jar, FreeCommand }
        public eExecutionMode ExecutionMode;

        public bool ParseConsoleOutputs;

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

        #region JavaProject
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
                    Console.WriteLine(string.Format("Failed to init the java.exe file path, Error: '{0}'", ex.Message));
                }
            }
        }

        string mJavaProjectResourcesPath = null;
        public string JavaProjectResourcesPath
        {
            get
            {
                return mJavaProjectResourcesPath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    mJavaProjectResourcesPath = Path.GetFullPath(value);
                }
            }
        }

        string mJavaProjectBinFolderPath = null;
        public string JavaProjectBinFolderPath
        {
            get
            {
                return mJavaProjectBinFolderPath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    mJavaProjectBinFolderPath = Path.GetFullPath(value);
                }
            }
        }
        #endregion

        #region MavenProject
        string mMavenCmdFullPath = null;
        public string MavenCmdFullPath
        {
            get
            {
                return mMavenCmdFullPath;
            }
            set
            {
                try
                {
                    mMavenCmdFullPath = value;
                    if (string.IsNullOrEmpty(mMavenCmdFullPath))
                    {
                        mMavenCmdFullPath = Environment.GetEnvironmentVariable("MAVEN_HOME");
                    }
                    if (string.IsNullOrEmpty(mMavenCmdFullPath))
                    {
                        if (Path.GetFullPath(mMavenCmdFullPath).ToLower().Contains(@"\bin\") == false)
                        {
                            mMavenCmdFullPath = Path.Combine(mMavenCmdFullPath, @"\bin");
                        }
                        if (Path.GetFullPath(mMavenCmdFullPath).ToLower().Contains(@"\mvn.cmd") == false)
                        {
                            mMavenCmdFullPath = Path.Combine(mMavenCmdFullPath, @"\mvn.cmd");
                        }

                        mMavenCmdFullPath = Path.GetFullPath(mMavenCmdFullPath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to init the mvn.cmd file path, Error: '{0}'", ex.Message));
                }
            }
        }

        string mMavenProjectFolderPath = null;
        public string MavenProjectFolderPath
        {
            get
            {
                return mMavenProjectFolderPath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    mMavenProjectFolderPath = Path.GetFullPath(value);
                }
            }
        }

        public bool PerformMavenInstall = true;

        public string MavenCommandArguments;

        public List<MavenCommandParameter> MavenCommandParameters = new List<MavenCommandParameter>();
        #endregion

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
                    if (JavaProjectType == eJavaProjectType.Regular)
                    {
                        mTestNGOutputReportFolderPath = TempWorkingFolder;
                    }
                    else//Maven
                    {
                        if (Directory.Exists(MavenProjectFolderPath))
                        { 
                            mTestNGOutputReportFolderPath = Path.Combine(MavenProjectFolderPath, @"target\surefire-reports");
                            if (Directory.Exists(mTestNGOutputReportFolderPath) == false)
                            {
                                Directory.CreateDirectory(mTestNGOutputReportFolderPath);
                            }
                        }
                    }
                }
            }
        }

        public bool ContinueExecutionOnTestFailure;

        public TestNGSuiteXML TestNgSuiteXML;

        public List<TestNGTestParameter> XmlParametersToOverwrite = new List<TestNGTestParameter>();

        public List<TestNGTest> XmlTestsToExecute = new List<TestNGTest>();

        public List<TestNGTestGroup> TestGroupsToInclude = new List<TestNGTestGroup>();

        public List<TestNGTestGroup> TestGroupsToExclude = new List<TestNGTestGroup>();


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

            if (JavaProjectType == eJavaProjectType.Regular)
            {
                if (Path.GetFileName(JavaExeFullPath).ToLower() != "java.exe" || File.Exists(JavaExeFullPath) == false)
                {
                    GingerAction.AddError(String.Format("Failed to find 'java.exe' at: '{0}'", JavaExeFullPath));
                    return false;
                }
                else
                {
                    GingerAction.AddExInfo(String.Format("Path of 'java.exe' file: '{0}'", JavaExeFullPath));
                }

                if (Directory.Exists(JavaProjectResourcesPath.Trim().TrimEnd('*')) == false)
                {
                    GingerAction.AddError(String.Format("Failed to find the TestNG resources folder at: '{0}'", JavaProjectResourcesPath));
                    return false;
                }
                else
                {
                    if (JavaProjectResourcesPath.Contains('*') == false)
                    {
                        JavaProjectResourcesPath = Path.Combine(JavaProjectResourcesPath, "*");
                    }
                    GingerAction.AddExInfo(String.Format("TestNG resources path: '{0}'", JavaProjectResourcesPath));
                }

                JavaProjectBinFolderPath = JavaProjectBinFolderPath.TrimEnd(new char[] { '\\', '/' });
                if (Path.GetFileName(JavaProjectBinFolderPath).ToLower() != "bin")
                {
                    JavaProjectBinFolderPath = Path.Combine(JavaProjectBinFolderPath, "\bin");
                }
                if (Directory.Exists(JavaProjectBinFolderPath) == false)
                {
                    GingerAction.AddError(String.Format("Failed to find the TestNG testing project Bin folder at: '{0}'", JavaProjectBinFolderPath));
                    return false;
                }
                else
                {
                    GingerAction.AddExInfo(String.Format("TestNG testing project Bin folder path: '{0}'", JavaProjectBinFolderPath));
                }
            }
            else //Maven Project
            {
                if (Path.GetFileName(MavenCmdFullPath).ToLower() != "mvn.cmd" || File.Exists(MavenCmdFullPath) == false)
                {
                    GingerAction.AddError(String.Format("Failed to find 'mvn.cmd' at: '{0}'", MavenCmdFullPath));
                    return false;
                }
                else
                {
                    GingerAction.AddExInfo(String.Format("Path of 'mvn.cmd' file: '{0}'", MavenCmdFullPath));
                }

                if (Directory.Exists(MavenProjectFolderPath) == false)
                {
                    GingerAction.AddError(String.Format("Failed to find the Maven Java project folder at: '{0}'", MavenProjectFolderPath));
                    return false;
                }
                else
                {
                    GingerAction.AddExInfo(String.Format("Maven Java project path: '{0}'", MavenProjectFolderPath));
                }
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

                    if (XmlParametersToOverwrite != null && XmlParametersToOverwrite.Count > 0)
                    {
                        string paramsListStr = "Parameters to overwrite: ";
                        foreach (TestNGTestParameter param in XmlParametersToOverwrite)
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
                        if (JavaProjectType == eJavaProjectType.Regular)
                        {
                            command = PrepareTestngXmlExecutionCommand();
                        }
                        else//Maven
                        {
                            command = PrepareMavenTestngXmlExecutionCommand();
                        }
                        break;
                    case eExecutionMode.Classes:
                        break;
                    case eExecutionMode.Methods:
                        break;
                    case eExecutionMode.Jar:
                        break;
                    case eExecutionMode.FreeCommand:
                        if (JavaProjectType == eJavaProjectType.Maven)
                        {
                            command = PrepareMavenFreeCommand();
                        }
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
                    //parse report
                    if (ExecutionMode == eExecutionMode.XML)
                    {
                        //parse the TestNG output result XML 
                        string testNgReportPath = Path.Combine(TestNGOutputReportFolderPath, "testng-results.xml");
                        TestNGReportXML ngReport = new TestNGReportXML(testNgReportPath);
                        if (string.IsNullOrEmpty(ngReport.LoadError) == true)
                        {
                            ngReport.ParseTestNGReport(GingerAction);
                        }
                        else
                        {
                            GingerAction.AddError(string.Format("Failed to parse the TestNG output report at path: '{0}'", testNgReportPath));
                        }
                    }
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

            command.ExecuterFilePath = string.Format("\"{0}\"", JavaExeFullPath);

            //class path
            command.Arguments = string.Format(" -cp \"{0}\";\"{1}\"", JavaProjectBinFolderPath, JavaProjectResourcesPath);

            //testng test arguments
            command.Arguments += " org.testng.TestNG";

            //TestNG XML path
            command.Arguments += string.Format(" \"{0}\"", TestNgSuiteXML.XmlFilePath);

            //Report output path
            command.Arguments += string.Format(" -d \"{0}\"", TestNGOutputReportFolderPath);

            //TODO: add handling to rest of possible arguments


            return command;
        }

        private CommandValues PrepareMavenTestngXmlExecutionCommand()
        {
            CommandValues command = new CommandValues();

            command.WorkingFolder = MavenProjectFolderPath;
            command.ExecuterFilePath = string.Format("\"{0}\"", MavenCmdFullPath);

            //Mvn arguments
            if (PerformMavenInstall)
            {
                command.Arguments = " clean install test";
            }
            else
            {
                command.Arguments = " clean test";
            }            

            //Maven parameters
            if (MavenCommandParameters != null && MavenCommandParameters.Count>0)
            {
                foreach (MavenCommandParameter mvnParam in MavenCommandParameters)
                {                    
                    if (!string.IsNullOrEmpty(mvnParam.Name.Trim()))
                    {
                        command.Arguments += string.Format(" -D{0}=\"{1}\"", mvnParam.Name.Trim(), mvnParam.Value);
                    }
                }
            }

            //TestNG XML path
            command.Arguments += string.Format(" -Dsurefire.suiteXmlFiles=\"{0}\"", TestNgSuiteXML.XmlFilePath);


            return command;
        }

        private CommandValues PrepareMavenFreeCommand()
        {
            CommandValues command = new CommandValues();

            command.WorkingFolder = MavenProjectFolderPath;
            command.ExecuterFilePath = MavenCmdFullPath;

            //Mvn arguments
            command.Arguments = string.Format(" {0}", MavenCommandArguments);

            //Maven parameters
            if (MavenCommandParameters != null && MavenCommandParameters.Count > 0)
            {
                foreach (MavenCommandParameter mvnParam in MavenCommandParameters)
                {
                    if (!string.IsNullOrEmpty(mvnParam.Name.Trim()))
                    {
                        command.Arguments += string.Format(" -D{0}=\"{1}\"", mvnParam.Name.Trim(), mvnParam.Value);
                    }
                }
            }

            return command;
        }

        private bool ExecuteCommand(CommandValues commandVals)
        {
            try
            {
                GingerAction.AddExInfo(string.Format("Executed command: '{0}'", commandVals.FullCommand));

                Process process = new Process();
                if (commandVals.WorkingFolder != null)
                {
                    process.StartInfo.WorkingDirectory = commandVals.WorkingFolder;
                }
                process.StartInfo.FileName = commandVals.ExecuterFilePath;               
                process.StartInfo.Arguments = commandVals.Arguments;

                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                mCommandOutputBuffer = string.Empty;
                mCommandOutputErrorBuffer = string.Empty;
                process.OutputDataReceived += (proc, outLine) => { AddCommandOutput(outLine.Data); };
                process.ErrorDataReceived += (proc, outLine) => { AddCommandOutputError(outLine.Data); };
                process.Exited += Process_Exited;

                process.Start();
                
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                int maxWaitingTime = 1000 * 60 * 180;//3 hours
                process.WaitForExit(maxWaitingTime);

                if (process.TotalProcessorTime.TotalMilliseconds >= maxWaitingTime)
                {
                    GingerAction.AddError(string.Format("Command processing timeout has reached!"));
                    return false;
                }

                return true;

                //int maxWaitingTime = 100 * 10 * 60 * 180;//3 hours
                //int counter = 0;
                //while (!process.HasExited)
                //{
                //    Thread.Sleep(100);
                //    counter += 100;
                //    if (counter >= maxWaitingTime)
                //    {
                //        GingerAction.AddError(string.Format("Command processing timeout has reached!"));
                //        return false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                GingerAction.AddError(string.Format("Failed to execute the command, Error is: '{0}'", ex.Message));
                return false;
            }
        }

        static protected void AddCommandOutput(string output)
        {
            mCommandOutputBuffer += output + "\n";
            Console.WriteLine(output);
        }

        static protected void AddCommandOutputError(string error)
        {
            mCommandOutputErrorBuffer += error + "\n";
            Console.WriteLine(error);
        }

        protected void Process_Exited(object sender, EventArgs e)
        {
            if (ParseConsoleOutputs)
            {
                ParseCommandOutput();
            }
        }

        private void ParseCommandOutput()
        {
            try
            {
                //Error
                if (!string.IsNullOrEmpty(mCommandOutputErrorBuffer))
                {
                    GingerAction.AddExInfo(string.Format("Console Errors: \n{0}", mCommandOutputErrorBuffer));
                }

                //Output values
                Regex rg = new Regex(@"Microsoft.*\n.*All rights reserved.");
                string stringToProcess = rg.Replace(mCommandOutputBuffer, "");
                string[] values = stringToProcess.Split('\n');
                foreach (string dataRow in values)
                {
                    if (dataRow.Length > 0) // Ignore empty lines
                    {
                        string param;
                        string value;
                        int signIndex = dataRow.IndexOf('=');
                        if (signIndex > 0)
                        {
                            param = dataRow.Substring(0, signIndex);
                            //the rest is the value
                            value = dataRow.Substring(param.Length + 1);
                            GingerAction.AddOutput(param, value, "Console Output");
                        }                        
                    }
                }
            }
            catch(Exception ex)
            {
                GingerAction.AddExInfo(string.Format("Failed to parse all command console outputs, Error:'{0}'", ex.Message));
            }
        }

        //public void SetXmlParametersValuesToOverwriteFromString(string xmlParams)
        //{
        //    if (!string.IsNullOrEmpty(xmlParams.Trim()))
        //    {
        //        foreach (string param in xmlParams.Split(','))
        //        {
        //            if (param.Contains('='))
        //            {
        //                string[] paramArgs = param.Split('=');
        //                TestNGTestParameter paramObj = new TestNGTestParameter();
        //                paramObj.Name = paramArgs[0].Trim();
        //                paramObj.Value = paramArgs[1];
        //                XmlParametersToOverwrite.Add(paramObj);
        //            }
        //        }
        //    }
        //}

        //public void SetXmlTestsToExecuteFromString(string xmlTests)
        //{
        //    if (!string.IsNullOrEmpty(xmlTests.Trim()))
        //    {
        //        foreach (string test in xmlTests.Split(','))
        //        {
        //            TestNGTest testObj = new TestNGTest();
        //            testObj.Name = test;
        //            XmlTestsToExecute.Add(testObj);
        //        }
        //    }
        //}

        //public void SetTestGroupsFromString(string groups, List<TestNGTestGroup> listToAddto)
        //{
        //    if (!string.IsNullOrEmpty(groups.Trim()))
        //    {
        //        foreach (string group in groups.Split(','))
        //        {
        //            TestNGTestGroup groupObj = new TestNGTestGroup();
        //            groupObj.Name = group;
        //            listToAddto.Add(groupObj);
        //        }
        //    }
        //}

    }

    public class CommandValues
    {
        public string WorkingFolder;
        public string ExecuterFilePath;
        public string Arguments;

        public string FullCommand
        {
            get
            {
                if (WorkingFolder == null)
                {
                    return string.Format("{0} {1}", ExecuterFilePath, Arguments);
                }
                else
                {
                    return string.Format("{0}>{1} {2}", WorkingFolder, ExecuterFilePath, Arguments);
                }
            }
        }
    }
}
