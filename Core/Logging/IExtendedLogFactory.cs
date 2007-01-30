using System;

namespace Rubicon.Logging
{
  /// <summary>
  /// The <see cref="IExtendedLogFactory"/> interface declares the methods available for retrieving a logger that implements
  /// <see cref="IExtendedLog"/>.
  /// </summary>
  public interface IExtendedLogFactory
  {
    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger for the <paramref name="name"/> specified.</returns>
    IExtendedLog GetLogger (string name);

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve.</param>
    /// <returns>A logger for the fully qualified name of the <paramref name="type"/> specified.</returns>
    IExtendedLog GetLogger (Type type);
  }
}