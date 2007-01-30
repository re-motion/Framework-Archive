using NUnit.Framework;
using Rubicon.Logging;

namespace Rubicon.Core.UnitTests.Logging
{
  [TestFixture]
  public class Log4NetLogFactoryTest
  {
    [Test]
    public void GetLogger_WithNameAsString ()
    {
      IExtendedLogFactory logFactory = new Log4NetLogFactory ();
      
      IExtendedLog log = logFactory.GetLogger ("The Name");

      Assert.IsInstanceOfType (typeof (Log4NetLog), log);
      Log4NetLog log4NetLog = (Log4NetLog) log;
      Assert.AreEqual ("The Name", log4NetLog.Logger.Name);
    }

    [Test]
    public void GetLogger_WithNameFromType ()
    {
      IExtendedLogFactory logFactory = new Log4NetLogFactory ();

      IExtendedLog log = logFactory.GetLogger (typeof (SampleType));

      Assert.IsInstanceOfType (typeof (Log4NetLog), log);
      Log4NetLog log4NetLog = (Log4NetLog) log;
      Assert.AreEqual ("Rubicon.Core.UnitTests.Logging.SampleType", log4NetLog.Logger.Name);
    }
  }
}