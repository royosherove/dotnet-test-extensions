using System;
using System.Collections.Generic;
using System.Threading;
using Osherove.ThreadTester.Events;

namespace Osherove.ThreadTester.Strategies
{
    internal abstract class AbstractThreadRunStrategy : IThreadRunStrategy
    {

        protected List<ThreadAction> threadActions;

        public abstract void StartAll(int timeout, List<ThreadAction> actions);

        public abstract void OnThreadFinished(ThreadAction threadAction);

        public void StopAll()
        {
            StopAllRunningThreads();
        }

        protected void StartAllThreadsAtOnce()
        {
            Console.WriteLine("Preparing " + threadActions.Count + " threads..");
            ManualResetEventEx threadStartSignal = new ManualResetEventEx(false);
            foreach (ThreadAction action in threadActions)
            {
                action.StartWhenSignaled(threadStartSignal);
            }
            Console.WriteLine("Starting all threads..");
            threadStartSignal.Set();
        }


        protected void StopAllRunningThreads()
        {
            if(ThreadAction.AllCanceled)
            {
                return;
            }
            Console.WriteLine("Stopping ALL running threads...");
            ThreadAction.StopAll();
        }
    }
}