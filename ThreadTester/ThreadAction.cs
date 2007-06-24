using System;
using System.Threading;

namespace Osherove.ThreadTester
{
    public class ThreadAction
    {
        private  static ManualResetEvent StopAllActionsSignal = new ManualResetEvent(false);
        private static long jobNumber=1;
        private Thread stopThread;
        public void Start()
        {
            StopAllActionsSignal.Reset();
            stopThread = new Thread(StopThreadOnSignal);
            thread.Start();
            stopThread.Start();
            ThreadAction.allCanceled = false;
        }
        public static void StopAll()
        {
            StopAllActionsSignal.Set();
        }

        void StopThreadOnSignal()
        {
            StopAllActionsSignal.WaitOne();
//            Console.WriteLine("Signaled to stop..");
            ThreadAction.allCanceled = true;
            StopActionThread();
        }

        private void StopActionThread()
        {
//                Console.WriteLine("aborting");
            try
            {
                if (thread.ThreadState==ThreadState.Running || thread.ThreadState==ThreadState.Background)
                {
                    thread.Abort();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("abort ex gotten");
            }
//            }
        }

        public ThreadAction(Func del)
        {
            this.doCallback = del;
            thread = new Thread(startDelegate);
            thread.Name = "Thread job " + jobNumber++;
        }

        void startDelegate()
        {
            
            if (startSignal != null)
            {
                startSignal.WaitOne();
            }

            try
            {
                doCallback.Invoke();
                signalFinishedCallback.Invoke(this);
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
            }
        }

        private Thread thread;
        private Func doCallback;
        private ThreadFinishedDelegate signalFinishedCallback;
        private ManualResetEvent startSignal;
        private static bool allCanceled;

        public Thread Thread
        {
            get { return thread; }
            set { thread = value; }
        }

        public Func DoCallback
        {
            get { return doCallback; }
            set { doCallback = value; }
        }

        public ThreadFinishedDelegate SignalFinishedCallback
        {
            get { return signalFinishedCallback; }
            set { signalFinishedCallback = value; }
        }

        public static bool AllCanceled
        {
            get { return allCanceled; }
        }

        public void StartWhenSignaled(ManualResetEvent startSignal)
        {
            this.startSignal = startSignal;
            Start();
        }
    }
}