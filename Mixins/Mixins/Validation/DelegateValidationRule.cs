using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins.Validation
{
  public class DelegateValidationRule<TDefinition> : IValidationRule<TDefinition> where TDefinition : IVisitableDefinition
  {
    public struct Args
    {
      public readonly ValidatingVisitor Validator;
      public readonly TDefinition Definition;
      public readonly IValidationLog Log;
      public readonly DelegateValidationRule<TDefinition> Self;

      public Args (ValidatingVisitor validator, TDefinition definition, IValidationLog log, DelegateValidationRule<TDefinition> self)
      {
        ArgumentUtility.CheckNotNull ("validator", validator);
        ArgumentUtility.CheckNotNull ("definition", definition);
        ArgumentUtility.CheckNotNull ("log", log);
        ArgumentUtility.CheckNotNull ("self", self);

        Validator = validator;
        Self = self;
        Log = log;
        Definition = definition;
      }
    }

    public delegate void Rule (Args args);

    private Rule _rule;
    private string _ruleName;
    private string _message;

    public DelegateValidationRule(Rule rule, string ruleName, string message)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      ArgumentUtility.CheckNotNull ("ruleName", ruleName);
      ArgumentUtility.CheckNotNull ("message", message);

      _rule = rule;
      _ruleName = ruleName;
      _message = message;
    }

    public DelegateValidationRule (Rule rule)
        : this (ArgumentUtility.CheckNotNull("rule", rule), rule.Method.DeclaringType.FullName + "." + rule.Method.Name, rule.Method.Name)
    {
    }

    public Rule RuleDelegate
    {
      get { return _rule; }
    }

    public string RuleName
    {
      get { return _ruleName; }
    }

    public string Message
    {
      get { return _message; }
    }

    public void Execute (ValidatingVisitor validator, TDefinition definition, IValidationLog log)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("log", log);
      RuleDelegate (new Args(validator, definition, log, this));
    }
  }
}
