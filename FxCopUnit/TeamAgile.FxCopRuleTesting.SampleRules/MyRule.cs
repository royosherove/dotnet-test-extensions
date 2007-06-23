using System;
using Microsoft.Cci;
using Microsoft.FxCop.Sdk;
using Microsoft.FxCop.Sdk.Introspection;

namespace TeamAgile.FxCopRuleTesting.SampleRules
{
    public class MyRule:CustomRuleBase<MyRule>
    {
        public override TargetVisibilities TargetVisibility
        {
            get { return TargetVisibilities.All; }
        }


        public override ProblemCollection Check(Member member)
        {
            Console.WriteLine("inside my rule!");
            ProblemCollection collection = new ProblemCollection(this);
            collection.Add(new Problem(GetResolution(member.Name.Name)));
            return collection;
        }
    }
}
