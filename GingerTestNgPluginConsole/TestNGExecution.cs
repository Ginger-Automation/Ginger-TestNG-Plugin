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
        public bool FailActionDueToTestngResultsXmlFailures;


        IGingerAction mGingerAction = null;
        public IGingerAction GingerAction
        {
            get
            {
                return mGingerAction;
            }
            set
            {
                mGingerAction = value;
                if (mGingerAction != null)
                {
                    mGingerAction.AddExInfo("\n");
                }
            }
        }

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
                    mJavaExeFullPath = General.TrimApostrophes(mJavaExeFullPath);
                    if (string.IsNullOrEmpty(mJavaExeFullPath))
                    {
                        mJavaExeFullPath = Environment.GetEnvironmentVariable("JAVA_HOME");
                    }
                    if (!string.IsNullOrEmpty(mJavaExeFullPath))
                    {
                        if (Path.GetFullPath(mJavaExeFullPath).ToLower().Contains("bin") == false)
                        {
                            mJavaExeFullPath = Path.Combine(mJavaExeFullPath, "bin");
                        }
                        if (Path.GetFullPath(mJavaExeFullPath).ToLower().Contains("java.exe") == false)
                        {
                            mJavaExeFullPath = Path.Combine(mJavaExeFullPath, "java.exe");
                        }

                        mJavaExeFullPath = Path.GetFullPath(mJavaExeFullPath);
                    }                    
                }
                catch (Exception ex)
                {
                    General.AddInfoToConsoleAndAction(GingerAction, string.Format("Failed to init the java.exe file path, Error: '{0}'", ex.Message));
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
                mJavaProjectResourcesPath = General.TrimApostrophes(mJavaProjectResourcesPath);
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
                mJavaProjectBinFolderPath = General.TrimApostrophes(mJavaProjectBinFolderPath);
                if (!string.IsNullOrEmpty(mJavaProjectBinFolderPath))
                {
                    mJavaProjectBinFolderPath = Path.GetFullPath(mJavaProjectBinFolderPath);                   
                    mJavaProjectBinFolderPath = General.TrimEndSleshes(mJavaProjectBinFolderPath);
                    if (Path.GetFileName(mJavaProjectBinFolderPath).ToLower() != "bin")
                    {
                        mJavaProjectBinFolderPath = Path.Combine(mJavaProjectBinFolderPath, "bin");
                    }

                    mJavaProjectBinFolderPath = Path.GetFullPath(mJavaProjectBinFolderPath);
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
                    mMavenCmdFullPath = General.TrimApostrophes(mMavenCmdFullPath);
                    if (string.IsNullOrEmpty(mMavenCmdFullPath))
                    {
                        mMavenCmdFullPath = Environment.GetEnvironmentVariable("MAVEN_HOME");
                    }
                    if (!string.IsNullOrEmpty(mMavenCmdFullPath))
                    {
                        if (Path.GetFullPath(mMavenCmdFullPath).ToLower().Contains(@"\bin") == false)
                        {
                            mMavenCmdFullPath = Path.Combine(mMavenCmdFullPath, @"bin");
                        }
                        if (Path.GetFullPath(mMavenCmdFullPath).ToLower().Contains(@"\mvn.cmd") == false)
                        {
                            mMavenCmdFullPath = Path.Combine(mMavenCmdFullPath, @"mvn.cmd");
                        }

                        mMavenCmdFullPath = Path.GetFullPath(mMavenCmdFullPath);
                    }
                }
                catch (Exception ex)
                {
                    General.AddInfoToConsoleAndAction(GingerAction, string.Format("Failed to init the mvn.cmd file path, Error: '{0}'", ex.Message));
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
                mMavenProjectFolderPath = value;
                mMavenProjectFolderPath = General.TrimApostrophes(mMavenProjectFolderPath);
                if (!string.IsNullOrEmpty(mMavenProjectFolderPath))
                {
                    mMavenProjectFolderPath = Path.GetFullPath(mMavenProjectFolderPath);
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
                mTestNGOutputReportFolderPath = value;
                mTestNGOutputReportFolderPath = General.TrimApostrophes(mTestNGOutputReportFolderPath);
                if (!string.IsNullOrEmpty(mTestNGOutputReportFolderPath))
                {
                    try
                    {

                        mTestNGOutputReportFolderPath = General.TrimRelativeSleshes(mTestNGOutputReportFolderPath);                        
                        if (Path.IsPathRooted(mTestNGOutputReportFolderPath) == false)//relative path provided
                        {
                            if (JavaProjectType == eJavaProjectType.Regular)
                            {
                                if (string.IsNullOrEmpty(JavaProjectBinPath) == false)
                                {
                                    mTestNGOutputReportFolderPath = Path.Combine(Path.GetDirectoryName(JavaProjectBinPath.TrimEnd(new char[] { '\\', '/' })), mTestNGOutputReportFolderPath);
                                }
                            }
                            else //Maven
                            {
                                if (string.IsNullOrEmpty(MavenProjectFolderPath) == false)
                                {
                                    mTestNGOutputReportFolderPath = Path.Combine(MavenProjectFolderPath, mTestNGOutputReportFolderPath);
                                }
                            }

                        }
                        else
                        {
                            mTestNGOutputReportFolderPath = Path.GetFullPath(mTestNGOutputReportFolderPath);
                        }
                        
                        if (!Directory.Exists(mTestNGOutputReportFolderPath))
                        {
                            Directory.CreateDirectory(mTestNGOutputReportFolderPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        General.AddInfoToConsoleAndAction(GingerAction, string.Format("Failed to create the customized TestNG Report folder at: '{0}', Error: '{1}'", mTestNGOutputReportFolderPath, ex.Message));
                    }
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

        public bool OverwriteOriginalTestngXml;

        string mOriginalTestngXmlPath = string.Empty;

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
                mTestngXmlPath = General.TrimApostrophes(mTestngXmlPath);
                if (!string.IsNullOrEmpty(mTestngXmlPath))
                {
                    if (!File.Exists(mTestngXmlPath))
                    {                        
                        mTestngXmlPath = General.TrimRelativeSleshes(mTestngXmlPath);
                        if (Path.IsPathRooted(mTestngXmlPath) == false)//relative path provided
                        {
                            if (JavaProjectType == eJavaProjectType.Regular)
                            {
                                if (string.IsNullOrEmpty(JavaProjectBinPath) == false)
                                {
                                    mTestngXmlPath = Path.Combine(Path.GetDirectoryName(JavaProjectBinPath.TrimEnd(new char[] { '\\', '/' })), mTestngXmlPath);
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

        public TestNGSuiteXML TestNgSuiteXMLObj;

        public List<TestNGTestParameter> TestngXmlParametersToOverwrite = new List<TestNGTestParameter>();

        public List<TestNGTest> XmlTestsToExecute = new List<TestNGTest>();

        public List<TestNGTestGroup> TestGroupsToInclude = new List<TestNGTestGroup>();

        public List<TestNGTestGroup> TestGroupsToExclude = new List<TestNGTestGroup>();


        public bool ContinueExecutionOnTestFailure;
        public bool ValidateAndPrepareConfigs()
        {
            //validate general inputes
            if (Directory.Exists(TempWorkingFolder) == false)
            {
                General.AddErrorToConsoleAndAction(GingerAction, String.Format("Failed to create temp working folder at: '{0}'", TempWorkingFolder));
                return false;
            }
            else
            {
                General.AddInfoToConsoleAndAction(GingerAction, String.Format("Temp working folder path: '{0}'", TempWorkingFolder));
            }

            if (ExecuterType == eExecuterType.Java)
            {
                if (Path.GetFileName(JavaExeFullPath).ToLower() != "java.exe" || File.Exists(JavaExeFullPath) == false)
                {
                    General.AddErrorToConsoleAndAction(GingerAction, String.Format("Failed to find java.exe file at: '{0}'", JavaExeFullPath));
                    return false;
                }
                else
                {
                    General.AddInfoToConsoleAndAction(GingerAction, String.Format("Path of java.exe file: '{0}'", JavaExeFullPath));
                }
            }
            else//Maven Executer
            {
                if (Path.GetFileName(MavenCmdFullPath).ToLower() != "mvn.cmd" || File.Exists(MavenCmdFullPath) == false)
                {
                    General.AddErrorToConsoleAndAction(GingerAction, String.Format("Failed to find mvn.cmd file at: '{0}'", MavenCmdFullPath));
                    return false;
                }
                else
                {
                    General.AddInfoToConsoleAndAction(GingerAction, String.Format("Path of mvn.cmd file: '{0}'", MavenCmdFullPath));
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
                            General.AddErrorToConsoleAndAction(GingerAction, String.Format("Failed to find the TestNG resources folder at: '{0}'", JavaProjectResourcesPath));
                            return false;
                        }
                        else
                        {
                            General.AddInfoToConsoleAndAction(GingerAction, String.Format("TestNG resources path: '{0}'", JavaProjectResourcesPath));
                        }

                        if (Directory.Exists(JavaProjectBinPath) == false)
                        {
                            General.AddErrorToConsoleAndAction(GingerAction, String.Format("Failed to find the TestNG testing project Bin folder at: '{0}'", JavaProjectBinPath));
                            return false;
                        }
                        else
                        {
                            General.AddInfoToConsoleAndAction(GingerAction, String.Format("TestNG testing project Bin folder path: '{0}'", JavaProjectBinPath));
                        }
                    }
                    else //Maven Project
                    {
                        if (Directory.Exists(MavenProjectFolderPath) == false)
                        {
                            General.AddErrorToConsoleAndAction(GingerAction, String.Format("Failed to find the Maven Java project folder at: '{0}'", MavenProjectFolderPath));
                            return false;
                        }
                        else
                        {
                            General.AddInfoToConsoleAndAction(GingerAction, String.Format("Maven Java project path: '{0}'", MavenProjectFolderPath));
                        }
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
                                    General.AddErrorToConsoleAndAction(GingerAction, string.Format("The Test '{0}' do not exist in the TestNG Suite XML", test.Name));
                                    return false;
                                }
                                else
                                {
                                    testsListStr += string.Format("'{0}', ", test.Name);
                                }
                            }
                            testsListStr.TrimEnd(',');
                            General.AddInfoToConsoleAndAction(GingerAction, testsListStr);
                        }

                        if (TestGroupsToInclude != null && TestGroupsToInclude.Count > 0)
                        {
                            string groupsToIncludeListStr = "Tests Groups to include: ";
                            foreach (TestNGTestGroup group in TestGroupsToInclude)
                            {
                                groupsToIncludeListStr += string.Format("'{0}', ", group.Name);
                            }
                            groupsToIncludeListStr.TrimEnd(',');
                            General.AddInfoToConsoleAndAction(GingerAction, groupsToIncludeListStr);
                        }
                        if (TestGroupsToExclude != null && TestGroupsToExclude.Count > 0)
                        {
                            string groupsToExcludeListStr = "Tests Groups to exclude: ";
                            foreach (TestNGTestGroup group in TestGroupsToExclude)
                            {
                                groupsToExcludeListStr += string.Format("'{0}', ", group.Name);
                            }
                            groupsToExcludeListStr.TrimEnd(',');
                            General.AddInfoToConsoleAndAction(GingerAction, groupsToExcludeListStr);
                        }
                    }
                    break;

                case eExecutionMode.FreeCommand:
                    if (string.IsNullOrEmpty(FreeCommandArguments.Trim()))
                    {
                        General.AddErrorToConsoleAndAction(GingerAction, String.Format("Provided Free Command Arguments are not valid: '{0}'", FreeCommandArguments));
                        return false;
                    }
                    else
                    {
                        General.AddInfoToConsoleAndAction(GingerAction, String.Format("Free Command Arguments: '{0}'", FreeCommandArguments));
                    }

                    if (CommandParametersToOverride != null && CommandParametersToOverride.Count > 0)
                    {
                        string paramsListStr = "Command Parameters to override: ";
                        foreach (CommandParameter param in CommandParametersToOverride)
                        {
                            param.Name = param.Name.Trim();                            
                            if (!FreeCommandArguments.Contains(param.Name))
                            {
                                General.AddErrorToConsoleAndAction(GingerAction, string.Format("The Command Parameter '{0}' do not exist in the Command Arguments", param.Name));
                                return false;
                            }
                            else
                            {
                                paramsListStr += string.Format("'{0}'='{1}', ", param.Name, param.Value);
                            }
                        }
                        paramsListStr.TrimEnd(',');
                        General.AddInfoToConsoleAndAction(GingerAction, paramsListStr);
                    }
                    break;
            }

            if (ExecutionMode != eExecutionMode.FreeCommand ||
                (ExecutionMode == eExecutionMode.FreeCommand && string.IsNullOrEmpty(TestngXmlPath) == false))
            {
                TestNgSuiteXMLObj = new TestNGSuiteXML(TestngXmlPath);
                if (TestNgSuiteXMLObj.LoadError != null)
                {
                    General.AddErrorToConsoleAndAction(GingerAction, TestNgSuiteXMLObj.LoadError);
                    return false;
                }
                else
                {
                    General.AddInfoToConsoleAndAction(GingerAction, String.Format("TestNG XML path: '{0}'", TestNgSuiteXMLObj.XmlFilePath));
                }
            }
            
            if (TestNgSuiteXMLObj != null && TestngXmlParametersToOverwrite != null && TestngXmlParametersToOverwrite.Count > 0)
            {
                string paramsListStr = "Parameters to override: ";
                foreach (TestNGTestParameter param in TestngXmlParametersToOverwrite)
                {
                    if (param == null || string.IsNullOrEmpty(param.Name))
                    {
                        continue;
                    }
                    else
                    {
                        param.Name = param.Name.Trim();
                    }
                    if (!string.IsNullOrEmpty(param.ParentNodeName))
                    {
                        param.ParentNodeName = param.ParentNodeName.Trim();
                    }
                    if (!TestNgSuiteXMLObj.IsParameterExistInXML(param.Name, param.ParentNodeName))
                    {
                        if (string.IsNullOrEmpty(param.ParentNodeName))
                        {
                            General.AddErrorToConsoleAndAction(GingerAction, string.Format("The Parameter '{0}' do not exist in the TestNG Suite XML", param.Name));
                        }
                        else
                        {
                            General.AddErrorToConsoleAndAction(GingerAction, string.Format("The Parameter '{0}\\{1}' do not exist in the TestNG Suite XML", param.ParentNodeName, param.Name));
                        }
                        return false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(param.ParentNodeName))
                        {
                            paramsListStr += string.Format("'{0}'='{1}', ", param.Name, param.Value);
                        }
                        else
                        {
                            paramsListStr += string.Format("'{0}\\{1}'='{2}', ", param.ParentNodeName, param.Name, param.Value);
                        }
                    }
                }
                paramsListStr.TrimEnd(',');
                General.AddInfoToConsoleAndAction(GingerAction, paramsListStr);
            }

            if (ParseTestngResultsXml == true)
            {
                if (Directory.Exists(TestngResultsXmlFolderPath) == false)
                {
                    General.AddErrorToConsoleAndAction(GingerAction, String.Format("Failed to find the TestNG output report root folder at: '{0}'", TestngResultsXmlFolderPath));
                    return false;
                }
                else
                {
                    General.AddInfoToConsoleAndAction(GingerAction, String.Format("TestNG output report root folder path: '{0}'", TestngResultsXmlFolderPath));
                }
            }

            return true;
        }

        public void Execute()
        {
            General.AddInfoToConsole("############################################ Execution Started");            
            General.AddInfoToConsole("# Validating Configurations");
            if (!ValidateAndPrepareConfigs())
            {
                return;
            }

            //prepare the customized xml
            General.AddInfoToConsole("# Prepare TestNG Xml for Execution");
            if (!PrepareTestNGXmlForExecution())
            {
                return;
            }

            //prepare the command 
            General.AddInfoToConsole("# Prepare Command for Execution");
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
                General.AddErrorToConsoleAndAction(GingerAction, string.Format("Failed to prepare the command to execute, Error: '{0}'", ex.Message));
                return;
            }

            if (command != null)
            {
                //execute the command
                General.AddInfoToConsole("### Starting Actual Execution");
                if (ExecuteCommand(command))
                {
                    //parse output
                    if (ParseConsoleOutputs)
                    {
                        General.AddInfoToConsole("### Parsing Command Outputs");
                        ParseCommandOutput();
                    }

                    //parse report
                    if (ParseTestngResultsXml)
                    {
                        General.AddInfoToConsole("### Parsing TestNG Results XML");
                        //parse the TestNG output result XML 
                        string testNgReportPath = Path.Combine(TestngResultsXmlFolderPath, "testng-results.xml");
                        General.AddInfoToConsoleAndAction(GingerAction, String.Format("TestNG Results XML full path: '{0}'", testNgReportPath));
                        TestNGReportXML ngReport = new TestNGReportXML(testNgReportPath);
                        if (string.IsNullOrEmpty(ngReport.LoadError) == true)
                        {
                            ngReport.ParseTestNGReport(GingerAction, FailActionDueToTestngResultsXmlFailures);
                        }
                        else
                        {
                            General.AddErrorToConsoleAndAction(GingerAction, "TestNG Output Results XML parsing failed");
                        }
                    }
                }
            }
            else
            {
                General.AddErrorToConsoleAndAction(GingerAction, "No command found to exeucte");
            }

            General.AddInfoToConsole("############################################ Execution Ended");
        }

        private bool PrepareTestNGXmlForExecution()
        {
            TestNGSuiteXML customizedSuiteXML=null;
            try
            {
                switch (ExecutionMode)
                {
                    case eExecutionMode.XML:
                    case eExecutionMode.FreeCommand:
                        General.AddInfoToConsole("Create Custom TestNG Xml for Execution");
                        //Parameters
                        if (TestNgSuiteXMLObj != null && TestngXmlParametersToOverwrite != null && TestngXmlParametersToOverwrite.Count > 0)
                        {
                            customizedSuiteXML = new TestNGSuiteXML(TestngXmlPath);
                            customizedSuiteXML.OverrideXMLParameters(TestngXmlParametersToOverwrite);                            
                        }
                        break;                        
                }
               
                //create temp XML
                if (customizedSuiteXML != null)
                {
                    General.AddInfoToConsole("Save Custom TestNG Xml for Execution");
                    if (OverwriteOriginalTestngXml)
                    {
                        customizedSuiteXML.SuiteXml.Save(TestngXmlPath);//overwrite original TestNG xml                        
                        TestNgSuiteXMLObj = new TestNGSuiteXML(TestngXmlPath);
                        General.AddInfoToConsoleAndAction(GingerAction, String.Format("The Parameters of '{0}' TestNG XML were overwritten", TestngXmlPath));
                    }
                    else
                    { 
                        string customeXMLFilePath = Path.Combine(TempWorkingFolder, "Custom " + Path.GetFileName(TestngXmlPath));
                        customizedSuiteXML.SuiteXml.Save(customeXMLFilePath);
                        mOriginalTestngXmlPath = TestngXmlPath;
                        TestngXmlPath = customeXMLFilePath;
                        TestNgSuiteXMLObj = new TestNGSuiteXML(TestngXmlPath);
                        General.AddInfoToConsoleAndAction(GingerAction, String.Format("Customized TestNG XML path: '{0}'", TestNgSuiteXMLObj.XmlFilePath));
                    }
                }
                else
                {
                    General.AddInfoToConsole("TestNG Xml customization is not needed");
                }
                return true;
            }
            catch(Exception ex)
            {
                General.AddErrorToConsoleAndAction(GingerAction, string.Format("Failed to Prepare TestNG Xml for execution, Error is: '{0}'", ex.Message));
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

            ////command parameters ovveride
            //if (CommandParametersToOverride != null && CommandParametersToOverride.Count > 0)
            //{
            //    commandArgsToExecute = string.Format(" {0}", OverrideCommandParameters());
            //}

            if(OverwriteOriginalTestngXml == false && TestngXmlParametersToOverwrite != null && TestngXmlParametersToOverwrite.Count > 0)
            {
                //need to update the path of the xml in the command arguments to point the customized one
                string xmlName = Path.GetFileName(mOriginalTestngXmlPath);
                int fileNameIndx = FreeCommandArguments.IndexOf(xmlName);
                if (fileNameIndx > 0)
                {
                    int rightIndx = fileNameIndx + xmlName.Length;
                    if (rightIndx < FreeCommandArguments.Length && FreeCommandArguments[rightIndx] == '\"')
                    {
                        rightIndx++; 
                    }
                    int leftIndx = fileNameIndx;
                    while (leftIndx > 0 && FreeCommandArguments[leftIndx-1] != '=')
                    {
                        leftIndx--;
                    }

                    if (rightIndx > leftIndx && rightIndx >0 && leftIndx>0)
                    {
                        string fullXmlPathArgument = FreeCommandArguments.Substring(leftIndx, rightIndx - leftIndx);
                        FreeCommandArguments= FreeCommandArguments.Replace(fullXmlPathArgument, string.Format("\"{0}\"", TestngXmlPath));
                    }
                    else
                    {
                        General.AddErrorToConsoleAndAction(GingerAction, string.Format("Failed to replace the original TestNG XML path '{0}' with the customized one '{1}' in the command arguments", mOriginalTestngXmlPath, TestngXmlPath));
                    }
                }
                else
                {
                    General.AddErrorToConsoleAndAction(GingerAction, string.Format("Failed to replace the original TestNG XML path '{0}' with the customized one '{1}' in the command arguments", mOriginalTestngXmlPath, TestngXmlPath));
                }
            }

            command.Arguments = string.Format(" {0}", FreeCommandArguments); 

            return command;
        }

        private string OverrideCommandParameters()
        {
            //foreach (CommandParameter cmdParam in CommandParametersToOverride)
            //{
            //    string fullParamName = cmdParam.Name.Trim();
            //    if (!string.IsNullOrEmpty(fullParamName))
            //    {
            //        if (fullParamName.IndexOf("-D") != 0)
            //        {
            //            fullParamName = string.Format("-D{0}", fullParamName);
            //        }
            //    }

            //    try
            //    {
            //        Match match = Regex.Match(FreeCommandArguments, string.Format("/{0}=\"([]+)\"/", fullParamName));
            //        if (match.Success)
            //        {
            //            // Finally, we get the Group value and display it.
            //            string key = match.Groups[1].Value;
            //            Console.WriteLine(key);
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}

            return FreeCommandArguments;
        }

        private bool ExecuteCommand(CommandElements commandVals)
        {
            try
            {
                General.AddInfoToConsoleAndAction(GingerAction, string.Format("Executed command: '{0}'", commandVals.FullCommand));

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
                    General.AddErrorToConsoleAndAction(GingerAction, string.Format("Command processing timeout has reached!"));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                General.AddErrorToConsoleAndAction(GingerAction, string.Format("Failed to execute the command, Error is: '{0}'", ex.Message));
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
            General.AddInfoToConsole("Command Execution Ended");
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
                        General.AddErrorToConsoleAndAction(GingerAction, string.Format("Console Errors: \n{0}", mCommandOutputErrorBuffer));
                    }
                    else
                    {
                        General.AddInfoToConsoleAndAction(GingerAction, string.Format("Console Errors: \n{0}", mCommandOutputErrorBuffer));
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
               General.AddInfoToConsoleAndAction(GingerAction, string.Format("Failed to parse all command console outputs, Error:'{0}'", ex.Message));
            }
        }
    }   
}
