using System.Reflection;
using Microsoft.FxCop.Sdk.Introspection;

namespace TeamAgile.FxCopRuleTesting.SampleRules
{
    public abstract class CustomRuleBase<T>:BaseIntrospectionRule
    {
        public CustomRuleBase()
            : base(typeof(T).Name,
                "TeamAgile.FxCopRuleTesting.SampleRules.RuleManifest.xml", 
                Assembly.GetExecutingAssembly())
        {}

      
    }
}
