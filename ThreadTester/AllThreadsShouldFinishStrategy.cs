using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Osherove.ThreadTester
{
    class AllThreadsShouldFinishStrategy:IThreadRunStrategy
    {
        readonly AutoResetEvent allThreadsAreFinishedSignal = new AutoResetEvent(false);
        private int finishedThreadsCount = 0;

        private List<ThreadAction> threadActions;
        private readonly object sync = new object();
        public void StartAll(int timeout,List<ThreadAction> actions)
        {
            this.threadActions = actions;
            StartAllThreads(timeout);
        }

        public void OnThreadFinished(Thread thread)
        {
            lock (this.sync)
            {
                finishedThreadsCount++;
                if (finishedThreadsCount == threadActions.Count)
                {
                    allThreadsAreFinishedSignal.Set();
                }
            }
        }


        public void StartAllThreads(int runningTimeout)
        {
            Console.WriteLine("Starting " + threadActions.Count + " threads..");
            finishedThreadsCount = 0;
            StartAllThreadsAtOnce();

            allThreadsAreFinishedSignal.WaitOne(runningTimeout, false);
            if (finishedThreadsCount != threadActions.Count)
            {
                Assert.Fail("Not all threads were done");
            }
        }

        private void StartAllThreadsAtOnce()
        {
            ManualResetEvent threadStartSignal = new ManualResetEvent(false);
            foreach (ThreadAction action in threadActions)
            {
                action.StartWhenSignaled(threadStartSignal);
            }
            threadStartSignal.Set();
        }
    }
}
