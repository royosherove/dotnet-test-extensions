using System;
using System.Collections.Generic;
using System.Text;

namespace Osherove.ThreadTester.Events
{
    public class UnhandledException:Exception
    {
        private object sender;
        private UnhandledExceptionEventArgs args;

        public UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            this.sender = sender;
            this.args = args;
        }

        public object Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        public UnhandledExceptionEventArgs Args
        {
            get { return args; }
            set { args = value; }
        }
    }
}
