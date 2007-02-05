using System;
using System.Globalization;
using log4net.Core;
using log4net.Util;

namespace Rubicon.Logging
{
  /// <summary>
  ///   Implementation of interface <see cref="ILog"/> for <b>log4net</b>.
  /// </summary>
  /// <remarks>Use <see cref="LogManager"/> to instantiate <see cref="Log4NetLog"/> via <see cref="LogManager.GetLogger"/>.</remarks>
  public class Log4NetLog : LogImpl, ILog
  {
    /// <summary>
    /// Converts <see cref="LogLevel"/> to <see cref="Level"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> to be converted.</param>
    /// <returns>Corresponding <see cref="Level"/> needed for logging to the <b>log4net </b> <see cref="log4net.ILog"/> interface.</returns>
    public static Level Convert (LogLevel logLevel)
    {
      switch (logLevel)
      {
        case LogLevel.Debug:
          return Level.Debug;
        case LogLevel.Info:
          return Level.Info;
        case LogLevel.Warn:
          return Level.Warn;
        case LogLevel.Error:
          return Level.Error;
        case LogLevel.Fatal:
          return Level.Fatal;
        default:
          throw new ArgumentException (string.Format ("LogLevel does not support value {0}.", logLevel), "logLevel");
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Log4NetLog"/> class 
    /// using the specified <see cref="log4net.Core.ILogger"/>.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> the log messages are written to.</param>
    public Log4NetLog (ILogger logger)
      : base (logger)
    {
    }

    /// <overloads><inheritdoc cref="ILog.Log(LogLevel, object)"/></overloads>
    /// <inheritdoc />
    public void Log (LogLevel logLevel, int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Convert (logLevel), eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    public void Log (LogLevel logLevel, int eventID, object message)
    {
      LogToLog4Net (Convert (logLevel), eventID, message, null);
    }

    /// <inheritdoc />
    public void Log (LogLevel logLevel, object message, Exception exceptionObject)
    {
      LogToLog4Net (Convert (logLevel), null, message, exceptionObject);
    }

    /// <inheritdoc />
    public void Log (LogLevel logLevel, object message)
    {
      LogToLog4Net (Convert (logLevel), null, message, null);
    }

    /// <overloads><inheritdoc cref="ILog.LogFormat(LogLevel, string, object[])"/></overloads>
    /// <inheritdoc />
    public void LogFormat (LogLevel logLevel, int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Convert (logLevel), eventID, format, args, exceptionObject);
    }

    /// <inheritdoc />
    public void LogFormat (LogLevel logLevel, int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Convert (logLevel), eventID, format, args, null);
    }

    /// <inheritdoc />
    public void LogFormat (LogLevel logLevel, string format, params object[] args)
    {
      LogToLog4NetFormat (Convert (logLevel), null, format, args, null);
    }

    /// <inheritdoc />
    public void LogFormat (LogLevel logLevel, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Convert (logLevel), null, format, args, exceptionObject);
    }

    /// <overloads><inheritdoc cref="ILog.Debug(object)"/></overloads>
    /// <inheritdoc />
    public void Debug (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Debug, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    public void Debug (int eventID, object message)
    {
      LogToLog4Net (Level.Debug, eventID, message, null);
    }

    /// <overloads><inheritdoc cref="ILog.DebugFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    public void DebugFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Debug, eventID, format, args, exceptionObject);
    }

    /// <inheritdoc />
    public void DebugFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Debug, eventID, format, args, null);
    }

    /// <inheritdoc />
    public void DebugFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Debug, null, format, args, exceptionObject);
    }

    /// <overloads><inheritdoc cref="ILog.Info(object)"/></overloads>
    /// <inheritdoc />
    public void Info (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Info, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    public void Info (int eventID, object message)
    {
      LogToLog4Net (Level.Info, eventID, message, null);
    }

    /// <overloads><inheritdoc cref="ILog.InfoFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    public void InfoFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Info, eventID, format, args, exceptionObject);
    }

    /// <inheritdoc />
    public void InfoFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Info, eventID, format, args, null);
    }

    /// <inheritdoc />
    public void InfoFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Info, null, format, args, exceptionObject);
    }

    /// <overloads><inheritdoc cref="ILog.Warn(object)"/></overloads>
    /// <inheritdoc />
    public void Warn (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Warn, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    public void Warn (int eventID, object message)
    {
      LogToLog4Net (Level.Warn, eventID, message, null);
    }

    /// <overloads><inheritdoc cref="ILog.WarnFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    public void WarnFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Warn, eventID, format, args, exceptionObject);
    }

    /// <inheritdoc />
    public void WarnFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Warn, eventID, format, args, null);
    }

    /// <inheritdoc />
    public void WarnFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Warn, null, format, args, exceptionObject);
    }

    /// <overloads><inheritdoc cref="ILog.Error(object)"/></overloads>
    /// <inheritdoc />
    public void Error (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Error, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    public void Error (int eventID, object message)
    {
      LogToLog4Net (Level.Error, eventID, message, null);
    }

    /// <overloads><inheritdoc cref="ILog.ErrorFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    public void ErrorFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Error, eventID, format, args, exceptionObject);
    }

    /// <inheritdoc />
    public void ErrorFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Error, eventID, format, args, null);
    }

    /// <inheritdoc />
    public void ErrorFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Error, null, format, args, exceptionObject);
    }

    /// <overloads><inheritdoc cref="ILog.Fatal(object)"/></overloads>
    /// <inheritdoc />
    public void Fatal (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Fatal, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    public void Fatal (int eventID, object message)
    {
      LogToLog4Net (Level.Fatal, eventID, message, null);
    }

    /// <overloads><inheritdoc cref="ILog.FatalFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    public void FatalFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Fatal, eventID, format, args, exceptionObject);
    }

    /// <inheritdoc />
    public void FatalFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Fatal, eventID, format, args, null);
    }

    /// <inheritdoc />
    public void FatalFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Fatal, null, format, args, exceptionObject);
    }


    private void LogToLog4NetFormat (Level level, int? eventID, string format, object[] args, Exception exceptionObject)
    {
      if (Logger.IsEnabledFor (level))
        LogToLog4Net (level, eventID, new SystemStringFormat (CultureInfo.InvariantCulture, format, args), exceptionObject);
    }

    private void LogToLog4Net (Level level, int? eventID, object message, Exception exceptionObject)
    {
      if (Logger.IsEnabledFor (level))
        Logger.Log (CreateLoggingEvent (level, eventID, message, exceptionObject));
    }

    private LoggingEvent CreateLoggingEvent (Level level, int? eventID, object message, Exception exceptionObject)
    {
      LoggingEvent loggingEvent = new LoggingEvent (typeof (Log4NetLog), null, Logger.Name, level, message, exceptionObject);

      if (eventID.HasValue)
        loggingEvent.Properties["EventID"] = eventID;

      return loggingEvent;
    }
  }
}