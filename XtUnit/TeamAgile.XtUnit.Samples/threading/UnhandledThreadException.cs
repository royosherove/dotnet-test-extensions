using System;

namespace HK.Common
{
    public class UnhandledThreadException : Exception
    {
        private string _message;
        private string _stackTrace;
        private Exception _originalException;

        public UnhandledThreadException(Exception exception)
        {
            _originalException = exception;
            _message = String.Format("({0}) {1}", exception.GetType(), exception.Message);
            _stackTrace = exception.StackTrace;
        }

        public override string Message
        {
            get { return _message; }
        }

        public override string StackTrace
        {
            get { return _stackTrace; }
        }

        public Exception OriginalException
        {
            get { return _originalException; }
        }
    }
}
