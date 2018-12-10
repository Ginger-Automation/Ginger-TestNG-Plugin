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

        public enum eExecuterType { Java, Maven }
        public eExecuterType ExecuterType;

        public enum eJavaProjectType { Regular, Maven}
        public eJavaProjectType JavaProjectType;

        public enum eExecutionMode { XML, DynamicXML, FreeCommand }
        public eExecutionMode ExecutionMode;
        
        static string mCommandOutputBuffer = string.Empty;
        static string mCommandOutputErrorBuffer = string.Empty;

        public bool ParseConsoleOutputs;
        public bool FailActionDueToConsoleErrors;

        public bool ParseTestngResultsXml;
        public bool FailActionDueToTestngResultsFailures;

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
                mJavaProjectResourcesPath = value;
                if (!string.IsNullOrEmpty(mJavaProjectResourcesPath))
                {
                    mJavaProjectResourcesPath = Path.GetFullPath(mJavaProjectResourcesPath);

                    if (mJavaProjectResourcesPath.Contains('*') == false)
                    {
                        mJavaProjectResourcesPath = Path.Combine(mJavaProjectResourcesPath, "*");
                    }
                }
            }
        }

        string mJavaProjectBinFolderPath = null;
        public string JavaProjectBinPath
        {
            get
            {
                return mJavaProjectBinFolderPath;
            }
            set
            {
                mJavaProjectBinFolderPath = value;
                if (!string.IsNullOrEmpty(mJavaProjectBinFolderPath))
                {
                    mJavaProjectBinFolderPath = Path.GetFullPath(mJavaProjectBinFolderPath);
                    mJavaProjectBinFolderPath = mJavaProjectBinFolderPath.TrimEnd(new char[] { '\\', '/' });
                    if (Path.GetFileName(mJavaProjectBinFolderPath).ToLower() != "bin")
                    {
                        mJavaProjectBinFolderPath = Path.Combine(mJavaProjectBinFolderPath, "\bin");
                    }
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
        #endregion

        string mFreeCommandArguments = null;
        public string FreeCommandArguments
        {
            get
            {
                return mFreeCommandArguments;
            }
            set
            {
                mFreeCommandArguments = value;
                if (!string.IsNullOrEmpty(mFreeCommandArguments))
                {
                   //trim java or mvn
                   if (ExecuterType == eExecuterType.Java)
                    {
                        if (mFreeCommandArguments.TrimStart().IndexOf("java") == 0)
                        {
                            mFreeCommandArguments= mFreeCommandArguments.Replace("java", "").TrimStart();
                        }
                    }
                   else//Maven executer
                    {
                        if (mFreeCommandArguments.TrimStart().IndexOf("mvn") == 0)
                        {
                            mFreeCommandArguments = mFreeCommandArguments.Replace("mvn", "").TrimStart();
                        }
                    }                   
                }
            }
        }
        public List<CommandParameter> CommandParametersToOverride = new List<CommandParameter>();

        string mTestNGOutputReportFolderPath = null;
        public string TestngResultsXmlFolderPath
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

        string mTestngXmlPath = null;
        public string TestngXmlPath
        {
            get
            {
                return mTestngXmlPath;
            }
            set
            {
                mTestngXmlPath = value;
                if (!string.IsNullOrEmpty(mTestngXmlPath))
                {
                    if (!File.Exists(mTestngXmlPath))
                    {
                        if (Path.IsPathRooted(mTestngXmlPath) == false)//relative path provided
                        {
                            if (ExecutionMode == eExecutionMode.XML)
                            {
                                if (JavaProjectType == eJavaProjectType.Regular)
                                {
                                    if (string.IsNullOrEmpty(JavaProjectBinPath) == false)
                                    {
                                        mTestngXmlPath = Path.Combine(Path.GetDirectoryName(JavaProjectBinPath), mTestngXmlPath);
                                    }
                                }
                                else //Maven
                                {
                                    if (string.IsNullOrEmpty(MavenProjectFolderPath) == false)
                                    {
                                        mTestngXmlPath = Path.Combine(MavenProjectFolderPath, mTestngXmlPath);
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }

        public TestNGSuiteXML TestNgSuiteXMLObj;

        public List<TestNGTestParameter> TestngXmlParametersToOverride = new List<TestNGTestParameter>();

        public List<TestNGTest> XmlTestsToExecute = new List<TestNGTest>();

        public List<TestNGTestGroup> TestGroupsToInclude = new List<TestNGTestGroup>();

        public List<TestNGTestGroup> TestGroupsToExclude = new List<TestNGTestGroup>();


        public bool ContinueExecutionOnTestFailure;
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

            if (ExecuterType == eExecuterType.Java)
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
            }
            else//Maven Executer
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
            }

            switch (ExecutionMode)
            {
                case eExecutionMode.XML:
                case eExecutionMode.DynamicXML:
                    if (JavaProjectType == eJavaProjectType.Regular)
                    {
                        if (Directory.Exists(JavaProjectResourcesPath.Trim().TrimEnd('*')) == false)
                        {
                            GingerAction.AddError(String.Format("Failed to find the TestNG resources folder at: '{0}'", JavaProjectResourcesPath));
                            return false;
                        }
                        else
                        {
                            GingerAction.AddExInfo(String.Format("TestNG resources path: '{0}'", JavaProjectResourcesPath));
                        }

                        if (Directory.Exists(JavaProjectBinPath) == false)
                        {
                            GingerAction.AddError(String.Format("Failed to find the TestNG testing project Bin folder at: '{0}'", JavaProjectBinPath));
                            return false;
                        }
                        else
                        {
                            GingerAction.AddExInfo(String.Format("TestNG testing project Bin folder path: '{0}'", JavaProjectBinPath));
                        }
                    }
                    else //Maven Project
                    {
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

                    TestNgSuiteXMLObj = new TestNGSuiteXML(TestngXmlPath);
                    if (TestNgSuiteXMLObj.LoadError != null)
                    {
                        GingerAction.AddError(TestNgSuiteXMLObj.LoadError);
                        return false;
                    }
                    else
                    {
                        GingerAction.AddExInfo(String.Format("TestNG XML path: '{0}'", TestNgSuiteXMLObj.XmlFilePath));
                    }

                    string suiteXmlString = TestNgSuiteXMLObj.SuiteXmlString;

                    if (TestngXmlParametersToOverride != null && TestngXmlParametersToOverride.Count > 0)
                    {
                        string paramsListStr = "Parameters to override: ";
                        foreach (TestNGTestParameter param in TestngXmlParametersToOverride)
                        {
                            param.Name = param.Name.Trim();
                            if (!TestNgSuiteXMLObj.IsParameterExistInXML(param.Name))
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

                    if (ExecutionMode == eExecutionMode.DynamicXML)
                    {
                        if (XmlTestsToExecute != null && XmlTestsToExecute.Count > 0)
                        {
                            string testsListStr = "Tests to execute: ";
                            foreach (TestNGTest test in XmlTestsToExecute)
                            {
                                test.Name = test.Name.Trim();
                                if (!TestNgSuiteXMLObj.IsTestExistInXML(test.Name))
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
                    }
                    break;

                case eExecutionMode.FreeCommand:
                    if (string.IsNullOrEmpty(FreeCommandArguments.Trim()))
                    {
                        GingerAction.AddError(String.Format("Provided Free Command Arguments are not valid: '{0}'", FreeCommandArguments));
                        return false;
                    }
                    else
                    {
                        GingerAction.AddExInfo(String.Format("Free Command Arguments: '{0}'", FreeCommandArguments));
                    }

                    if (CommandParametersToOverride != null && CommandParametersToOverride.Count > 0)
                    {
                        string paramsListStr = "Command Parameters to override: ";
                        foreach (CommandParameter param in CommandParametersToOverride)
                        {
                            param.Name = param.Name.Trim();                            
                            if (!FreeCommandArguments.Contains(param.Name))
                            {
                                GingerAction.AddError(string.Format("The Command Parameter '{0}' do not exist in the Command Arguments", param.Name));
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
                    break;
            }

            if (ParseTestngResultsXml == true)
            {
                if (Directory.Exists(TestngResultsXmlFolderPath) == false)
                {
                    GingerAction.AddError(String.Format("Failed to find the TestNG output report root folder at: '{0}'", TestngResultsXmlFolderPath));
                    return false;
                }
                else
                {
                    GingerAction.AddExInfo(String.Format("TestNG output report root folder path: '{0}'", TestngResultsXmlFolderPath));
                }
            }

            return true;
        }

        public void Execute()
        {
            if (!ValidateAndPrepareConfigs())
            {
                return;
            }

            //prepare the customized xml
            if (!PrepareTestNGXmlForExecution())
            {
                return;
            }

            //prepare the command 
            CommandElements command = null;
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

                    case eExecutionMode.FreeCommand:
                        if (ExecuterType == eExecuterType.Maven)
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
                    //parse output
                    if (ParseConsoleOutputs)
                    {
                        ParseCommandOutput();
                    }

                    //parse report
                    if (ParseTestngResultsXml)
                    {
                        //parse the TestNG output result XML 
                        string testNgReportPath = Path.Combine(TestngResultsXmlFolderPath, "testng-results.xml");
                        TestNGReportXML ngReport = new TestNGReportXML(testNgReportPath);
                        if (string.IsNullOrEmpty(ngReport.LoadError) == true)
                        {
                            ngReport.ParseTestNGReport(GingerAction, FailActionDueToTestngResultsFailures);
                        }
                        else
                        {
                            GingerAction.AddError(string.Format("Failed to parse the TestNG output report at path: '{0}', due to the Error '{1}'", testNgReportPath, ngReport.LoadError));
                        }
                    }
                }
            }
            else
            {
                GingerAction.AddError("No command found to exeucte");
            }
        }

        private bool PrepareTestNGXmlForExecution()
        {
            TestNGSuiteXML customizedSuiteXML=null;
            try
            {
                switch (ExecutionMode)
                {
                    case eExecutionMode.XML:
                        //Parameters
                        if (TestngXmlParametersToOverride != null && TestngXmlParametersToOverride.Count > 0)
                        {
                            customizedSuiteXML = new TestNGSuiteXML(TestNgSuiteXMLObj.XmlFilePath);
                            customizedSuiteXML.OverrideXMLParameters(TestngXmlParametersToOverride);                            
                        }
                        break;
                }
               
                //create temp XML
                if (customizedSuiteXML != null)
                {
                    string customeXMLFilePath = Path.Combine(TempWorkingFolder, "CustomeTestng.xml");
                    customizedSuiteXML.SuiteXml.Save(customeXMLFilePath);
                    TestngXmlPath = customeXMLFilePath;
                    TestNgSuiteXMLObj = new TestNGSuiteXML(TestngXmlPath);
                    GingerAction.AddExInfo(String.Format("Customized TestNG XML path: '{0}'", TestNgSuiteXMLObj.XmlFilePath));
                }
                return true;
            }
            catch(Exception ex)
            {
                GingerAction.AddError(string.Format("Failed to Prepare TestNG Xml for execution, Error is: '{0}'", ex.Message));
                return false;
            }
        }

        private CommandElements PrepareTestngXmlExecutionCommand()
        {
            CommandElements command = new CommandElements();

            command.ExecuterFilePath = string.Format("\"{0}\"", JavaExeFullPath);

            //class path
            command.Arguments = string.Format(" -cp \"{0}\";\"{1}\"", JavaProjectBinPath, JavaProjectResourcesPath);

            //testng test arguments
            command.Arguments += " org.testng.TestNG";

            //TestNG XML path
            command.Arguments += string.Format(" \"{0}\"", TestNgSuiteXMLObj.XmlFilePath);

            //Report output path
            command.Arguments += string.Format(" -d \"{0}\"", TestngResultsXmlFolderPath);

            return command;
        }

        private CommandElements PrepareMavenTestngXmlExecutionCommand()
        {
            CommandElements command = new CommandElements();

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

            //TestNG XML path
            command.Arguments += string.Format(" -Dsurefire.suiteXmlFiles=\"{0}\"", TestNgSuiteXMLObj.XmlFilePath);

            return command;
        }

        private CommandElements PrepareMavenFreeCommand()
        {
            CommandElements command = new CommandElements();

            command.WorkingFolder = MavenProjectFolderPath;
            command.ExecuterFilePath = string.Format("\"{0}\"", MavenCmdFullPath); 

            string commandArgsToExecute= string.Format(" {0}", FreeCommandArguments);

            //command parameters ovveride
            if (CommandParametersToOverride != null && CommandParametersToOverride.Count > 0)
            {
                commandArgsToExecute = string.Format(" {0}", OverrideCommandParameters());
            }

            command.Arguments = commandArgsToExecute;

            return command;
        }

        private string OverrideCommandParameters()
        {
            foreach (CommandParameter cmdParam in CommandParametersToOverride)
            {
                string fullParamName = cmdParam.Name.Trim();
                if (!string.IsNullOrEmpty(fullParamName))
                {
                    if (fullParamName.IndexOf("-D") != 0)
                    {
                        fullParamName = string.Format("-D{0}", fullParamName);
                    }
                }

                try
                {
                    Match match = Regex.Match(FreeCommandArguments, string.Format("/{0}=\"([]+)\"/", fullParamName));
                    if (match.Success)
                    {
                        // Finally, we get the Group value and display it.
                        string key = match.Groups[1].Value;
                        Console.WriteLine(key);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return FreeCommandArguments;
        }

        private bool ExecuteCommand(CommandElements commandVals)
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

                int maxWaitingTime = 1000 * 60 * 60;//1 hour
                process.WaitForExit(maxWaitingTime);

                if (process.TotalProcessorTime.TotalMilliseconds >= maxWaitingTime)
                {
                    GingerAction.AddError(string.Format("Command processing timeout has reached!"));
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
        }

        private void ParseCommandOutput()
        {
            try
            {
                //Error
                if (!string.IsNullOrEmpty(mCommandOutputErrorBuffer.Trim().Trim('\n')))
                {
                    if (FailActionDueToConsoleErrors)
                    {
                        GingerAction.AddError(string.Format("Console Errors: \n{0}", mCommandOutputErrorBuffer));
                    }
                    else
                    {
                        GingerAction.AddExInfo(string.Format("Console Errors: \n{0}", mCommandOutputErrorBuffer));
                    }
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
    }   
}
