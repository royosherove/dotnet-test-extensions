using System;
using System.IO;
using System.IO.IsolatedStorage;
using TeamAgile.FxCopUnit;
using XtUnit.Framework;

namespace TeamAgile.FxCopRuleTesting
{
   

    [AttributeUsage(AttributeTargets.Method)]
    public class FxCopRuleToTestAttribute:TestProcessingAttributeBase
    {
        #region Properties

        private string ruleDllFileName;

        public string RuleDllFileName
        {
            get { return ruleDllFileName; }
            set { ruleDllFileName = value; }
        }
        private string  ruleCheckID;

        public string  RuleCheckID
        {
            get { return ruleCheckID; }
            set { ruleCheckID = value; }
        }

        private string  ruleCategory;

        public string  RuleCategory
        {
            get { return ruleCategory; }
            set { ruleCategory = value; }
        }

        private string  targetAssemblyNameToRunRuleOn;

        public string  TargetAssemblyNameToRunRuleOn
        {
            get { return targetAssemblyNameToRunRuleOn; }
            set { targetAssemblyNameToRunRuleOn = value; }
        }

        private bool  targetIsExecutingTestAssembly;

        public bool  TargetIsExecutingTestAssembly
        {
            get { return targetIsExecutingTestAssembly; }
            set { targetIsExecutingTestAssembly = value; }
        }

        private Type targetType;

        public Type  TargetType
        {
            get { return targetType; }
            set { targetType = value; }
        }

        private string  targetMemberName;

        public string  TargetMemberName
        {
            get { return targetMemberName; }
            set { targetMemberName = value; }
        }

        #endregion

        #region constructors

        public FxCopRuleToTestAttribute(string ruleID, string ruleCategory)
        {
            this.ruleCheckID = ruleID;
            this.ruleCategory = ruleCategory;
        }

        public FxCopRuleToTestAttribute(string ruleDllFileName, string ruleID, string ruleCategory, string targetAssemblyNameToRunRuleOn)
        {
            this.ruleDllFileName = ruleDllFileName;
            this.ruleCheckID = ruleID;
            this.ruleCategory = ruleCategory;
            this.targetAssemblyNameToRunRuleOn = targetAssemblyNameToRunRuleOn;
        }


        public FxCopRuleToTestAttribute(string ruleID, string ruleCategory, string targetAssemblyNameToRunRuleOn)
        {
            this.ruleCheckID = ruleID;
            this.ruleCategory = ruleCategory;
            this.targetAssemblyNameToRunRuleOn = targetAssemblyNameToRunRuleOn;
        }


        public FxCopRuleToTestAttribute(string ruleDllFileName, string ruleID, string ruleCategory, bool targetIsExecutingTestAssembly)
        {
            this.ruleDllFileName = ruleDllFileName;
            this.ruleCheckID = ruleID;
            this.ruleCategory = ruleCategory;
            this.targetIsExecutingTestAssembly = targetIsExecutingTestAssembly;
        }

        public FxCopRuleToTestAttribute(string ruleID, string ruleCategory, bool targetIsExecutingTestAssembly)
        {
            this.ruleCheckID = ruleID;
            this.ruleCategory = ruleCategory;
            this.targetIsExecutingTestAssembly = targetIsExecutingTestAssembly;
        }

        #endregion


        

        protected override void OnPreProcess()
        {
            getRuleDllFileNameIfNeeded();
            string referenceDir = Path.GetDirectoryName(ruleDllFileName);
            
            if(targetIsExecutingTestAssembly)
            {
                targetAssemblyNameToRunRuleOn = DeclaringType.Assembly.Location;
            }
            
            FxCopUtil fx = createAndConfigureFxCopUtil(referenceDir);
            FxCopTestFixture parentFixture = methodCallTarget as FxCopTestFixture;
            if( parentFixture==null)
            {
                throw new InvalidOperationException("You must inherit your test fixture class from FxCopTestFixture!");
            }
            parentFixture.FxRunner = fx;
            fx.Analyzed+=(parentFixture.OnFxCopAnalyzed);
        }

        private FxCopUtil createAndConfigureFxCopUtil(string referenceDir)
        {
            bool hasRuleCategory = !string.IsNullOrEmpty(ruleCategory);
            bool hasRuleCheckID = !string.IsNullOrEmpty(ruleCheckID);
            bool hasTargetMember = !string.IsNullOrEmpty(targetMemberName);
            bool hasTargetType = targetType!=null;

            FxCopUtil fx =
                FxCopUtil.Create(referenceDir, ruleDllFileName, targetAssemblyNameToRunRuleOn);
            
            if (hasRuleCategory && hasRuleCheckID)
            {
                fx.EnableSpecificRule(ruleCategory, ruleCheckID);
            }
            if(hasTargetMember && hasTargetType)
            {
                fx.EnableTargetMember(targetType, targetMemberName);
            }
            if (hasTargetType && !hasTargetMember)
            {
                fx.EnableTypeToCheck(targetType);
            }
            return fx;
        }

        private void getRuleDllFileNameIfNeeded()
        {
            if(!string.IsNullOrEmpty(ruleDllFileName))
            {
                return;
            }
            bool isDefined = DeclaringType.IsDefined(typeof(FxCopCustomRuleLocation), true);
            if (!isDefined)
            {
                throw new InvalidOperationException(
                    "The test current fixture does not contain an FxCopTestFixture attribute");
            }
            FxCopCustomRuleLocation attribute = DeclaringType.GetCustomAttributes(typeof(FxCopCustomRuleLocation), true)[0] as FxCopCustomRuleLocation;
            this.ruleDllFileName = attribute.CustomRulesDLL;
        }

        protected override void OnPostProcess()
        {
            FxCopTestFixture fixture = methodCallTarget as FxCopTestFixture;
            if(fixture==   null)
            {
                return;
            }
            fixture.FxReport.Delete();
            
        }
    }
}
