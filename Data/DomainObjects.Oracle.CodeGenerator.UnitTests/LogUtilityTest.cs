using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using log4net.Appender;
using log4net.Config;
using log4net.Core;

namespace Rubicon.Data.DomainObjects.Oracle.CodeGenerator.UnitTests
{
  [TestFixture]
  public class LogUtilityTest
  {
    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (_memoryAppender);
    }

    [Test]
    public void LogError ()
    {
      LogUtility.LogError ("Test message", new ApplicationException ("Test"));

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Error, events[0].Level);
      Assert.AreEqual ("Test message", events[0].RenderedMessage);
      Assert.IsNotNull (events[0].ExceptionObject);
    }

    [Test]
    public void LogWarning ()
    {
      LogUtility.LogWarning ("Test message");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Warn, events[0].Level);
      Assert.AreEqual ("Test message", events[0].RenderedMessage);
    }
  }
}
