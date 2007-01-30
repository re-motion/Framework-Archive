using System;
using System.Reflection;
using log4net.Core;
using Rubicon.Utilities;

namespace Rubicon.Logging
{
  /// <summary>
  /// Implementation of <see cref="IExtendedLogFactory"/> for <b>log4net</b>.
  /// </summary>
  public class Log4NetLogFactory : IExtendedLogFactory
  {
    /// <summary>
    /// Creates a new instance of the <see cref="Log4NetLog"/> type.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve. Must not be <see langword="null"/> or empty.</param>
    /// <returns>A <see cref="Log4NetLog"/> for the <paramref name="name"/> specified.</returns>
    public IExtendedLog GetLogger (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      return new Log4NetLog (LoggerManager.GetLogger (Assembly.GetCallingAssembly (), name));
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Log4NetLog"/> type.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve. Must not be <see langword="null"/>.</param>
    /// <returns>A <see cref="Log4NetLog"/> for the fully qualified name of the <paramref name="type"/> specified.</returns>
    public IExtendedLog GetLogger (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return new Log4NetLog (LoggerManager.GetLogger (Assembly.GetCallingAssembly (), type));
    }
  }
}