using Amdocs.Ginger.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StandAloneActions
{
    [GingerService("TestNG", "Execute TestNG scripts from Ginger")]
    public class TestNgAction : IGingerService
    {
        [GingerAction("ReadExcelCell", "Read From Excel")]
    /*    public void ReadExcelCell(GingerAction GA,List<Tuple<string,string>> TextToreplace, string FileName, string testNglocation, string Java)
        {
            
        }*/
        public void ReadExcelCell(GingerAction GA,string tst)
        {
            GA.AddOutput(" tst",tst);

        }
    }
}
