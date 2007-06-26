using System;
using System.Diagnostics;

using TeamAgile.ApplicationBlocks.Interception;

namespace HK.Testing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ExitTestOnDemandAttribute : BaseProcessingAttribute
    {
        [DebuggerStepThrough]
        protected override void OnPreProcess(object sender, PreProcessEventArgs args)
        {
            MultiThreadedTestCase testCase = (MultiThreadedTestCase)args.TargetObject;
            testCase._BeginTest(args.MethodCallMessage.MethodName);
        }

        [DebuggerStepThrough]
        protected override void OnPostProcess(object sender, PostProcessEventArgs args)
        {
            MultiThreadedTestCase testCase = (MultiThreadedTestCase)args.TargetObject;

            int timeoutInSeconds = 0;

            if (this is ExitTestOnTimeOutAttribute) //works also for FailTestOnTimeOutAttribute
            {
                ExitTestOnTimeOutAttribute timeoutAttribute = (ExitTestOnTimeOutAttribute)this;
                timeoutInSeconds = timeoutAttribute.Timeout;
            }
            bool failOnTimeout = this is ExitTestOnTimeOutAttribute;
            testCase._EndTest(timeoutInSeconds, failOnTimeout);
        }
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ExitTestOnTimeOutAttribute : ExitTestOnDemandAttribute
    {
        public int _timoutInSeconds;

        public ExitTestOnTimeOutAttribute(int timeoutInSeconds)
        {
            _timoutInSeconds = timeoutInSeconds;
        }

        public int Timeout
        {
            get { return _timoutInSeconds; }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FailTestOnTimeOutAttribute : ExitTestOnTimeOutAttribute
    {
        public FailTestOnTimeOutAttribute(int timeoutInSeconds)
            : base(timeoutInSeconds)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SucceedTestOnTimeOutAttribute : ExitTestOnTimeOutAttribute
    {
        public SucceedTestOnTimeOutAttribute(int timeoutInSeconds)
            : base(timeoutInSeconds)
        {
        }
    }

}
