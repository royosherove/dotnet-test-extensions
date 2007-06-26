using System;

namespace Osherove.ThreadTester.Events
{
    public class WaitEventArgs:EventArgs
    {
        private bool cancelwait;

        public bool CancelWait
        {
            get { return cancelwait; }
            set { cancelwait = value; }
        }

        private int? timeout;

        public int? TimeOut
        {
            get { return timeout; }
            set { timeout = value; }
        }

        private bool? exitContext;

        public bool? ExitContext
        {
            get { return exitContext; }
            set { exitContext = value; }
        }

        public WaitEventArgs()
        {
        }


        public WaitEventArgs(int? timeout)
        {
            this.timeout = timeout;
        }

        public WaitEventArgs(int? timeout, bool? exitContext)
        {
            this.timeout = timeout;
            this.exitContext = exitContext;
        }
    }
}