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
            Console.WriteLine("Starting Ginger TestNg Plugin");
           // PACTService s = new PACTService();
            GingerAction GA = new GingerAction();

            Console.WriteLine("Done!");
            TestNgAction ta = new TestNgAction();
            

            GingerNode gingerNode = new GingerNode(new TestNgAction());
           gingerNode.StartGingerNode("TestNg1 1", SocketHelper.GetLocalHostIP(), 15001);
            Console.ReadLine();
           
        }


    }
}
