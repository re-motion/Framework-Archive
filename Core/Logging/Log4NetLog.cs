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


    public Log4NetLog (ILogger logger)
      : base (logger)
    {
    }


    public void Log (LogLevel logLevel, int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Convert (logLevel), eventID, message, exceptionObject);
    }

    public void Log (LogLevel logLevel, int eventID, object message)
    {
      LogToLog4Net (Convert (logLevel), eventID, message, null);
    }

    public void Log (LogLevel logLevel, object message, Exception exceptionObject)
    {
      LogToLog4Net (Convert (logLevel), null, message, exceptionObject);
    }

    public void Log (LogLevel logLevel, object message)
    {
      LogToLog4Net (Convert (logLevel), null, message, null);
    }

    public void LogFormat (LogLevel logLevel, int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Convert (logLevel), eventID, format, args, exceptionObject);
    }

    public void LogFormat (LogLevel logLevel, int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Convert (logLevel), eventID, format, args, null);
    }

    public void LogFormat (LogLevel logLevel, string format, params object[] args)
    {
      LogToLog4NetFormat (Convert (logLevel), null, format, args, null);
    }

    public void LogFormat (LogLevel logLevel, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Convert (logLevel), null, format, args, exceptionObject);
    }


    public void Debug (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Debug, eventID, message, exceptionObject);
    }

    public void Debug (int eventID, object message)
    {
      LogToLog4Net (Level.Debug, eventID, message, null);
    }

    public void DebugFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Debug, eventID, format, args, exceptionObject);
    }

    public void DebugFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Debug, eventID, format, args, null);
    }

    public void DebugFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Debug, null, format, args, exceptionObject);
    }


    public void Info (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Info, eventID, message, exceptionObject);
    }

    public void Info (int eventID, object message)
    {
      LogToLog4Net (Level.Info, eventID, message, null);
    }

    public void InfoFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Info, eventID, format, args, exceptionObject);
    }

    public void InfoFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Info, eventID, format, args, null);
    }

    public void InfoFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Info, null, format, args, exceptionObject);
    }


    public void Warn (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Warn, eventID, message, exceptionObject);
    }

    public void Warn (int eventID, object message)
    {
      LogToLog4Net (Level.Warn, eventID, message, null);
    }

    public void WarnFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Warn, eventID, format, args, exceptionObject);
    }

    public void WarnFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Warn, eventID, format, args, null);
    }

    public void WarnFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Warn, null, format, args, exceptionObject);
    }


    public void Error (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Error, eventID, message, exceptionObject);
    }

    public void Error (int eventID, object message)
    {
      LogToLog4Net (Level.Error, eventID, message, null);
    }

    public void ErrorFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Error, eventID, format, args, exceptionObject);
    }

    public void ErrorFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Error, eventID, format, args, null);
    }

    public void ErrorFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Error, null, format, args, exceptionObject);
    }


    public void Fatal (int eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Level.Fatal, eventID, message, exceptionObject);
    }

    public void Fatal (int eventID, object message)
    {
      LogToLog4Net (Level.Fatal, eventID, message, null);
    }

    public void FatalFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Fatal, eventID, format, args, exceptionObject);
    }

    public void FatalFormat (int eventID, string format, params object[] args)
    {
      LogToLog4NetFormat (Level.Fatal, eventID, format, args, null);
    }

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