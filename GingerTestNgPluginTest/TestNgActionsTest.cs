using GingerTestHelper;
using GingerTestNgPlugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GingerTestNgPluginTest
{
    [TestClass]
    public class TestNgActionsTest
    {
        [TestMethod]
        public void ReadTestNgXml()
        {
            string TestNgXmll = TestResources.GetTestResourcesFile(@"TestNGxml.xml");

            TestNGSuite Parser = new TestNGSuite(TestNgXmll);

            Assert.AreEqual(3,Parser.Tests.Count);
        }

        [TestMethod]
        public void GenerateModifiedXml()
        {
            string TestNgXmll = System.IO.File.ReadAllText(@"C:\sandbox\TestNGxml.xml");

             TestNGSuite Suite = new TestNGSuite(TestNgXmll);
            
            string xml = Suite.GetTextNGXml();
           
        }

        public void Execute()
        {
            string TestNgXmll = System.IO.File.ReadAllText(@"C:\sandbox\TestNGxml.xml");

            TestNGSuite Suite = new TestNGSuite(TestNgXmll);
            string xml = Suite.GetTextNGXml();

            Suite.Execute("", "", @"C:\Users\mohdkhan\eclipse-workspace\TestNGDemo\testng.xml");
        }
    }
}
