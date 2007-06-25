using System;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Thrown when there is a profound error in the mixin configuration which is detected during configuration analysis. The problem prevents
  /// the configuration from being fully analyzed. See also <see cref="ValidationException"/>.
  /// </summary>
  public class ConfigurationException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ConfigurationException (string message)
        : base (message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ConfigurationException (string message, Exception innerException)
        : base (message, innerException)
    {
    }
  }
}