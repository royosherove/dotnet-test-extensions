using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Osherove.ThreadTester.Events;

namespace Osherove.ThreadTester.Strategies
{
    class RunForSpecificTimeStrategy : AbstractThreadRunStrategy
    {
        private bool isFinishing;

        public override void StartAll(int timeout, List<ThreadAction> actions)
        {
            this.threadActions = actions;
            StartAllThreads(timeout);
        }

        public override void OnThreadFinished(ThreadAction threadAction)
        {
            if (isFinishing || ThreadAction.AllCanceled)
            {
                return;
            }
            ThreadAction action = new ThreadAction(threadAction.DoCallback);
            action.SignalFinishedCallback = threadAction.SignalFinishedCallback;
            this.threadActions.Add(action);
            action.Start();
        }


        public void StartAllThreads(int runningTimeout)
        {
            isFinishing = false;
            StartAllThreadsAtOnce();

            AutoResetEventEx timout = new AutoResetEventEx(false);
            Thread stopperThread = FlagIfEndingPrematurely(timout);
            stopperThread.Start();
            
            timout.WaitOne(runningTimeout, false);
            
            stopperThread.Abort();
            isFinishing = true;
            StopAllRunningThreads();
        }

        private Thread FlagIfEndingPrematurely(AutoResetEventEx flag)
        {
            Thread t = new Thread(new ThreadStart(delegate
                                      {
                                          try
                                          {
                                              while (true)
                                              {
                                                  if (ThreadAction.AllCanceled)
                                                  {
                                                      Console.WriteLine("Ending Prematurely");
                                                      flag.Set();
                                                      return;
                                                  }
                                                  Thread.Sleep(100);
                                              }
                                          }
                                          catch (ThreadAbortException e)
                                          {
                                              Thread.ResetAbort();
                                          }
                                      }));
            return t;
        }
    }
}