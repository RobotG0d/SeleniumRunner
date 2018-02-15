using System;

namespace SeleniumRunner.Model.Entities
{
    public class TestReport
    {
        public Test Test { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public Exception Exception { get; set; }

        public bool Success => Exception == null;

        public TestReport(Test test, TimeSpan timeSpan)
        {
            Test = test;
            TimeSpan = timeSpan;
        }

        public TestReport(Test test, TimeSpan timeSpan, Exception exception) : this(test, timeSpan)
        {
            Exception = exception;
        }
    }
}
