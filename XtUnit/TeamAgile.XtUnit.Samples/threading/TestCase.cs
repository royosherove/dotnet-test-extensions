using System;
using System.Diagnostics;

using HK.Testing;

namespace HK.ATS.Testing
{
    public class TestCase : MultiThreadedTestCase
    {

        public override void BeforeTestCaseRun()
        {
//            _log = Broker.Api.Log;
//            Broker.Api.Log.EntryAdded += new Log.EntryAddedEventHandler(OutputLogEntry);
//            Broker.Api.ExceptionThrown += TestExceptionEventHandler;
        }

        //remove event handlers
        public override void AfterTestCaseRun()
        {
//            ((Log)_log).EntryAdded -= new Log.EntryAddedEventHandler(OutputLogEntry);
//            Broker.Api.ExceptionThrown -= TestExceptionEventHandler;
        }

//        private static void OutputLogEntry(DateTime logTime, LogCategory category, string text)
//        {
//            Debug.WriteLine(String.Format("{0} | {1} | {2}", logTime.ToString(Formatting.TimeIncludingMilliseconds), category.ToString().PadRight(12), text));
//        }

    }
}
