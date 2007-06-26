using System;
using System.Diagnostics;
using System.Threading;

using HK.Common;

using TeamAgile.ApplicationBlocks.Interception;

namespace HK.Testing
{
    [NUnit.Framework.TestFixture]
    [DebuggerStepThrough]
    public abstract class MultiThreadedTestCase : InterceptableObject
    {
        private Exception _exception;
        private AutoResetEvent _autoResetEvent;
        private bool _testSucceeded;
        private const string _category = "TestRunner";

        protected Log _log;

        [NUnit.Framework.SetUp]
        public virtual void BeforeEachTestRun()
        {
        }

        [NUnit.Framework.TearDown]
        public virtual void AfterEachTestRun()
        {
        }

        [NUnit.Framework.TestFixtureSetUp]
        public virtual void BeforeTestCaseRun()
        {
        }

        [NUnit.Framework.TestFixtureTearDown]
        public virtual void AfterTestCaseRun()
        {
        }

        public void _BeginTest(string methodName)
        {
            //reset variables
            _exception = null;
            _autoResetEvent = new AutoResetEvent(false);
            _testSucceeded = false;

            _log.AddEntry(_category, String.Format("Beginning test '{0}'", methodName));
        }

        public void TestSucceeded(string reason)
        {
            _testSucceeded = true;
            _log.AddEntry(_category, String.Format("Test succeeded because: {0}.", reason));
            Debug.WriteLine(String.Empty);
            ExitTest();
        }

        public void _EndTest(int timeoutInSeconds, bool failOnTimeout)
        {
            if (timeoutInSeconds > 0)
            {
                if (failOnTimeout)
                {
                    _log.AddEntry(_category, String.Format("Test will fail in {0} second(s).", timeoutInSeconds));
                }
                else
                {
                    _log.AddEntry(_category, String.Format("Test will succeed in {0} second(s).", timeoutInSeconds));
                }
                _autoResetEvent.WaitOne(TimeSpan.FromSeconds(timeoutInSeconds), false);
            }
            else
            {
                _autoResetEvent.WaitOne();
            }
            if (_exception != null)
            {
                //rethrow new exception to get complete correct stack trace
                throw new UnhandledThreadException(_exception);
            }
            if (failOnTimeout && !_testSucceeded)
            {
                throw new TimeoutException(
                    String.Format("Alloted time ({0} seconds) for test execution elapsed.", timeoutInSeconds));
            }
        }

        //used by inheriting class to marshal exceptions
        protected void TestExceptionEventHandler(Exception exception)
        {
            _exception = exception;
            ExitTest();
        }

        private void ExitTest()
        {
            _autoResetEvent.Set();
        }
    }

    public class Log
    {
        private bool entryAdded;

        public void AddEntry(string category, string msg)
        {
            Console.WriteLine("{0},{1}",category,msg);
        }

    }
}
