using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.Validation
{
  public class DelegateValidationRule<TDefinition> : IValidationRule<TDefinition> where TDefinition : IVisitableDefinition
  {
    public delegate void Rule (TDefinition definition, IValidationLog log, DelegateValidationRule<TDefinition> self);

    private Rule _rule;
    private string _ruleName;
    private string _message;

    public DelegateValidationRule(Rule rule, string ruleName, string message)
    {
      _rule = rule;
      _ruleName = ruleName;
      _message = message;
    }

    public DelegateValidationRule (Rule rule)
        : this (rule, rule.Method.DeclaringType.FullName + "." + rule.Method.Name, rule.Method.Name)
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

    public void Execute (TDefinition definition, IValidationLog log)
    {
      RuleDelegate (definition, log, this);
    }
  }
}
