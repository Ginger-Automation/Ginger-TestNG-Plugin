using GingerTestHelper;
using GingerTestNgPlugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GingerTestNgPluginTest
{
    [TestClass]
    public class TestNGReportTests
    {
        [TestMethod]
        public void ReadTestNgReport()
        {
            string ReportXMl = TestResources.GetTestResourcesFile(@"Test-Results.xml");

           TestNGReport Parser = TestNGReport.LoadfromXMl(ReportXMl);

        }

    }
}
