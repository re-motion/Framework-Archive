using System;
using log4net.Core;
using NUnit.Framework;

namespace Rubicon.Core.UnitTests.Logging.Log4NetLogTests
{
  [TestFixture]
  public class WarningTest : BaseTest
  {
    [Test]
    public void IsEnabled_WithLevelInfo ()
    {
      SetLoggingThreshold (Level.Info);
      Assert.IsTrue (Log.IsWarnEnabled);
    }

    [Test]
    public void IsEnabled_WithLevelWarn ()
    {
      SetLoggingThreshold (Level.Warn);
      Assert.IsTrue (Log.IsWarnEnabled);
    }

    [Test]
    public void IsEnabled_WithLevelError ()
    {
      SetLoggingThreshold (Level.Error);
      Assert.IsFalse (Log.IsWarnEnabled);
    }

    [Test]
    public void Logger_Log ()
    {
      SetLoggingThreshold (Level.Warn);
      Logger.Log (GetType (), Level.Warn, "The message.", null);

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Warn, events[0].Level);
      Assert.AreEqual ("The message.", events[0].MessageObject);
    }

    [Test]
    public void Test_WithMessageEventIDAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Warn);

      Log.Warn (2, (object) "The message.", exception);

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (2, loggingEvent.Properties["EventID"]);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessageAndEventID ()
    {
      SetLoggingThreshold (Level.Warn);

      Log.Warn (1, (object) "The message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (1, loggingEvent.Properties["EventID"]);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessageAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Warn);

      Log.Warn ((object) "The message.", exception);

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessage ()
    {
      SetLoggingThreshold (Level.Warn);

      Log.Warn ((object) "The message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithLogLevelNone ()
    {
      Logger.Repository.Threshold = Level.Off;

      Log.Warn (1, (object) "The message.");

      Assert.IsEmpty (GetLoggingEvents ());
    }

    [Test]
    public void Test_FormatWithMessageAndEventIDAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Warn);

      Log.WarnFormat (1, exception, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual (1, loggingEvent.Properties["EventID"]);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessageAndEventID ()
    {
      SetLoggingThreshold (Level.Warn);

      Log.WarnFormat (1, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual (1, loggingEvent.Properties["EventID"]);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessageAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Warn);

      Log.WarnFormat (exception, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessage ()
    {
      SetLoggingThreshold (Level.Warn);

      Log.WarnFormat ("{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithEnumAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Warn);

      Log.WarnFormat (LogMessages.TheMessage, exception, "First", "Second");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message with First and Second.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual ((int) LogMessages.TheMessage, loggingEvent.Properties["EventID"]);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithEnum ()
    {
      SetLoggingThreshold (Level.Warn);

      Log.WarnFormat (LogMessages.TheMessage, "First", "Second");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message with First and Second.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual ((int) LogMessages.TheMessage, loggingEvent.Properties["EventID"]);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithLogLevelNone ()
    {
      Logger.Repository.Threshold = Level.Off;

      Log.WarnFormat (1, "{0} {1}", "The", "message.");

      Assert.IsEmpty (GetLoggingEvents ());
    }
  }
}