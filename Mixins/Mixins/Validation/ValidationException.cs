using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Text;

namespace Mixins.Validation
{
  public class ValidationException : Exception
  {
    public readonly IValidationLog ValidationLog;

    public ValidationException (string message, IValidationLog log)
        : base (message)
    {
      ValidationLog = log;
    }

    public ValidationException (IValidationLog log)
      : this (BuildExceptionString (log), log)
    {
    }

    private static string BuildExceptionString (IValidationLog log)
    {
      StringBuilder sb = new StringBuilder ("Some parts of the mixin configuration could not be validated.");
      foreach (ValidationResult item in log.GetResults())
      {
        if (item.TotalRulesExecuted != item.Successes.Count)
        {
          sb.Append (Environment.NewLine).Append (item.Definition.FullName).Append (": There were ");
          sb.Append (item.Failures.Count).Append (" errors, ").Append (item.Warnings.Count).Append (" warnings, and ")
              .Append (item.Exceptions.Count).Append (" unexpected exceptions. ");
          if (item.Exceptions.Count > 0)
            sb.Append ("First exception: ").Append (item.Exceptions[0].Message);
          else if (item.Failures.Count > 0)
            sb.Append ("First error: ").Append (item.Failures[0].Message);
        }
      }
      sb.Append (Environment.NewLine).Append ("See the list returned by Log.GetResults() for a detailed list of issues.");
      return sb.ToString();
    }
  }
}
