using Amdocs.Ginger.Plugin.Core;
using GingerTestNgPluginConsole;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GingerTestNgPluginTest
{
    [TestClass]
    public class TestNGExecuterServiceConfigurationsTest
    {
        /// <summary>
        /// Check that solution takes Java path from system environment JAVA_HOME (need to be added as prerequisite) 
        /// </summary>
        [TestMethod]
        public void JavaExePathTest1()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;

            //Act
            testNgExecuter.JavaExeFullPath = null;

            //Assert           
            Assert.IsTrue(testNgExecuter.JavaExeFullPath != null, string.Format("JavaExeFullPath- '{0}'-  is not Null", testNgExecuter.JavaExeFullPath));
            Assert.IsTrue(Path.GetDirectoryName(testNgExecuter.JavaExeFullPath).ToLower().Contains(@"bin"), string.Format("JavaExeFullPath- '{0}'-  pointing to bin folder", testNgExecuter.JavaExeFullPath) );
            Assert.IsTrue(Path.GetFileName(testNgExecuter.JavaExeFullPath).ToLower().Contains("java"), string.Format("JavaExeFullPath- '{0}'- pointing to java.exe file", testNgExecuter.JavaExeFullPath) );
            Assert.IsTrue(File.Exists(testNgExecuter.JavaExeFullPath), string.Format("JavaExeFullPath- '{0}'- pointing to java.exe file which exist", testNgExecuter.JavaExeFullPath) );
        }

        [TestMethod]
        public void JavaExePathTest2()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;

            //Act
            testNgExecuter.JavaExeFullPath = "\"C:\\Program Files\\Java\\jdk1.8.0_191\"";

            //Assert                  
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.JavaExeFullPath, @"C:\Program Files\Java\jdk1.8.0_191\bin\java", General.eCompareType.Contains), string.Format("JavaExeFullPath '{0}' pointing to bin\\java file", testNgExecuter.JavaExeFullPath));
        }

        [TestMethod]
        public void JavaProjectBinPathTest()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;

            //Act
            testNgExecuter.JavaProjectBinPath = @"C:\Users\menik\eclipse-workspace\Learn-TestNG";

            //Assert           
            Assert.IsTrue(testNgExecuter.JavaProjectBinPath.ToLower().Contains(@"bin"), "JavaProjectBinPath pointing to bin folder");
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.JavaProjectBinPath, @"C:\Users\menik\eclipse-workspace\Learn-TestNG\bin", General.eCompareType.Equal), "JavaProjectBinPath pointing to bin folder");
        }

        [TestMethod]
        public void JavaProjectResourcesPathTest()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;

            //Act
            testNgExecuter.JavaProjectResourcesPath = @"C:\TestNG\";

            //Assert           
            Assert.IsTrue(Path.GetFileName(testNgExecuter.JavaProjectResourcesPath).ToLower() == "*", string.Format("JavaProjectResourcesPath '{0}' having *", testNgExecuter.JavaProjectResourcesPath));
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.JavaProjectResourcesPath, @"C:\TestNG\*", General.eCompareType.Equal), string.Format("JavaProjectResourcesPath '{0}' having path with *", testNgExecuter.JavaProjectResourcesPath));
        }

        /// <summary>
        /// Check that solution takes Mvn path from system environment MAVEN_HOME (need to be added as prerequisite) 
        /// </summary>
        [TestMethod]
        public void MavenCmdPathTest1()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Maven;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;

            //Act
            testNgExecuter.MavenCmdFullPath = null;

            //Assert           
            Assert.IsTrue(testNgExecuter.MavenCmdFullPath != null, "MavenCmdFullPath is not Null");
            Assert.IsTrue(Path.GetDirectoryName(testNgExecuter.MavenCmdFullPath).ToLower().Contains(@"bin"), "MavenCmdFullPath pointing to bin folder");
            Assert.IsTrue(Path.GetFileName(testNgExecuter.MavenCmdFullPath).ToLower().Contains("mvn"), "MavenCmdFullPath pointing to java.exe file");
            Assert.IsTrue(File.Exists(testNgExecuter.MavenCmdFullPath), "MavenCmdFullPath pointing to mvn file which exist");
        }

        [TestMethod]
        public void MavenCmdPathTest2()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Maven;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.XML;

            //Act
            testNgExecuter.MavenCmdFullPath = @"C:\TestNG\PBG Flows\apache-maven-3.5.3";

            //Assert           
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.MavenCmdFullPath, @"C:\TestNG\PBG Flows\apache-maven-3.5.3\bin\mvn", General.eCompareType.Contains), "MavenCmdFullPath pointing to bin\\mvn.cmd file");
        }

        [TestMethod]
        public void FreeCommandArgumentsJavaTest()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.FreeCommand;

            //Act
            testNgExecuter.FreeCommandArguments = @"java -Dhost=openshift-master.d1liveaws.corp.amdocs.com -Dopenshift=true -Dport=443 -DocNameSpace=ordercapture-integrated-env-aws -DcareNameSpace=ordercapture-integrated-env-aws -Dtoken=$() -cp . org.testng.TestNG src/test/resources/fit/executionSuites/oc_mat.xml";

            //Assert           
            Assert.IsTrue(testNgExecuter.FreeCommandArguments == @"-Dhost=openshift-master.d1liveaws.corp.amdocs.com -Dopenshift=true -Dport=443 -DocNameSpace=ordercapture-integrated-env-aws -DcareNameSpace=ordercapture-integrated-env-aws -Dtoken=$() -cp . org.testng.TestNG src/test/resources/fit/executionSuites/oc_mat.xml", "FreeCommandArguments remove 'java' addition");
        }

        [TestMethod]
        public void FreeCommandArgumentsMavenTest()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Maven;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;
            testNgExecuter.ExecutionMode = TestNGExecution.eExecutionMode.FreeCommand;

            //Act
            testNgExecuter.FreeCommandArguments = @"mvn -s cfg/maven/CI/settings.xml test -e -U -DsuiteToRun=/src/test/resources/fit/executionSuites/oc_mat.xml";

            //Assert           
            Assert.IsTrue(testNgExecuter.FreeCommandArguments == @"-s cfg/maven/CI/settings.xml test -e -U -DsuiteToRun=/src/test/resources/fit/executionSuites/oc_mat.xml", "FreeCommandArguments remove 'mvn' addition");
        }

        [TestMethod]
        public void RelativeTestNGXmlPathWithJavaTest()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;

            //Act
            testNgExecuter.JavaProjectBinPath = @"C:\Users\menik\eclipse-workspace\Learn-TestNG\bin";
            testNgExecuter.TestngXmlPath = "\"\\src\\Calculator\\testng.xml\"";

            //Assert                  
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.TestngXmlPath, @"C:\Users\menik\eclipse-workspace\Learn-TestNG\src\Calculator\testng.xml", General.eCompareType.Equal), string.Format("Relative TestngXmlPath '{0}' been converted to full path", testNgExecuter.TestngXmlPath));
        }

        [TestMethod]
        public void RelativeTestNGXmlPathWithJavaTest2()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;

            //Act
            testNgExecuter.JavaProjectBinPath = @"C:\Users\menik\eclipse-workspace\Learn-TestNG";
            testNgExecuter.TestngXmlPath = @"src\Calculator\testng.xml";

            //Assert                  
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.TestngXmlPath, @"C:\Users\menik\eclipse-workspace\Learn-TestNG\src\Calculator\testng.xml", General.eCompareType.Equal), "Relative TestngXmlPath been converted to full path");
        }

        [TestMethod]
        public void RelativeTestNGXmlPathWithJavaTest3()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;

            //Act
            testNgExecuter.JavaProjectBinPath = "\"C:\\Users\\menik\\eclipse-workspace\\Learn-TestNG\\bin\\\"";
            testNgExecuter.TestngXmlPath = @"src\Calculator\testng.xml";

            //Assert                  
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.TestngXmlPath, @"C:\Users\menik\eclipse-workspace\Learn-TestNG\src\Calculator\testng.xml", General.eCompareType.Equal), "Relative TestngXmlPath been converted to full path");
        }

        [TestMethod]
        public void RelativeTestNGXmlPathWithMavenTest()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Maven;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;

            //Act
            testNgExecuter.MavenProjectFolderPath = @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test";
            testNgExecuter.TestngXmlPath = @"\src\test\resources\fit\flowSuites\Dynamic Device from CouchBase.xml";

            //Assert                  
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.TestngXmlPath, @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test\src\test\resources\fit\flowSuites\Dynamic Device from CouchBase.xml", General.eCompareType.Equal), "Relative TestngXmlPath been converted to full path");
        }

        [TestMethod]
        public void RelativeTestNGXmlPathWithMavenTest2()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Maven;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;

            //Act
            testNgExecuter.MavenProjectFolderPath = @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test";
            testNgExecuter.TestngXmlPath = @"src\test\resources\fit\flowSuites\Dynamic Device from CouchBase.xml";

            //Assert                  
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.TestngXmlPath, @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test\src\test\resources\fit\flowSuites\Dynamic Device from CouchBase.xml", General.eCompareType.Equal), "Relative TestngXmlPath been converted to full path");
        }

        [TestMethod]
        public void RelativeTestngResultsXmlFolderPathWithJavaTest()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Java;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Regular;

            //Act
            testNgExecuter.JavaProjectBinPath = @"C:\Users\menik\eclipse-workspace\Learn-TestNG\bin";
            testNgExecuter.TestngResultsXmlFolderPath = @"\report";

            //Assert                  
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.TestngResultsXmlFolderPath,  @"C:\Users\menik\eclipse-workspace\Learn-TestNG\report", General.eCompareType.Equal), string.Format("Relative TestngResultsXmlFolderPath been converted to full path: '{0}'", testNgExecuter.TestngResultsXmlFolderPath));
        }

        [TestMethod]
        public void RelativeTestngResultsXmlFolderPathWithMavenTest()
        {
            //Arrange
            TestNGExecution testNgExecuter = new TestNGExecution();
            testNgExecuter.ExecuterType = TestNGExecution.eExecuterType.Maven;
            testNgExecuter.JavaProjectType = TestNGExecution.eJavaProjectType.Maven;

            //Act
            testNgExecuter.MavenProjectFolderPath = @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test";
            testNgExecuter.TestngResultsXmlFolderPath = @"\report";

            //Assert                  
            Assert.IsTrue(General.CompareWithoutSleshSensitivity(testNgExecuter.TestngResultsXmlFolderPath, @"C:\TestNG_WORKSPACE\PBG Flows\order_capture_test\report", General.eCompareType.Equal), "Relative TestngResultsXmlFolderPath been converted to full path");
        }
    }
}
