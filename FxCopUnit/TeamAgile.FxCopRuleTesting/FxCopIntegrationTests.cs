using System;
using System.Reflection;
using NUnit.Framework;
using TeamAgile.FxCopUnit;

namespace TeamAgile.FxCopRuleTesting
{
    [TestFixture]
    [FxCopCustomRuleLocation("TeamAgile.FxCopRuleTesting.SampleRules.dll")]
    public class FxCopIntegrationTests:FxCopTestFixture
    {
        [Test]
        [FxCopRuleToTest("Rule0001", "FxCopCustomRules.CustomRules",true)]
        public void RunRuleAgainstMethod()
        {
            MethodInfo methodInfo = GetMethodInfo(MethodUnderTest);
            FxRunner.EnableMethodToCheck(methodInfo);
            FxRunner.Analyze();

            FxReport.Assert.AtLeastOneAnalysisProblem();
            Console.WriteLine(FxReport.Document.InnerXml);
        }
        

        public void MethodUnderTest()
        {
            Console.WriteLine("yo!");
        }
    }
}
