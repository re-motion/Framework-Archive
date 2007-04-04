using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.Validation.DefaultLog
{
  public class DefaultValidationLog : IValidationLog
  {
    private Stack<DefaultValidationResult> _currentData = new Stack<DefaultValidationResult> ();
    private List<DefaultValidationResult> _results = new List<DefaultValidationResult> ();

    public IList<DefaultValidationResult> Results
    {
      get { return _results; }
    }

    public void ValidationStartsFor (IVisitableDefinition definition)
    {
      _currentData.Push (new DefaultValidationResult (definition));
    }

    public void ValidationEndsFor (IVisitableDefinition definition)
    {
      if (_currentData.Count == 0)
      {
        string message = string.Format ("Validation of definition {0}/{1} cannot be ended, because it wasn't started.", definition.GetType ().Name,
                                        definition.FullName);
        throw new InvalidOperationException (message);
      }
      else
      {
        DefaultValidationResult popped = _currentData.Pop ();
        if (!popped.Definition.Equals (definition))
        {
          string message = string.Format("Cannot end validation for {0} while {1} is validated.", definition.FullName, popped.Definition.FullName);
          throw new InvalidOperationException (message);
        }
        _results.Add (popped);
      }
    }

    private DefaultValidationResult GetCurrentResult ()
    {
      if (_currentData.Count == 0)
      {
        throw new InvalidOperationException ("Validation has not been started.");
      }
      return _currentData.Peek ();
    }

    public void Succeed (IValidationRule rule)
    {
      GetCurrentResult().Successes.Add (new DefaultValidationResultItem(rule));
    }

    public void Warn (IValidationRule rule)
    {
      GetCurrentResult ().Warnings.Add (new DefaultValidationResultItem(rule));
    }

    public void Fail (IValidationRule rule)
    {
      GetCurrentResult ().Failures.Add (new DefaultValidationResultItem (rule));
    }

    public void UnexpectedException<TDefinition> (IValidationRule<TDefinition> rule, Exception ex) where TDefinition : IVisitableDefinition
    {
      GetCurrentResult ().Exceptions.Add (new DefaultValidationExceptionItem (rule, ex));
    }
  }
}
