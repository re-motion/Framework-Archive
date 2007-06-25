using System;
using System.Text;
using Rubicon.Mixins;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Validation
{
	/// <summary>
	/// Thrown when there is an error in the mixin configuration which is detected during validation of the configuration. The problem prevents
	/// code being generated from the configuration. See also <see cref="ConfigurationException"/>.
	/// </summary>
  public class ValidationException : Exception
  {
    public readonly IValidationLog ValidationLog;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="message">The exception message.</param>
		/// <param name="log">The validation log.</param>
    public ValidationException (string message, IValidationLog log)
        : base (message)
    {
      ValidationLog = log;
    }

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationException"/> class and creates a descriptive message from the validation log.
		/// </summary>
		/// <param name="log">The validation log log.</param>
		/// <exception cref="ArgumentNullException">The log is empty.</exception>
    public ValidationException (IValidationLog log)
      : this (BuildExceptionString (log), log)
    {
    }

    private static string BuildExceptionString (IValidationLog log)
    {
			ArgumentUtility.CheckNotNull ("log", log);

      StringBuilder sb = new StringBuilder ("Some parts of the mixin configuration could not be validated.");
      foreach (ValidationResult item in log.GetResults())
      {
        if (item.TotalRulesExecuted != item.Successes.Count)
        {
          sb.Append (Environment.NewLine).Append (item.Definition.FullName);
          string parentString = item.GetParentDefinitionString();
          if (parentString.Length > 0)
            sb.Append (" (").Append (parentString).Append (")");
          sb.Append (": There were ");
          sb.Append (item.Failures.Count).Append (" errors, ").Append (item.Warnings.Count).Append (" warnings, and ")
              .Append (item.Exceptions.Count).Append (" unexpected exceptions. ");
          if (item.Exceptions.Count > 0)
            sb.Append ("First exception: ").Append (item.Exceptions[0].Message);
          else if (item.Failures.Count > 0)
            sb.Append ("First error: ").Append (item.Failures[0].Message);
        }
      }
      sb.Append (Environment.NewLine).Append ("See Log.GetResults() for a full list of issues.");
      return sb.ToString();
    }
  }
}
