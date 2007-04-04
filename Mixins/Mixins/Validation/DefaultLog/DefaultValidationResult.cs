using System.Collections.Generic;
using Mixins.Definitions;

namespace Mixins.Validation.DefaultLog
{
  public struct DefaultValidationResult
  {
    public readonly IVisitableDefinition Definition;

    public readonly List<DefaultValidationResultItem> Successes;
    public readonly List<DefaultValidationResultItem> Warnings;
    public readonly List<DefaultValidationResultItem> Failures;
    public readonly List<DefaultValidationExceptionItem> Exceptions;

    public DefaultValidationResult (IVisitableDefinition definition)
    {
      Definition = definition;
      Successes = new List<DefaultValidationResultItem> ();
      Warnings = new List<DefaultValidationResultItem> ();
      Failures = new List<DefaultValidationResultItem> ();
      Exceptions = new List<DefaultValidationExceptionItem> ();
    }

    public int TotalRulesExecuted
    {
      get { return Successes.Count + Warnings.Count + Failures.Count + Exceptions.Count; }
    }
  }
}