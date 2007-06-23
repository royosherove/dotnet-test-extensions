using System;
using Microsoft.FxCop.Common;
using NUnit.Framework;

namespace TeamAgile.FxCopUnit
{
    public partial class AssertFxCop
    {
        private FxCopReport report;

        public FxCopReport Report
        {
            get { return report; }
            set { report = value; }
        }

        public AssertFxCop(FxCopReport report)
        {
            this.report = report;
        }

        public   void NoAnalysisProblems()
        {
            Assert.AreEqual(0, FxCopOM.Project.AnalysisResults.Exceptions.Count, "Analysis problems found");
        }
        public   void AtLeastOneAnalysisProblem()
        {
            Assert.Greater(FxCopOM.Project.AnalysisResults.MessageCount,0, "No Analysis problems found");
        }
        
        public   void ContainsBuildBreakingMessage()
        {
            Assert.IsTrue(FxCopOM.Project.ContainsBuildBreakingMessage, "Build breaking message not found");
        }
        public   void DoesNotContainBuildBreakingMessage()
        {
            Assert.IsFalse(FxCopOM.Project.ContainsBuildBreakingMessage, "Build breaking message found");
        }

        public   void AnalysisExceptionsEqual(int expected, int actual)
        {
            Assert.AreEqual(expected, FxCopOM.Project.AnalysisResults.Exceptions.Count, "Analysis exceptions amount differs");
        }
        
       
        
        public   void AnalysisExceptionMessageExists(string message)
        {
            foreach (Exception exception in FxCopOM.Project.AnalysisResults.Exceptions)
            {
                if(exception.Message==message)
                {
                    return;
                }
            }
            Assert.Fail("Could not locate expected analysis exception message in analysis results");
        }
    }
}
