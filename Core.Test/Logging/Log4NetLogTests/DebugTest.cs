using System;
using System.Reflection;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using Rubicon.Logging;

namespace Rubicon.Core.UnitTests.Logging.Log4NetLogTests
{
  [TestFixture]
  public class DebugingTest
  {
    private ILogger _logger;
    private IExtendedLog _log;
    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (_memoryAppender);

      _logger = LoggerManager.GetLogger (Assembly.GetCallingAssembly (), "The Name");
      _log = new Log4NetLog (_logger);
    }

    [TearDown]
    public void TearDown ()
    {
      LoggerManager.Shutdown ();
    }

    [Test]
    public void IsEnabled_WithLevelAll ()
    {
      _logger.Repository.Threshold = Level.All;
      Assert.IsTrue (_log.IsDebugEnabled);
    }

    [Test]
    public void IsEnabled_WithLevelDebug ()
    {
      _logger.Repository.Threshold = Level.Debug;
      Assert.IsTrue (_log.IsDebugEnabled);
    }

    [Test]
    public void IsEnabled_WithLevelWarn ()
    {
      _logger.Repository.Threshold = Level.Warn;
      Assert.IsFalse (_log.IsDebugEnabled);
    }

    [Test]
    public void Logger_Log ()
    {
      _logger.Repository.Threshold = Level.Debug;
      _logger.Log (GetType (), Level.Debug, "The message.", null);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Debug, events[0].Level);
      Assert.AreEqual ("The message.", events[0].MessageObject);
    }

    [Test]
    public void Test_WithMessageEventIDAndException ()
    {
      Exception exception = new Exception ();
      _logger.Repository.Threshold = Level.Debug;

      _log.Debug (1, (object) "The message.", exception);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Debug, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (1, loggingEvent.Properties["EventID"] = 1);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessageAndEventID ()
    {
      _logger.Repository.Threshold = Level.Debug;

      _log.Debug (1, (object) "The message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Debug, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (1, loggingEvent.Properties["EventID"] = 1);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessageAndException ()
    {
      Exception exception = new Exception ();
      _logger.Repository.Threshold = Level.Debug;

      _log.Debug ((object) "The message.", exception);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Debug, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessage ()
    {
      _logger.Repository.Threshold = Level.Debug;

      _log.Debug ((object) "The message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Debug, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithLogLevelNone ()
    {
      _logger.Repository.Threshold = Level.Off;

      _log.Debug (1, (object) "The message.");

      Assert.IsEmpty (_memoryAppender.GetEvents ());
    }

    [Test]
    public void Test_FormatWithMessageAndEventIDAndException ()
    {
      Exception exception = new Exception ();
      _logger.Repository.Threshold = Level.Debug;

      _log.DebugFormat (1, exception, "{0} {1}", "The", "message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Debug, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual (1, loggingEvent.Properties["EventID"] = 1);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessageAndEventID ()
    {
      _logger.Repository.Threshold = Level.Debug;

      _log.DebugFormat (1, "{0} {1}", "The", "message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Debug, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual (1, loggingEvent.Properties["EventID"] = 1);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessageAndException ()
    {
      Exception exception = new Exception ();
      _logger.Repository.Threshold = Level.Debug;

      _log.DebugFormat (exception, "{0} {1}", "The", "message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Debug, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessage ()
    {
      _logger.Repository.Threshold = Level.Debug;

      _log.DebugFormat (1, "{0} {1}", "The", "message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Debug, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithLogLevelNone ()
    {
      _logger.Repository.Threshold = Level.Off;

      _log.DebugFormat (1, "{0} {1}", "The", "message.");

      Assert.IsEmpty (_memoryAppender.GetEvents ());
    }
  }
}