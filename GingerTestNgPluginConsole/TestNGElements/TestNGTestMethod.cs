using System;
using System.Collections.Generic;
using System.Text;
using static GingerTestNgPlugin.NGProperties;

namespace GingerTestNgPlugin
{
    public class TestNGTestMethod
    {
        public string Name;

        public eTestExecutionStatus ExecutionStatus;
        public DateTime ExecutionStartTime;
        public DateTime ExecutionEndTime;
        public int ExecutionDurationMS;
        public string ExecutionSignature;
    }
}
