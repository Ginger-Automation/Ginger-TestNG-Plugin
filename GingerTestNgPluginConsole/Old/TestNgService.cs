using Amdocs.Ginger.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using GingerTestNgPlugin;

namespace StandAloneActions
{
    //[GingerService("TestNG", "Execute TestNG scripts from Ginger")]
    public class TestNgService //: IStandAloneAction, IGingerService
    {
        /// <summary>
        /// Execute Test from Gingerwith TestNG
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="TestNgXMlName"></param>
        /// <param name="ProjectLocation"></param>
        /// <param name="LibraryFolder"></param>
        /// <param name="JavaLocation"></param>
        /// <param name="FreeCommand"></param>
        [GingerAction("RunTestNgSuite", "Run TestNG Suite from XMl")]

        public void RunTestNgSuite(IGingerAction GA, string TestNgXMlName, string ProjectLocation, string LibraryFolder, string JavaLocation, string ShowMethodDetails)
        {
            bool AddmethodDetailstoOutput = false;
            if (!string.IsNullOrEmpty(ShowMethodDetails) && ShowMethodDetails.ToUpper() == "TRUE")
            {
                AddmethodDetailstoOutput = true;
            }
            try
            {
                TestNGReport Report;

                string TestNgxmlPath;
                if (ProjectLocation.EndsWith("\\"))
                {
                    TestNgxmlPath = ProjectLocation + TestNgXMlName;
                }
                else
                {
                    TestNgxmlPath = ProjectLocation + "\\" + TestNgXMlName;
                }
                string TestNGXML = System.IO.File.ReadAllText(TestNgxmlPath);

                Report = TestNGSuite.Execute(TestNgXMlName, ProjectLocation, LibraryFolder, JavaLocation);

                ProcessTestNGReport(GA, Report, AddmethodDetailstoOutput);
            }

            catch (Exception EX)
            {
                GA.AddError("TestNg Execution Failed");
                GA.AddExInfo(EX.Message);
            }
        }

        [GingerAction("Run TestNg with Maven", "Run TestNg suites as part of Maven Build with Surefire")]
        public void RunTestNgWithMaven(IGingerAction GA, string TestNgSuiteXML, string WorkingDirectory, string MavenBinDirectory, string MavenSettingsFile, string Commandlinearguments, string ShowMethodDetails)
        {
            bool AddmethodDetailstoOutput = false;
            if (!string.IsNullOrEmpty(ShowMethodDetails) && ShowMethodDetails.ToUpper() == "TRUE")
            {
                AddmethodDetailstoOutput = true;
            }
            StringBuilder FreeCommand = new StringBuilder("");
            if (!string.IsNullOrEmpty(MavenBinDirectory))
            {

                string MavenCmd = MavenBinDirectory.EndsWith(@"\") ? MavenBinDirectory + "mvn.cmd" : MavenBinDirectory + @"\mvn.cmd";
                FreeCommand.Append(MavenCmd);
            }
            else
            {
                FreeCommand.Append("mvn");
            }

            if (!string.IsNullOrEmpty(MavenSettingsFile))
            {
                FreeCommand.Append(" -s " + MavenSettingsFile);
            }

            if (string.IsNullOrEmpty(TestNgSuiteXML))
            {
                GA.AddError("Tests to run not specified");
                return;
            }
            else
            {
                FreeCommand.Append(@" test -DsuiteToRun=" + TestNgSuiteXML);
            }

            if (!string.IsNullOrEmpty(Commandlinearguments))
            {
                FreeCommand.Append(" " + Commandlinearguments);
            }

            TestNGReport Report = TestNGSuite.Execute(FreeCommand.ToString(), WorkingDirectory, @"target\surefire-reports");

            ProcessTestNGReport(GA, Report, AddmethodDetailstoOutput);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="Freecommand"></param>
        /// <param name="WorkingDirectory"></param>
        /// <param name="ReportsDirectory"></param>
        /// <param name="MavenSettingsFile"></param>
        /// <param name="Commandlinearguments"></param>
        /// <param name="ShowMethodDetails"></param>
        [GingerAction("Run TestNg with Free Command", "Run TestNg with Free Command")]
        public void RunTestNgFreeCommand(IGingerAction GA, string Freecommand, string WorkingDirectory, string ReportsDirectory, string MavenSettingsFile, string Commandlinearguments, string ShowMethodDetails)
        {
            bool AddmethodDetailstoOutput = false;
            if (!string.IsNullOrEmpty(ShowMethodDetails) && ShowMethodDetails.ToUpper() == "TRUE")
            {
                AddmethodDetailstoOutput = true;
            }

            TestNGReport Report = TestNGSuite.Execute(Freecommand, WorkingDirectory, ReportsDirectory);

            ProcessTestNGReport(GA, Report, AddmethodDetailstoOutput);
            GA.AddOutput("test", "text");
            GA.AddExInfo("test");
            GA.AddError("action faile test");
        }

        /// <summary>
        /// Process TestNgReport object and update the action with outcome
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="Report"></param>
        public static void ProcessTestNGReport(IGingerAction GA, TestNGReport Report, bool AddMethoudDetailsToOutput)
        {
            if (Report.Failed > 0)
            {
                GA.AddError("TestNg Execution Failed");
            }
            GA.AddOutput("Passed", Report.Passed, "TEST");
            GA.AddOutput("Failed", Report.Failed);
            GA.AddOutput("Ignored", Report.Ignored);
            GA.AddOutput("Skipped", Report.Skipped);

            foreach (TestNGSuite TNS in Report.Suites)
            {
                GA.AddOutput(TNS.Name, TNS.Duration, "Duration");
                foreach (TestNGTest Ntest in TNS.Tests)
                {
                    string TestPreFix = TNS.Name + "|" + Ntest.Name;

                    GA.AddOutput(TestPreFix, Ntest.ExecutionDurationMS, "Duration");
                    if (!String.IsNullOrEmpty(Ntest.ExecutionExceptionMsg))
                    {
                        GA.AddExInfo(Environment.NewLine);
                        GA.AddExInfo(TestPreFix + " NG Exception" + Environment.NewLine);
                        GA.AddExInfo(Ntest.ExecutionExceptionMsg);
                        GA.AddExInfo(Environment.NewLine);
                    }
                    if (!String.IsNullOrEmpty(Ntest.ExecutionExceptionStackTrace))
                    {
                        GA.AddExInfo(Environment.NewLine);
                        GA.AddExInfo(TestPreFix + " NG StackTrace" + Environment.NewLine);
                        GA.AddExInfo(Ntest.ExecutionExceptionStackTrace);
                        GA.AddExInfo(Environment.NewLine);
                    }

                    if (AddMethoudDetailsToOutput)
                    {
                        foreach (TestNGTestClass NClass in Ntest.Classes)
                        {
                            foreach (TestNGTestMethod Nm in NClass.Methods)
                            {
                                string MethodPrefix = TestPreFix + "|" + NClass.Name + "|" + Nm.Name;
                                GA.AddOutput(MethodPrefix, Nm.ExecutionStatus, "Status");
                                GA.AddOutput(MethodPrefix, Nm.ExecutionDurationMS, "Duration");

                            }
                        }
                    }
                }

            }

        }
    }
}