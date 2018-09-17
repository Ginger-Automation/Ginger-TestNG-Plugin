using Amdocs.Ginger.CoreNET.Drivers.CommunicationProtocol;
using Amdocs.Ginger.Plugin.Core;
using GingerCoreNET.DriversLib;
using GingerTestNgPlugin;
using StandAloneActions;
using System;

namespace GingerTestNgPluginConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting PACT Plugin");
           // PACTService s = new PACTService();
            GingerAction GA = new GingerAction();

            Console.WriteLine("Done!");
            TestNgAction ta = new TestNgAction();
            ta.ReadExcelCell(GA, "tst");

            GingerNode gingerNode = new GingerNode(new TestNgAction());
            gingerNode.StartGingerNode("TestNg1 1", SocketHelper.GetLocalHostIP(), 15001);
            gingerNode.GingerNodeMessage += GingerNode_GingerNodeMessage;
            /*test def
            PACTEditor2 editor = new PACTEditor2();
            var v = editor.HighlightingDefinition;
    
            TODO: Wait for?
                    */
            TestNGSuite Parser = new TestNGSuite(@"C:\sandbox\TestNGxml.xml");
            Console.ReadKey();
        }

        private static void GingerNode_GingerNodeMessage(GingerNode gingerNode, GingerNode.eGingerNodeEventType GingerNodeEventType)
        {
            
        }
    }
}
