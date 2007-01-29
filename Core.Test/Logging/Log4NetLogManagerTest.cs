using NUnit.Framework;
using Rubicon.Logging;

namespace Rubicon.Core.UnitTests.Logging
{
  [TestFixture]
  public class Log4NetLogManagerTest
  {
    [Test]
    public void GetLogger_WithNameAsString ()
    {
      IExtendedLogManager logManager = new Log4NetLogManager ();
      
      IExtendedLog log = logManager.GetLogger ("The Name");

      Assert.IsInstanceOfType (typeof (Log4NetLog), log);
      Log4NetLog log4NetLog = (Log4NetLog) log;
      Assert.AreEqual ("The Name", log4NetLog.Logger.Name);
    }

    [Test]
    public void GetLogger_WithNameFromType ()
    {
      IExtendedLogManager logManager = new Log4NetLogManager ();

      IExtendedLog log = logManager.GetLogger (typeof (SampleType));

      Assert.IsInstanceOfType (typeof (Log4NetLog), log);
      Log4NetLog log4NetLog = (Log4NetLog) log;
      Assert.AreEqual ("Rubicon.Core.UnitTests.Logging.SampleType", log4NetLog.Logger.Name);
    }
  }
}