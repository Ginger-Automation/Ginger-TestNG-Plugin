using System;

namespace GingerTestNgPlugin
{
    public class TestNGTestMethod
    {
        public string Name;

        public eTestExecutionStatus ExecutionStatus= eTestExecutionStatus.NA;
        public DateTime ExecutionStartTime;
        public DateTime ExecutionEndTime;
        public int ExecutionDurationMS;
        public string ExecutionSignature;
        public TestNGTestException ExecutionException;
    }
}
