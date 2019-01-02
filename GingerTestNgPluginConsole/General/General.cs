using Amdocs.Ginger.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        public static bool OutputParamExist(GingerAction GA, string paramName, string paramValue = null)
        {
            IGingerActionOutputValue val = null;
            if (paramValue == null)
            {
                val = GA.Output.OutputValues.Where(x => x.Param == paramName).FirstOrDefault();
            }
            else
            {
                val = GA.Output.OutputValues.Where(x => x.Param == paramName && x.Value.ToString() == paramValue).FirstOrDefault();
            }

            if (val == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public enum eCompareType { Equal, Contains }
        public static bool CompareWithoutSleshSensitivity(string string1, string string2, eCompareType compareType)
        {
            int counter = 0;
            while (counter <=1)
            {
                if (counter == 0)
                {
                    string1 = string1.Replace('\\', '/');
                    string2 = string2.Replace('\\', '/');
                }
                else
                {
                    string1 = string1.Replace('/', '\\');
                    string2 = string2.Replace('/', '\\');
                }
                
                if (compareType == eCompareType.Equal)
                {
                    if (string1 == string2)
                    {
                        return true;
                    }
                }
                else
                {
                    if (string1.Contains(string2))
                    {
                        return true;
                    }
                }

                counter++;
            }
            return false;
        }

        public static string GetOSFoldersSeperator()
        {
            string foldersSeperator;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foldersSeperator = ";";
            }
            else//Linux
            {
                foldersSeperator = ":";
            }

            return foldersSeperator;
        }
    }
}
