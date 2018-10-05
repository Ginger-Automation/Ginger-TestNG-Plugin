using Amdocs.Ginger.Plugin.Core;
using GingerTestHelper;
using GingerTestNgPlugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StandAloneActions;
using System;
using System.IO;
namespace GingerTestNgPluginTest
{
    [TestClass]
    public class TestNgActionsTest
    {
        [TestMethod]
        public void Sandbox()
        {
            var abc = System.IO.Path.GetFullPath("mvn");
            string commandline = @"mvn -s cfg/maven/CI/settings.xml test -e -U -DsuiteToRun=/src/test/resources/fit/executionSuites/oc_mat.xml -DthreadPoolSize=1  -Dopenshift=true -Dhost=ilocpde01-master.corp.amdocs.com -Dport=8443 -DocNameSpace=oc-cd-1091-ankitad-0265 -DcareNameSpace=care-for-openshift-testing -Dtoken=eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJrdWJlcm5ldGVzL3NlcnZpY2VhY2NvdW50Iiwia3ViZXJuZXRlcy5pby9zZXJ2aWNlYWNjb3VudC9uYW1lc3BhY2UiOiJvYy1jZC0xMDkxLWFua2l0YWQtMDI2NSIsImt1YmVybmV0ZXMuaW8vc2VydmljZWFjY291bnQvc2VjcmV0Lm5hbWUiOiJmaXQtc2EtdG9rZW4tc21rbnQiLCJrdWJlcm5ldGVzLmlvL3NlcnZpY2VhY2NvdW50L3NlcnZpY2UtYWNjb3VudC5uYW1lIjoiZml0LXNhIiwia3ViZXJuZXRlcy5pby9zZXJ2aWNlYWNjb3VudC9zZXJ2aWNlLWFjY291bnQudWlkIjoiMDE1YmFmMDktYmJlMC0xMWU4LWI5YzUtMDA1MDU2OWFjMDFkIiwic3ViIjoic3lzdGVtOnNlcnZpY2VhY2NvdW50Om9jLWNkLTEwOTEtYW5raXRhZC0wMjY1OmZpdC1zYSJ9.18jQg5JCcJNnWbCMgLkf_zttWgOuS9e-FiEDCjGrrJIjefwb3lwkEvbnhaSFZHi4w_T5HqSVe4Uaqd5N88CyKRi6IP5UyI-wUKGtgZbmamcplri2GyeAWRxhBxEE4-cOfle6sK9fRyBRav3cw80p9UzVgvGipW9Q5ZXCjxSj-IllbMiOg7jsPMiAJ21hAQJsmMc-SPhnmRi0GTS-DcK_n1DyqdBfjQNZrTLw-M-hpcW4oSGkYJCGTL6SOp_ybhR2S5qxmVlwe9wfYzZGZFsbrKpR-YlLm9uZRh-31BkN0Fs2Oc8o2i5gRYo29SgEPc2VupFU6dJuOTwx47qIaNIRAg";
            string FilePath = commandline;
            if (commandline.Contains(""))
            {
                string[] FileCommand = commandline.Split(" ", 2);
                FilePath = FileCommand[0];
                commandline = FileCommand[1];
            }

            if (!File.Exists(FilePath))
            {
                foreach (string DirPath in Environment.GetEnvironmentVariable("PATH").Split(";"))
                {
                    string Dir = DirPath.EndsWith("\\") ? DirPath : DirPath + "\\";
                    string TempPath = Dir + FilePath;
                    if (File.Exists(TempPath))
                    {
                        FilePath = TempPath;
                        break;
                    }

                }
            }
        }


        [TestMethod]
        public void ReadTestNgXml()
        {
            string TestNgXmll = TestResources.GetTestResourcesFile(@"TestNGxml.xml");

            TestNGSuite Parser = new TestNGSuite(System.IO.File.ReadAllText(TestNgXmll));

            Assert.AreEqual(3, Parser.Tests.Count);
        }

