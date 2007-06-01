using System.Collections.Generic;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins.Validation
{
  public struct ValidationResult
  {
    public readonly IVisitableDefinition Definition;

    public readonly List<ValidationResultItem> Successes;
    public readonly List<ValidationResultItem> Warnings;
    public readonly List<ValidationResultItem> Failures;
    public readonly List<ValidationExceptionResultItem> Exceptions;

    public ValidationResult (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      Definition = definition;
      Successes = new List<ValidationResultItem> ();
      Warnings = new List<ValidationResultItem> ();
      Failures = new List<ValidationResultItem> ();
      Exceptions = new List<ValidationExceptionResultItem> ();
    }

    public int TotalRulesExecuted
    {
      get { return Successes.Count + Warnings.Count + Failures.Count + Exceptions.Count; }
    }
  }
}