using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace GingerTestNgPlugin
{
   public class TestNGReport
    {
        public static TestNGReport LoadfromXMl(string xmlfilepath)
        {
            return new TestNGReport(File.ReadAllText(xmlfilepath));
      
        }
        public readonly int Failed;
        public readonly int Passed;
        public readonly int Skipped;
        public readonly int Ignored;
        public readonly List<TestNGSuite> Suites;

        private TestNGReport(string ReportXMl)
        {
            XmlDocument NGReport = new XmlDocument();
            NGReport.LoadXml(ReportXMl);
            Suites = new List<TestNGSuite>();

          Int32.TryParse(NGReport.DocumentElement.GetAttribute("skipped").ToString(), out Passed);
           Int32.TryParse(NGReport.DocumentElement.GetAttribute("failed").ToString(), out Failed);
         Int32.TryParse(NGReport.DocumentElement.GetAttribute("passed").ToString(),out Skipped);
            Int32.TryParse(NGReport.DocumentElement.GetAttribute("ignored").ToString(),out Ignored);

            foreach (XmlElement Suite in NGReport.GetElementsByTagName("suite"))
            {
                TestNGSuite NGSuite = TestNGSuite.LoadFromReport(Suite.OuterXml);
                Suites.Add(NGSuite);
            }

        }
    }
}
