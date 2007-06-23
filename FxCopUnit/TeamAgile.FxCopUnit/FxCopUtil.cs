//this class is based partially on code from Sasha Goldstein's blog Entry
//about Running FxCop From code: http://blogs.microsoft.co.il/blogs/sasha/archive/2007/02/10/Run-FxCop-from-Code.aspx

using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Cci;
using Microsoft.FxCop.Common;
using Microsoft.FxCop.Sdk;
using Microsoft.FxCop.Sdk.Introspection;
using NUnit.Framework;

namespace TeamAgile.FxCopUnit
{
    public struct RuleResultData
    {
        public string RuleName;
        public long ElapsedMs;
    }

    public delegate void AnalyzedDelegate(FxCopReport report);
    public class FxCopUtil
    {
        internal event AnalyzedDelegate Analyzed = delegate { };
        public static FxCopUtil Create(string references, string ruleLocation, string targetLocation)
        {
            FxCopUtil util = new FxCopUtil(references,ruleLocation,targetLocation);
            return util;
        }
       
        private RuleFile m_ruleFile;
        private TargetFile m_targetFile;
        private RuleResultData result;
        private FxCopReport report;

        public  RuleResultData Analyze()
        {
            result.ElapsedMs = -1;

            Stopwatch stopper = null;
            try
            {

                stopper = Stopwatch.StartNew();
                FxCopOM.Engines.Analyze(FxCopOM.Project, true);
                stopper.Stop();
                StringBuilder failMessage =new StringBuilder();
                if (!FxCopOM.Project.AnalysisResults.AnalysisOccurred)
                {
                    failMessage.AppendLine("Analysis was not performed!");
                }
                if (FxCopOM.Project.AnalysisResults.Exceptions.Count > 0)
                {
                    failMessage.AppendLine("Exceptions occurred during analysis");
                    foreach (Exception exception in FxCopOM.Project.AnalysisResults.Exceptions)
                    {
                        failMessage.AppendLine(exception.ToString());
                    }
                }
                if (FxCopOM.Project.AnalysisResults.RuleExceptions.Count > 0)
                {
                    failMessage.AppendLine("Rule exceptions occurred during analysis");
                    foreach (Exception exception in FxCopOM.Project.AnalysisResults.RuleExceptions)
                    {
                        failMessage.AppendLine(exception.ToString());
                    }
                }
                if(!string.IsNullOrEmpty(failMessage.ToString()))
                {
                    Assert.Fail(failMessage.ToString());
                }
                Report = FxCopReport.MakeReport();
                Analyzed(Report);
            }
            catch (FxCopException fxCopException)
            {
                Assert.Fail(fxCopException.Message);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.ToString());
            }
            result.ElapsedMs = stopper == null ? -1 : stopper.ElapsedMilliseconds;
            return result;
        }

        internal FxCopReport Report
        {
            get { return report; }
            set { report = value; }
        }



        public  void EnableSpecificRule( string specificRuleCategory, string specificRuleId)
        {
            if (!String.IsNullOrEmpty(specificRuleCategory) && !String.IsNullOrEmpty(specificRuleId))
            {
                m_ruleFile.CheckAllChildren(false);
                Rule specificRule = FxCopOM.Project.AllRules.Find(specificRuleCategory, specificRuleId);
                specificRule.Enabled = true;
                specificRule.Checked = true;
                result.RuleName = specificRule.DisplayName;
            }
        }

        private FxCopUtil(string referencesDirectory, string ruleLocation, string targetLocation)
        {
            InitRulesAndTargets(referencesDirectory, ruleLocation, targetLocation);
        }

        private void InitRulesAndTargets(string referencesDirectory, string ruleLocation, string targetLocation)
        {
            result = new RuleResultData();
            ExceptionCollection exceptionCollection = FxCopOM.Initialize();
            if ((exceptionCollection != null) && (exceptionCollection.Count > 0))
            {
                foreach (Exception exception in exceptionCollection)
                {
                    Console.WriteLine("* " + exception.Message);
                    Console.WriteLine(exception.StackTrace);
                }
            }

            FxCopOM.Project = new Project();
            FxCopOM.Project.Options.SharedProject = false;

            FxCopOM.Project.Targets.AddReferenceDirectory(referencesDirectory);
                
            TargetFile targetFile = new TargetFile(targetLocation, FxCopOM.Project.Targets);
            FxCopOM.Engines.LoadTargets(targetFile);
            FxCopOM.Project.Targets.Add(targetFile);
                
            RuleFile ruleFile = new RuleFile(ruleLocation, FxCopOM.Project.RuleFiles);
            ruleFile.CheckAllChildren(true);
            FxCopOM.Project.RuleFiles.Add(ruleFile);

            FxCopOM.Project.Options.Stylesheet = string.Empty;
            m_ruleFile = ruleFile;
            m_targetFile = targetFile;
        }

        public  void EnableTypeToCheck(Type type)
        {
            bool found = false;
            foreach (NodeBaseDictionary child in m_targetFile.Children) if (child is TargetModuleDictionary)
                {
                    foreach (TargetModule m in child.Values)
                        foreach (TargetNamespace ns in m.Namespaces.Values)
                            foreach (TargetType targetType in ns.Types.Values)
                                if (targetType.FullName == type.FullName)
                                {
                                    Console.WriteLine("Checking - " + targetType.FullName);
                                    targetType.Checked = true;
                                    targetType.CheckAllChildren(true);
                                }
                }
            if (!found)
            {
                throw new InvalidOperationException(
                    string.Format("Could not locate type {0}", type));
            }
        }

        internal void EnableTargetMember(Type type,string memberName)
        {
            bool found = false;
            foreach (NodeBaseDictionary child in m_targetFile.Children)
                if (child is TargetModuleDictionary)
                    foreach (TargetModule m in child.Values)
                        foreach (TargetNamespace ns in m.Namespaces.Values)
                            foreach (TargetType targetType in ns.Types.Values)
                                if (targetType.FullyQualifiedName == type.FullName)
                                    foreach (TargetMember targetMember in targetType.Members.Values)
                                    {
                                        Console.WriteLine("is it .. " + targetMember.FullyQualifiedName);
                                        if (targetMember.Name == memberName)
                                        {
                                            Console.WriteLine("Enabling target- " + targetMember.FullyQualifiedName);
                                            targetType.Checked = true;
                                            targetType.CheckAllChildren(true);
                                            found = true;
                                            break;
                                        }
                                    }
            if (!found)
            {
                throw new InvalidOperationException(
                    string.Format("Could not locate member {0} in type {1}", memberName, type));
            }
        }

        public void EnableMethodToCheck(MethodInfo info)
        {
            string cciName = getCiiNameForMethod(info);
            EnableTargetMember(info.DeclaringType,cciName);   
        }

        private string getCiiNameForMethod(MethodInfo info)
        {
            Method method = Method.GetMethod(info);
            return RuleUtilities.Format(method,false,true);
        }
    }
    
    }

