
using System.Reflection;
using XtUnit.Framework;

namespace TeamAgile.FxCopUnit
{
    public class FxCopTestFixture:TestFixtureBase
    {
        public virtual void OnFxCopAnalyzed(FxCopReport report)
        {
            FxReport = report;
        }
        private FxCopUtil fxCopUtil;
        private FxCopReport fxReport;

        public FxCopUtil FxRunner
        {
            get { return fxCopUtil; }
            set { fxCopUtil = value; }
        }

        public FxCopReport FxReport
        {
            get { return fxReport; }
            set { fxReport = value; }
        }

        public delegate void ParameterlessMethodDelegate();

        public MethodInfo GetMethodInfo(ParameterlessMethodDelegate method)
        {
            return method.Method;
        }
    }
}
