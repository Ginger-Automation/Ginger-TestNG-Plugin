﻿using Amdocs.Ginger.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GingerTestNgPluginConsole
{
    public class General
    {
        public static void AddErrorToConsoleAndAction(IGingerAction gingerAction, string error)
        {
            gingerAction.AddError(error);
            Console.WriteLine(string.Format("ERROR: {0}", error));
        }

        public static void AddInfoToConsoleAndAction(IGingerAction gingerAction, string message)
        {
            gingerAction.AddExInfo(message);
            Console.WriteLine(string.Format("INFO: {0}", message));
        }

        public static void AddInfoToConsole(string message)
        {
            Console.WriteLine(string.Format("INFO: {0}", message));
        }

        public static string TrimApostrophes(string str)
        {
            string customStr = str;
            if (!string.IsNullOrEmpty(customStr))
            {
                customStr = customStr.TrimStart('\"');
                customStr = customStr.TrimEnd('\"');
            }

            return customStr;
        }

        public static string TrimRelativeSleshes(string str)
        {
            string customStr = str;
            if (!string.IsNullOrEmpty(customStr))
            {
                customStr = customStr.TrimStart(new char[] { '\\', '/' });
            }

            return customStr;
        }

        public static string TrimEndSleshes(string str)
        {
            string customStr = str;
            if (!string.IsNullOrEmpty(customStr))
            {
                customStr = customStr.TrimEnd(new char[] { '\\', '/' });
            }

            return customStr;
        }
    }
}
