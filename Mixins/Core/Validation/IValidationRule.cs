using System;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.Validation
{
  public interface IValidationRule
  {
    string RuleName { get; }
    string Message { get; }
  }

  public interface IValidationRule<TDefinition> : IValidationRule
      where TDefinition : IVisitableDefinition
  {
    void Execute (ValidatingVisitor validator, TDefinition definition, IValidationLog log);
  }
}
