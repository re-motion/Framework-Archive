using System;
using System.Reflection;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using Rubicon.Logging;

namespace Rubicon.Core.UnitTests.Logging.Log4NetLogImplementation
{
  [TestFixture]
  public class WarningTest
  {
    private ILogger _logger;
    private IExtendedLog _log;
    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (_memoryAppender);

      _logger = LoggerManager.GetLogger (Assembly.GetCallingAssembly(), "The Name");
      _log = new Log4NetLog (_logger);
    }

    [TearDown]
    public void TearDown ()
    {
      LoggerManager.Shutdown();  
    }

    [Test]
    public void Logger_Log ()
    {
      _logger.Repository.Threshold = Level.Warn;
      _logger.Log (GetType (), Level.Warn, "The message.", null);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Warn, events[0].Level);
      Assert.AreEqual ("The message.", events[0].MessageObject);
    }

    [Test]
    public void Test_WithMessageEventIDAndException ()
    {
      Exception exception = new Exception ();
      _logger.Repository.Threshold = Level.Warn;

      _log.Warn (1, (object) "The message.", exception);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (1, loggingEvent.Properties["EventID"] = 1);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessageAndEventID ()
    {
      _logger.Repository.Threshold = Level.Warn;

      _log.Warn (1, (object) "The message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (1, loggingEvent.Properties["EventID"] = 1);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithLogLevelNone ()
    {
      _logger.Repository.Threshold = Level.Off;

      _log.Warn (1, (object) "The message.");

      Assert.IsEmpty (_memoryAppender.GetEvents ());
    }

    [Test]
    public void Test_FormatWithMessageEventIDAndException ()
    {
      Exception exception = new Exception ();
      _logger.Repository.Threshold = Level.Warn;

      _log.WarnFormat (1, exception, "{0} {1}", "The", "message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (1, loggingEvent.Properties["EventID"] = 1);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessageEventID ()
    {
      _logger.Repository.Threshold = Level.Warn;

      _log.WarnFormat (1, "{0} {1}", "The", "message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Warn, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (1, loggingEvent.Properties["EventID"] = 1);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (_logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (_logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithLogLevelNone ()
    {
      _logger.Repository.Threshold = Level.Off;

      _log.WarnFormat (1, "{0} {1}", "The", "message.");

      Assert.IsEmpty (_memoryAppender.GetEvents ());
    }
  }
}