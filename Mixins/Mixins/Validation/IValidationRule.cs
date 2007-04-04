using System;
using Mixins.Definitions;

namespace Mixins.Validation
{
  public interface IValidationRule
  {
    string RuleName { get; }
    string Message { get; }
  }

  public interface IValidationRule<TDefinition> : IValidationRule
      where TDefinition : IVisitableDefinition
  {
    void Execute (TDefinition definition, IValidationLog log);
  }
}
