using System;
using log4net.Core;

namespace Rubicon.Logging
{
  /// <summary>
  ///   Implementation of interface <see cref="IExtendedLog"/> for <b>log4net</b>.
  /// </summary>
  public class Log4NetLog : LogImpl, IExtendedLog
  {
    public Log4NetLog (ILogger logger)
      : base (logger)
    {
    }


    public void Info (int eventID, object message, Exception exceptionObject)
    {
      Log (Level.Info, eventID, message, exceptionObject);
    }

    public void Info (int eventID, object message)
    {
      Info (eventID, message, null);
    }

    public void InfoFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (Level.Info, eventID, format, args, exceptionObject);
    }

    public void InfoFormat (int eventID, string format, params object[] args)
    {
      InfoFormat (eventID, null, format, args);
    }


    public void Debug (int eventID, object message, Exception exceptionObject)
    {
      Log (Level.Debug, eventID, message, exceptionObject);
    }

    public void Debug (int eventID, object message)
    {
      Debug (eventID, message, null);
    }

    public void DebugFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (Level.Debug, eventID, format, args, exceptionObject);
    }

    public void DebugFormat (int eventID, string format, params object[] args)
    {
      DebugFormat (eventID, null, format, args);
    }


    public void Warn (int eventID, object message, Exception exceptionObject)
    {
      Log (Level.Warn, eventID, message, exceptionObject);
    }

    public void Warn (int eventID, object message)
    {
      Warn (eventID, message, null);
    }

    public void WarnFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (Level.Warn, eventID, format, args, exceptionObject);
    }

    public void WarnFormat (int eventID, string format, params object[] args)
    {
      WarnFormat (eventID, null, format, args);
    }


    public void Error (int eventID, object message, Exception exceptionObject)
    {
      Log (Level.Error, eventID, message, exceptionObject);
    }

    public void Error (int eventID, object message)
    {
      Error (eventID, message, null);
    }

    public void ErrorFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (Level.Error, eventID, format, args, exceptionObject);
    }

    public void ErrorFormat (int eventID, string format, params object[] args)
    {
      ErrorFormat (eventID, null, format, args);
    }


    public void Fatal (int eventID, object message, Exception exceptionObject)
    {
      Log (Level.Fatal, eventID, message, exceptionObject);
    }

    public void Fatal (int eventID, object message)
    {
      Fatal (eventID, message, null);
    }

    public void FatalFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (Level.Fatal, eventID, format, args, exceptionObject);
    }

    public void FatalFormat (int eventID, string format, params object[] args)
    {
      FatalFormat (eventID, null, format, args);
    }


    private void LogFormat (Level level, int eventID, string format, object[] args, Exception exceptionObject)
    {
      if (Logger.IsEnabledFor (level))
        Log (level, eventID, string.Format (format, args), exceptionObject);
    }

    private void Log (Level level, int eventID, object message, Exception exceptionObject)
    {
      if (Logger.IsEnabledFor (level))
        Logger.Log (CreateLoggingEvent (level, eventID, message, exceptionObject));
    }

    private LoggingEvent CreateLoggingEvent (Level level, int eventID, object message, Exception exceptionObject)
    {
      LoggingEvent loggingEvent = new LoggingEvent (typeof (Log4NetLog), null, Logger.Name, level, message, exceptionObject);
      loggingEvent.Properties["EventID"] = eventID;

      return loggingEvent;
    }
  }
}