        [TestMethod]
        public void GenerateModifiedXml()
        {
            string TestNgXmll = System.IO.File.ReadAllText(@"C:\sandbox\TestNGxml.xml");

            TestNGSuite Suite = new TestNGSuite(TestNgXmll);

            string xml = Suite.GetTextNGXml();

        }
        [TestMethod]
        public void ExecuteWithXML()
        {
            string TestNgXmll = System.IO.File.ReadAllText(@"C:\Users\mohdkhan\eclipse-workspace\SeleniumTestNg\testng.xml");

            TestNGSuite Suite = new TestNGSuite(TestNgXmll);
            string xml = Suite.GetTextNGXml();


            TestNGReport Report = TestNGSuite.Execute("testng.xml", @"C:\Users\mohdkhan\eclipse-workspace\SeleniumTestNg", @"C:\eclipse\plugins", "");
        }

        [TestMethod]
        public void ExecuteCommandline()
        {
            string FreeCommand = @"mvn -s cfg/maven/CI/settings.xml test -e -U -DsuiteToRun=/src/test/resources/fit/executionSuites/oc_mat.xml -DthreadPoolSize=1  -Dopenshift=true -Dhost=ilocpde01-master.corp.amdocs.com -Dport=8443 -DocNameSpace=oc-cd-1091-ankitad-0265 -DcareNameSpace=care-for-openshift-testing -Dtoken=eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJrdWJlcm5ldGVzL3NlcnZpY2VhY2NvdW50Iiwia3ViZXJuZXRlcy5pby9zZXJ2aWNlYWNjb3VudC9uYW1lc3BhY2UiOiJvYy1jZC0xMDkxLWFua2l0YWQtMDI2NSIsImt1YmVybmV0ZXMuaW8vc2VydmljZWFjY291bnQvc2VjcmV0Lm5hbWUiOiJmaXQtc2EtdG9rZW4tc21rbnQiLCJrdWJlcm5ldGVzLmlvL3NlcnZpY2VhY2NvdW50L3NlcnZpY2UtYWNjb3VudC5uYW1lIjoiZml0LXNhIiwia3ViZXJuZXRlcy5pby9zZXJ2aWNlYWNjb3VudC9zZXJ2aWNlLWFjY291bnQudWlkIjoiMDE1YmFmMDktYmJlMC0xMWU4LWI5YzUtMDA1MDU2OWFjMDFkIiwic3ViIjoic3lzdGVtOnNlcnZpY2VhY2NvdW50Om9jLWNkLTEwOTEtYW5raXRhZC0wMjY1OmZpdC1zYSJ9.18jQg5JCcJNnWbCMgLkf_zttWgOuS9e-FiEDCjGrrJIjefwb3lwkEvbnhaSFZHi4w_T5HqSVe4Uaqd5N88CyKRi6IP5UyI-wUKGtgZbmamcplri2GyeAWRxhBxEE4-cOfle6sK9fRyBRav3cw80p9UzVgvGipW9Q5ZXCjxSj-IllbMiOg7jsPMiAJ21hAQJsmMc-SPhnmRi0GTS-DcK_n1DyqdBfjQNZrTLw-M-hpcW4oSGkYJCGTL6SOp_ybhR2S5qxmVlwe9wfYzZGZFsbrKpR-YlLm9uZRh-31BkN0Fs2Oc8o2i5gRYo29SgEPc2VupFU6dJuOTwx47qIaNIRAg";
            string PWD = @"C:\sandbox\order_capture_test";
            TestNGReport TR = TestNGSuite.Execute(FreeCommand, PWD, @"target\surefire-reports");


        }

        [TestMethod]
        public void ExecuteActionTest()
        {
            GingerAction GA = new GingerAction();
            TestNgService TNA = new TestNgService();
        
            string TestNgXmll = System.IO.File.ReadAllText(@"C:\Users\mohdkhan\eclipse-workspace\SeleniumTestNg\testng.xml");

            TestNGSuite Suite = new TestNGSuite(TestNgXmll);
            string xml = Suite.GetTextNGXml();
            TNA.RunTestNgSuite(GA, @"testng.xml", @"C:\Users\mohdkhan\eclipse-workspace\SeleniumTestNg", @"C:\eclipse\plugins",String.Empty,"True");

            TestNGReport Report = TestNGSuite.Execute("testng.xml", @"C:\Users\mohdkhan\eclipse-workspace\SeleniumTestNg", @"C:\eclipse\plugins", "");
        }
    }
}
