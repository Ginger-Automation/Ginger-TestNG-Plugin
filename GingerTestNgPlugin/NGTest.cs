using System;
using System.Collections.Generic;
using static GingerTestNgPlugin.NGProperties;

namespace GingerTestNgPlugin
{
    public class NGTest
    {
        public List<NGClass> Classes;
        public List<NGParameter> Parameters;
        public string Name;
        public string Signature;
        public int Duration;
        public DateTime StartedAt;
        public DateTime FinishedAt;
        public string NgException;
        public string NGStackTrace;



    }
}