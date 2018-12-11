using Amdocs.Ginger.Plugin.Core;
using System;

namespace GingerTestNgPluginConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Ginger TestNg Plugin");

            using (GingerNodeStarter gingerNodeStarter = new GingerNodeStarter())
            {               
                gingerNodeStarter.StartNode("TestNG Execution Service", new TestNGExecuterService());
                gingerNodeStarter.Listen();
            }
        }
    }
}
