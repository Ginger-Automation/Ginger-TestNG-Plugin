using System;
using System.Collections.Generic;
using static GingerTestNgPlugin.NGProperties;

namespace GingerTestNgPlugin
{
    public enum eTestExecutionStatus
    {
        NA,
        PASS,
        FAIL,
        SKIP
    }

    public class TestNGTest
    {
        public string Name { get; set; } //using get;set; this attribute will be shown as input value in Ginger side
        public List<TestNGTestClass> Classes;
        public List<TestNGTestParameter> Parameters;

        public eTestExecutionStatus ExecutionStatus;
        public DateTime ExecutionStartTime;
        public DateTime ExecutionEndTime;
        public int ExecutionDurationMS;
        public string ExecutionExceptionMsg;
        public string ExecutionExceptionStackTrace;
    }
}