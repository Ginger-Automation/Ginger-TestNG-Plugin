using GingerTestNgPlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace GingerTestNgPlugin
{
    public class TestNGTestSuite
    {
        public string Name;
        public List<TestNGTestParameter> Parameters;
        public List<TestNGListener> Listeners;
        public List<TestNGTest> Tests;

        public DateTime ExecutionStartTime;
        DateTime ExecutionEndTime;
        public Int32 ExecutionDurationMS;
    }
}
