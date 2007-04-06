using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

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

    public void Execute (ValidatingVisitor validator, TDefinition definition, IValidationLog log)
    {
      RuleDelegate (new Args(validator, definition, log, this));
    }
  }
}
