namespace Remotion.Logging
{
  // TODO FS: Move to Remotion.Interfaces
  /// <summary>
  /// Defines the log levels available when logging through the <see cref="ILog"/> interface.
  /// </summary>
  public enum LogLevel
  {
    /// <summary>
    /// The <see cref="LogLevel.Debug"/> level designates fine-grained informational events that are most useful to debug an application.
    /// </summary>
    Debug = 0,

    /// <summary>
    /// The <see cref="LogLevel.Info"/> level designates informational messages that highlight the progress of the application at coarse-grained level. 
    /// </summary>
    Info,

    /// <summary>
    /// The <see cref="LogLevel.Warn"/> level designates potentially harmful situations.
    /// </summary>
    Warn,

    /// <summary>
    /// The <see cref="LogLevel.Error"/> level designates error events that might still allow the application to continue running. 
    /// </summary>
    Error,

    /// <summary>
    /// The <see cref="LogLevel.Fatal"/> level designates very severe error events that will presumably lead the application to abort.
    /// </summary>
    Fatal
  }
}