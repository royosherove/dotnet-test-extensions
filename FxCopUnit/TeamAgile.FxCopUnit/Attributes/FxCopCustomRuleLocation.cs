using System;

namespace TeamAgile.FxCopRuleTesting
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FxCopCustomRuleLocation:Attribute
    {
        private string customRulesDLL;

        public string  CustomRulesDLL
        {
            get { return customRulesDLL; }
            set { customRulesDLL = value; }
        }

        public FxCopCustomRuleLocation(string customRulesDLL)
        {
            this.customRulesDLL = customRulesDLL;
        }
    }
}