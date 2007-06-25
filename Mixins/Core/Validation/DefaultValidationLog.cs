using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Validation
{
  public class DefaultValidationLog : IValidationLog
  {
    private Stack<ValidationResult> _currentData = new Stack<ValidationResult> ();
    private List<ValidationResult> _results = new List<ValidationResult> ();

    private int failures = 0;
    private int warnings = 0;
    private int exceptions = 0;
    private int successes = 0;

    public IEnumerable<ValidationResult> GetResults()
    {
      return _results;
    }

    public int ResultCount
    {
      get { return _results.Count; }
    }

    public int GetNumberOfFailures()
    {
      return failures;
    }

    public int GetNumberOfWarnings ()
    {
      return warnings;
    }

    public int GetNumberOfSuccesses ()
    {
      return successes;
    }

    public int GetNumberOfUnexpectedExceptions ()
    {
      return exceptions;
    }

    public int GetNumberOfRulesExecuted ()
    {
      return successes + warnings + failures + exceptions;
    }

    public void ValidationStartsFor (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      _currentData.Push (new ValidationResult (definition));
    }

    public void ValidationEndsFor (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      if (_currentData.Count == 0)
      {
        string message = string.Format ("Validation of definition {0}/{1} cannot be ended, because it wasn't started.", definition.GetType ().Name,
            definition.FullName);
        throw new InvalidOperationException (message);
      }
      else
      {
        ValidationResult popped = _currentData.Pop ();
        if (!popped.Definition.Equals (definition))
        {
          string message = string.Format("Cannot end validation for {0} while {1} is validated.", definition.FullName, popped.Definition.FullName);
          throw new InvalidOperationException (message);
        }
        _results.Add (popped);
      }
    }

    private ValidationResult GetCurrentResult ()
    {
      if (_currentData.Count == 0)
      {
        throw new InvalidOperationException ("Validation has not been started.");
      }
      return _currentData.Peek ();
    }

    public void Succeed (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult().Successes.Add (new ValidationResultItem(rule));
      ++successes;
    }

    public void Warn (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult ().Warnings.Add (new ValidationResultItem(rule));
      ++warnings;
    }

    public void Fail (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult ().Failures.Add (new ValidationResultItem (rule));
      ++failures;
    }

    public void UnexpectedException<TDefinition> (IValidationRule<TDefinition> rule, Exception ex) where TDefinition : IVisitableDefinition
    {
      GetCurrentResult ().Exceptions.Add (new ValidationExceptionResultItem (rule, ex));
      ++exceptions;
    }
  }
}