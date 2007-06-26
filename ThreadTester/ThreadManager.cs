using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using NUnit.Framework;
using Osherove.ThreadTester.Strategies;
using Osherove.ThreadTester.Strategies;
using Osherove.ThreadTester.Tests;
using Osherove.ThreadTester.Strategies;
using Timer=System.Timers.Timer;

namespace Osherove.ThreadTester
{
    public delegate void Func();
    public delegate bool CheckDelegate();
    public delegate void ThreadFinishedDelegate(ThreadAction threadAction);

    public class ThreadManager
    {
        private IThreadRunStrategy runner = new AllThreadsShouldFinishStrategy();

        private readonly List<ThreadAction> threadActions = new List<ThreadAction>();
        private long lastRunTime;
        readonly Stopwatch stopwatch = new Stopwatch();
        private long timeOut;
        private ThreadRunBehavior runBehavior=ThreadRunBehavior.RunUntilAllThreadsFinish;

        void SignalThreadIsFinished(ThreadAction threadAction)
        {
            runner.OnThreadFinished(threadAction);

        }

        internal static readonly List<Exception> exceptions = new List<Exception>();

        public static List<Exception> Exceptions
        {
            get { return exceptions; }
        }

        public void AddThreadAction(Func ActionCallback)
        {
            ThreadAction action = new ThreadAction(ActionCallback);
            action.SignalFinishedCallback = SignalThreadIsFinished;
            threadActions.Add(action);
        }

        public List<ThreadAction> ThreadActions
        {
            get { return threadActions; }
        }


        public long LastRunTime
        {
            get { return lastRunTime; }
        }

        public long TimeOut
        {
            get { return timeOut; }
        }

        public void StartAllThreads(int runningTimeout)
        {
            runner = CreateStrategy(RunBehavior);
            timeOut = runningTimeout;
            stopwatch.Reset();
            stopwatch.Start();
            checkTimer.Start();
            
            runner.StartAll(runningTimeout, threadActions);
            
            checkTimer.Stop();
            stopwatch.Stop();
            lastRunTime = stopwatch.ElapsedMilliseconds;
        }


        public ThreadRunBehavior RunBehavior
        {
            get { return runBehavior; }
            set { runBehavior = value; }
        }

        protected IThreadRunStrategy CreateStrategy(ThreadRunBehavior val)
        {
            Dictionary<ThreadRunBehavior, IThreadRunStrategy> runStrategies = new Dictionary<ThreadRunBehavior, IThreadRunStrategy>();
            runStrategies.Add(ThreadRunBehavior.RunForSpecificTime, new RunForSpecificTimeStrategy());
            runStrategies.Add(ThreadRunBehavior.RunUntilAllThreadsFinish, new AllThreadsShouldFinishStrategy());

            return runStrategies[val];
        }

        private Timer checkTimer = new Timer();
        public void StopWhenTrue(CheckDelegate checkCallback,double interval)
        {
            checkTimer = new Timer(interval);
            checkTimer.Elapsed+=delegate {
                                        if(checkCallback())
                                        {
                                            checkTimer.Stop();
                                            runner.StopAll();
                                        }
                                    };
            
        }
    }
}