using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Threading;

namespace Osherove.ThreadTester.Events
{

    [ComVisible(true), HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class EventWaitHandleEx : EventWaitHandle
    {
        public delegate void WaitDelegate(object sender, WaitEventArgs args);
        public event WaitDelegate BeforeWaitCalled = delegate { };
        public event EventHandler Closed = delegate { };
        
        public EventWaitHandleEx(bool initialState, EventResetMode mode)
            : base(initialState, mode)
        {
        }


        public override bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            bool cancel = false;
            OnWait((int?) timeout.TotalMilliseconds, exitContext, ref cancel);
            if (cancel)
            {
                return false;
            }

            return base.WaitOne(timeout, exitContext);
        }

        public override bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            bool cancel = false;
            OnWait(millisecondsTimeout, exitContext, ref cancel);
            if (cancel && allowWaitCanceling)
            {
                return false;
            }
            return base.WaitOne(millisecondsTimeout, exitContext);
        }


        public override bool WaitOne()
        {
                        bool cancel = false;
                        OnWait(null, null,ref cancel);
                        if (cancel)
                        {
                            return false;
                        } 
            return base.WaitOne();
        }


        private bool allowWaitCanceling;

        /// <summary>
        /// Setting this to true will trigger the BeforeWaitCalled event
        /// in a synchronized fasion. when false, the event is thrown in asyc (new thread).
        /// </summary>
        public bool AllowWaitCanceling
        {
            get { return allowWaitCanceling; }
            set { allowWaitCanceling = value; }
        }

        private void TriggerWaitCalledNewThread(int? timeout, bool? exitContext, ref bool cancel)
        {

            WaitEventArgs args = new WaitEventArgs(timeout, exitContext);
            Thread t = new Thread(new ThreadStart(delegate
                                                      {
                                                          SafeTrigger(BeforeWaitCalled, this, args);
                                                          if (args.CancelWait)
                                                          {
                                                              ArgumentException exception = new ArgumentException("You can't cancel a call to WaitOne() without setting AllowWaitCanceling to true");
                                                              ThreadManager.exceptions.Add(exception);
                                                              throw exception;
                                                          }
                                                      }));
            t.Start();
        }
        private static void SafeTrigger(Delegate del, params object[] args)
        {
//            //            return;
//            del.DynamicInvoke(args);
//            return;

            foreach (Delegate callback in del.GetInvocationList())
            {
                try
                {
                    callback.DynamicInvoke(args);
                }
                catch (Exception e)
                {
                    //                    Console.WriteLine(e.ToString());
                }
            }
        }
        private void TriggerWaitCalledSameThread(int? timeout, bool? exitContext, ref bool cancel)
        {
            WaitEventArgs args = new WaitEventArgs(timeout, exitContext);
            SafeTrigger(BeforeWaitCalled, this, args);
            if (args.CancelWait)
            {
                cancel = true;
            }
        }
        private int waiters;

        public int Waiters
        {
            get { return waiters; }
            set { waiters = value; }
        }

        private void OnWait(int? timeout, bool? exitContext, ref bool cancel)
        {

            waiters++;
            if (allowWaitCanceling)
            {
                TriggerWaitCalledSameThread(timeout, exitContext, ref cancel);
                if(cancel)
                {
                    waiters--;
                }
            }
            else
            {
                TriggerWaitCalledNewThread(timeout, exitContext, ref cancel);
            }

        }
   
    
    
    }
   


    
}