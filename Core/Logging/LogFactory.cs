using System;

namespace Rubicon.Logging
{
  /// <summary>
  /// Use this class to create a logger implementing <see cref="IExtendedLog"/> from the current <see cref="IExtendedLogFactory"/>.
  /// </summary>
  /// <remarks>
  /// Currently only <b>log4net</b> is supported as logging infrastructure.
  /// </remarks>
  public static class LogFactory
  {
    // TODO: Get from config section
    private static IExtendedLogFactory s_current = new Log4NetLogFactory ();

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger for the <paramref name="name"/> specified.</returns>
    public static IExtendedLog GetLogger (string name)
    {
      return s_current.GetLogger (name);
    }

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve.</param>
    /// <returns>A logger for the fully qualified name of the <paramref name="type"/> specified.</returns>
    public static IExtendedLog GetLogger (Type type)
    {
      return s_current.GetLogger (type);
    }
  }
}