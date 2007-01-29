using System;

namespace Rubicon.Logging
{
  /// <summary>
  /// The <see cref="IExtendedLog"/> interface declares methods for logging messages.
  /// </summary>
  /// <remarks>
  /// The <see cref="IExtendedLog"/> interface is intended to implement adapters to various logging frameworks.
  /// Current implementation: <see cref="Log4NetLog"/>.
  /// </remarks>
  public interface IExtendedLog
  {
    void Info (int eventID, object message, Exception exceptionObject);

    void Info (int eventID, object message);

    void Info (object message, Exception exception);

    void Info (object message);

    void InfoFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    void InfoFormat (int eventID, string format, params object[] args);

    void InfoFormat (string format, params object[] args);


    void Debug (int eventID, object message, Exception exceptionObject);

    void Debug (int eventID, object message);

    void Debug (object message, Exception exception);

    void Debug (object message);

    void DebugFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    void DebugFormat (int eventID, string format, params object[] args);

    void DebugFormat (string format, params object[] args);


    void Warn (int eventID, object message, Exception exceptionObject);

    void Warn (int eventID, object message);

    void Warn (object message, Exception exception);

    void Warn (object message);

    void WarnFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    void WarnFormat (int eventID, string format, params object[] args);

    void WarnFormat (string format, params object[] args);


    void Error (int eventID, object message, Exception exceptionObject);

    void Error (int eventID, object message);

    void Error (object message, Exception exception);

    void Error (object message);

    void ErrorFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    void ErrorFormat (int eventID, string format, params object[] args);

    void ErrorFormat (string format, params object[] args);

    
    void Fatal (int eventID, object message, Exception exceptionObject);

    void Fatal (int eventID, object message);

    void Fatal (object message, Exception exception);

    void Fatal (object message);

    void FatalFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    void FatalFormat (int eventID, string format, params object[] args);

    void FatalFormat (string format, params object[] args);


    bool IsDebugEnabled { get; }

    bool IsInfoEnabled { get; }

    bool IsWarnEnabled { get; }

    bool IsErrorEnabled { get; }

    bool IsFatalEnabled { get; }
  }
}