using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Osherove.ThreadTester.Events;
using Osherove.ThreadTester.Strategies;

namespace Osherove.ThreadTester.Tests
{
    [TestFixture]
    public class ThreadTests
    {

        class Singlton
        {
            public readonly Guid guid;
            private static Singlton instance;

            public Guid Guid
            {
                get { return guid; }
            }

            public Singlton()
            {
                guid = Guid.NewGuid();
            }
            static object sync = new object();
            public static Singlton Instance
            {
                get
                {
//                    Monitor.Enter(sync);
                    if(instance==null)
                    {
                        Thread.Sleep(new Random(15).Next(10,100));
                        instance=new Singlton();
                    }
//                    Monitor.Exit(sync);
                    return instance;
                }
            }
        }
        class Counter
        {
            private int count = 0;
            public void Increment()
            {
                count++;
                int x = count*50;
                string temp = x.ToString() + Guid.NewGuid().ToString();
            }


            public int Count
            {
                get { return count; }
            }
        }
        
      
        [Test]
        public void StartAllThreads_exceptionsInThreads_FoundInExceptionsProperty()
        {
        
            ThreadManager tt = new ThreadManager();
            tt.AddThreadAction(delegate
                                   {
                                       throw  new Exception("forced exception 1");
                                   });
            tt.AddThreadAction(delegate
                                   {
                                       throw  new Exception("forced exception 2");
                                   });
            tt.AddThreadAction(delegate
                                   {
                                       throw  new Exception("forced exception 3");
                                   });
        
            tt.StartAllThreads(1000);
            Assert.AreEqual(3,ThreadManager.exceptions.Count);
        }
       

        [Test]
        public void Singlton_MultiThreaded_SameInstance()
        {
            ThreadManager tt = new ThreadManager();
            Guid guid1 = Guid.Empty, 
                guid2 = Guid.Empty;

            tt.AddThreadAction(delegate
                                   {
                                       guid1 = Singlton.Instance.guid;
                                   });
            tt.AddThreadAction(delegate
                                   {
                                       guid2 = Singlton.Instance.guid;
                                   });

            tt.StartAllThreads(1000);
            Assert.AreEqual(guid1.ToString(),guid2.ToString());
        }
        [Test]
        public void SingleThread()
        {
            Counter c = new Counter();
            ThreadManager tt = new ThreadManager();
            tt.AddThreadAction(
                delegate
                    {
                        c.Increment();
                    });

            tt.RunBehavior=ThreadRunBehavior.RunUntilAllThreadsFinish;
            tt.StartAllThreads(500);
            Assert.IsTrue(tt.LastRunTime<tt.TimeOut);
        }
        
       
        
        [Test]
        public void SingleThreadForSpecifiedTimeStrategy()
        {
            Counter c = new Counter();
            ThreadManager tt = new ThreadManager();
            tt.AddThreadAction(
                delegate
                    {
                        c.Increment();
                    });

            tt.RunBehavior = ThreadRunBehavior.RunForSpecificTime;
            tt.StartAllThreads(500);
            Assert.IsTrue(tt.LastRunTime >499,"runtime was "+ tt.LastRunTime);
        }
        
        
        [Test]
        public void TryToCreateARaceCondition()
        {
            Counter c = new Counter();
            ThreadManager tt = new ThreadManager();
            for (int i = 0; i < 100; i++)
            {
                tt.AddThreadAction(
                    delegate
                        {
//                            Console.WriteLine(Thread.CurrentThread.Name);
                            for (int j = 0; j < 100000; j++)
                            {
                                c.Increment();
                            }
                        });
            }

            
          

            tt.StartAllThreads(5000);
            Assert.AreEqual(1000000,c.Count);
        }
        
        [Test]
        public void HundredThreads()
        {
            Counter c = new Counter();
            ThreadManager tt = new ThreadManager();
            for (int i = 0; i < 100; i++)
            {
                tt.AddThreadAction(delegate
                                       {
                                           for (int j = 0; j < 10; j++)
                                           {
                                               c.Increment();
                                               Thread.Sleep(new Random(j+1).Next(100,300));
                                           }
                                       });
            }
          
            //this test will run for 22.5 seconds
            tt.RunBehavior=ThreadRunBehavior.RunForSpecificTime;
            tt.StartAllThreads(22500);
        }

      
        
        
        [Test]
        public void StopWhenTrue_StopAfterCountReaches1000()
        {
            Counter c = new Counter();
            ThreadManager tt = new ThreadManager();
            tt.RunBehavior=ThreadRunBehavior.RunForSpecificTime;
            tt.AddThreadAction(delegate
                                       {
                                           for (int j = 0; j < 103; j++)
                                           {
                                               c.Increment();
                                           }
                                           Thread.Sleep(50);
                                       });

            tt.StopWhenTrue(delegate
                                {
                                    Console.WriteLine("currently at " + c.Count);
                                    return c.Count > 1000;
                                },100);

            tt.StartAllThreads(10000);
            Assert.Greater(c.Count,1000);
            Assert.Less(c.Count,1050);
        }
    }
}
