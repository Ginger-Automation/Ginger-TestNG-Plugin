using Amdocs.Ginger.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using GingerTestNgPlugin;

namespace StandAloneActions
{
    [GingerService("TestNG", "Execute TestNG scripts from Ginger")]
    public class TestNgAction : IStandAloneAction,IGingerService
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

        public void RunTestNgSuite(IGingerAction GA, string TestNgXMlName, string ProjectLocation, string LibraryFolder, string JavaLocation)
        {
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
            
                    Report = TestNGSuite.Execute(TestNgXMlName, ProjectLocation, LibraryFolder, "");
       

             

                ProcessTestNGReport(GA, Report);
            }





            catch (Exception EX)
            {
                GA.AddError("TestNg Execution Failed");
                GA.AddExInfo(EX.Message);

            }
        }

        [GingerAction("Run TestNg with Maven", "Run TestNg suites as part of Maven Build with Surefire")]
        public void RunTestNgWithMaven(IGingerAction GA, string TestNgSuiteXML, string WorkingDirectory, string MavenBinDirectory,string MavenSettingsFile, string Commandlinearguments)
        {
            StringBuilder FreeCommand = new StringBuilder("");
            if(!string.IsNullOrEmpty(MavenBinDirectory))
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
                FreeCommand.Append(" -s "+MavenSettingsFile);
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
                FreeCommand.Append(Commandlinearguments);
            }

            TestNGReport Report = TestNGSuite.Execute(FreeCommand.ToString(), WorkingDirectory, @"target\surefire-reports");
            ProcessTestNGReport(GA, Report);
        }


        [GingerAction("Run TestNg with Free COmmand", "Run TestNg with Free COmmand")]
        public void RunTestNgFreeCommand(IGingerAction GA, string Freecommand, string WorkingDirectory, string ReportsDirectory, string MavenSettingsFile, string Commandlinearguments)
        {
            TestNGReport Report = TestNGSuite.Execute(Freecommand, WorkingDirectory,ReportsDirectory);
            ProcessTestNGReport(GA, Report);
        }

            /// <summary>
            /// Process TestNgReport object and update the action with outcome
            /// </summary>
            /// <param name="GA"></param>
            /// <param name="Report"></param>
            public static void ProcessTestNGReport(IGingerAction GA, TestNGReport Report)
        {
            if (Report.Failed > 0)
            {
                GA.AddError("TestNg Execution Failed");
            }
            GA.AddOutput("Passed", Report.Passed);
            GA.AddOutput("Failed", Report.Failed);
            GA.AddOutput("Ignored", Report.Ignored);
            GA.AddOutput("Skipped", Report.Skipped);

            foreach (TestNGSuite TNS in Report.Suites)
            {
                GA.AddOutput(TNS.Name + "-Duration", TNS.Duration);
                foreach (NGTest Ntest in TNS.Tests)
                {

                    string TestPreFix = TNS.Name + "|" + Ntest.Name;

                    GA.AddOutput(TestPreFix + "-Duration", Ntest.Duration);
                    if (!String.IsNullOrEmpty(Ntest.NgException))
                    {
                        GA.AddExInfo(Environment.NewLine);
                        GA.AddExInfo(TestPreFix + " NG Exception" + Environment.NewLine);
                        GA.AddExInfo(Ntest.NgException);
                        GA.AddExInfo(Environment.NewLine);
                    }
                    if (!String.IsNullOrEmpty(Ntest.NGStackTrace))
                    {
                        GA.AddExInfo(Environment.NewLine);
                        GA.AddExInfo(TestPreFix + " NG StackTrace" + Environment.NewLine);
                        GA.AddExInfo(Ntest.NGStackTrace);
                        GA.AddExInfo(Environment.NewLine);
                    }
                    foreach (NGClass NClass in Ntest.Classes)
                    {


                        foreach (NGMethod Nm in NClass.Methods)
                        {
                            string MethodPrefix = TestPreFix + "|" + NClass.Name + "|" + Nm.Name;

                            GA.AddOutput(MethodPrefix, Nm.Status);
                            GA.AddOutput(MethodPrefix + "-Duration", Nm.Duration);

                        }
                    }
                }


            }

        }
    }
}