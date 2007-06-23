using System.Reflection;
using Microsoft.Cci;
using Microsoft.FxCop.Sdk.Introspection;
using NUnit.Framework;
using TeamAgile.FxCopRuleTesting.SampleRules;

namespace TeamAgile.FxCopRuleTesting
{
    [TestFixture]
    public class FxCopUnitTests
    {
        [Test]
        public void InvokeMyRuleWithMethodInfo()
        {
            MethodInfo someMethodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
            Microsoft.Cci.Method methodData = Microsoft.Cci.Method.GetMethod(someMethodInfo);
            MyRule ruleUnderTest = new MyRule();
            ProblemCollection problems = ruleUnderTest.Check(methodData);
            Assert.Greater(0,problems.Count);
        }
        
        [Test]
        public void InvokeMyRuleWithTypeInfo()
        {
            Microsoft.Cci.TypeNode thisTypeData = Microsoft.Cci.Class.GetTypeNode(this.GetType());
            
            MyRule ruleUnderTest = new MyRule();
            ProblemCollection problems = ruleUnderTest.Check(thisTypeData);

            Assert.Greater(0,problems.Count);
        }
    }
}
