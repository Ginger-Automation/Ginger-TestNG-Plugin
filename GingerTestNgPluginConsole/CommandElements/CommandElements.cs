using System;
using System.Collections.Generic;
using System.Text;

namespace GingerTestNgPluginConsole
{
    public class CommandElements
    {
        public string WorkingFolder;
        public string ExecuterFilePath;
        public string Arguments;

        public string FullCommand
        {
            get
            {
                if (WorkingFolder == null)
                {
                    return string.Format("{0}{1}", ExecuterFilePath, Arguments);
                }
                else
                {
                    return string.Format("{0}>{1} {2}", WorkingFolder, ExecuterFilePath, Arguments);
                }
            }
        }
    }
}
