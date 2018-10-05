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
            
            GingerNodeStarter.StartNode(new TestNgService(), "TestNg Service 1");

            Console.ReadKey();
           
        }


    }
}
