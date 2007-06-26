using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Osherove.ThreadTester.Events;

namespace Osherove.ThreadTester.Tests
{
    [TestFixture]
    public class EventWaitHandleExTests
    {
        [Test]
//        [ExpectedException(typeof(ArgumentException), "You can't cancel a call to WaitOne() without setting AllowWaitCanceling to true")]
        public void WaitOne_CancelSetToTrueOnEvent_CanCancelWait()
        {
            AutoResetEventEx e = new AutoResetEventEx(false);
            e.BeforeWaitCalled+=delegate(object sender, WaitEventArgs args)
                                    {
                                        args.CancelWait = true;
                                    };
          e.WaitOne(100, true);
          Assert.AreEqual(1,ThreadManager.exceptions.Count);  
            
        }

        [Test]
        public void BeforeWaitCalled_Triggered()
        {
            AutoResetEventEx e = new AutoResetEventEx(false);
            e.BeforeWaitCalled += delegate(object sender, WaitEventArgs args)
                                      {
                                          Assert.AreEqual(100,args.TimeOut);
                                          Assert.AreEqual(true,args.ExitContext);
                                           Console.WriteLine("wait({0},{1})", args.TimeOut, args.ExitContext);
                                      };
            bool result = e.WaitOne(100, true);
            Assert.AreEqual(1, e.Waiters);
            Console.WriteLine("done");
        }
        
        [Test]
        public void BeforeWaitCalled_TriggeredAndCancenled()
        {
            AutoResetEventEx e = new AutoResetEventEx(false);
            e.AllowWaitCanceling = true;
            e.BeforeWaitCalled += delegate(object sender, WaitEventArgs args)
                                      {
                                          args.CancelWait = true;
                                      };
            bool result = e.WaitOne(1000, true);
            Assert.AreEqual(0,e.Waiters);
            Console.WriteLine("done");
        }
    }
}
