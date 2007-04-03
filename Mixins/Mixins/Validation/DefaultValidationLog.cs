using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.Validation
{
  public class DefaultValidationLog : IValidationLog
  {
    public struct ValidationData
    {
      public readonly IVisitableDefinition Definition;

      public readonly List<string> Successes;
      public readonly List<string> Warnings;
      public readonly List<string> Failures;
      public readonly List<Exception> Exceptions;

      public ValidationData (IVisitableDefinition definition)
      {
        Definition = definition;
        Successes = new List<string> ();
        Warnings = new List<string> ();
        Failures = new List<string> ();
        Exceptions = new List<Exception> ();
      }

      public int TotalRulesExecuted
      {
        get { return Successes.Count + Warnings.Count + Failures.Count + Exceptions.Count; }
      }
    }

    private Stack<ValidationData> _currentData = new Stack<ValidationData> ();
    private List<ValidationData> _results = new List<ValidationData> ();

    public IEnumerable<ValidationData> Results
    {
      get { return _results; }
    }

    public void ValidationStartsFor (IVisitableDefinition definition)
    {
      _currentData.Push (new ValidationData (definition));
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
        _results.Add (_currentData.Pop ());
      }
    }

    public void Succeed (string message)
    {
      CheckStackNotEmpty ();
      _currentData.Peek ().Successes.Add (message);
    }

    public void Warn (string message)
    {
      CheckStackNotEmpty ();
      _currentData.Peek ().Warnings.Add (message);
    }

    public void Fail (string message)
    {
      CheckStackNotEmpty ();
      _currentData.Peek ().Failures.Add (message);
    }

    public void UnexpectedException (Exception ex)
    {
      CheckStackNotEmpty ();
      _currentData.Peek ().Exceptions.Add (ex);
    }

    private void CheckStackNotEmpty ()
    {
      if (_currentData.Count == 0)
      {
        throw new InvalidOperationException ("Validation has not been started.");
      }
    }
  }
}
