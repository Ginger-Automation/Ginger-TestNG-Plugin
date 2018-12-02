using Amdocs.Ginger.Plugin.Core;
using StandAloneActions;
using System;

namespace GingerTestNgPluginConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Ginger TestNg Plugin");

            //GingerNodeStarter.StartNode(new TestNGExecuterService(), "TestNG Execution Service"); //TODO: Fix StartNode after updating Ginger.Plugin.Core

            Console.ReadKey();
           
        }


    }
}
