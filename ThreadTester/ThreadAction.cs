using System.Threading;

namespace Osherove.ThreadTester
{
    public class ThreadAction
    {
        private static long jobNumber=1;
        public void Start()
        {
            thread.Start();
        }
        public void Stop()
        {
            if(thread.ThreadState==ThreadState.Running)
            {
                thread.Abort();
            }
        
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
                //make sure exception does not get re-thrown
                Thread.ResetAbort();
            }
        }

        private Thread thread;
        private Func doCallback;
        private ThreadFinishedDelegate signalFinishedCallback;
        private ManualResetEvent startSignal;

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

        public void StartWhenSignaled(ManualResetEvent startSignal)
        {
            this.startSignal = startSignal;
            Start();
        }
    }
}