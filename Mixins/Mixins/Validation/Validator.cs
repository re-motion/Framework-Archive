using System;
using System.Reflection;
using Mixins.Definitions;
using Mixins.Validation.DefaultLog;
using Mixins.Validation.Rules;

namespace Mixins.Validation
{
  public static class Validator
  {
    public static DefaultValidationLog Validate (IVisitableDefinition startingPoint, params IRuleSet[] customRuleSets)
    {
      DefaultValidationLog log = new DefaultValidationLog ();
      Validate (startingPoint, log, customRuleSets);
      return log;
    }

    public static void Validate (IVisitableDefinition startingPoint, IValidationLog log, params IRuleSet[] customRuleSets)
    {
      ValidatingVisitor visitor = new ValidatingVisitor (log);
      InstallDefaultRules (visitor);
      
      foreach (IRuleSet ruleSet in customRuleSets)
      {
        ruleSet.Install (visitor);
      }

      startingPoint.Accept (visitor);
    }

    private static void InstallDefaultRules (ValidatingVisitor visitor)
    {
      foreach (Type t in Assembly.GetExecutingAssembly ().GetTypes ())
      {
        if (!t.IsAbstract && typeof(IRuleSet).IsAssignableFrom(t) && t.Namespace == typeof (IRuleSet).Namespace)
        {
          IRuleSet ruleSet = (IRuleSet)Activator.CreateInstance (t);
          ruleSet.Install (visitor);
        }
      }
    }
  }
}
