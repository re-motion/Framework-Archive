using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using log4net;
using log4net.spi;
using log4net.Filter;
using log4net.Appender;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

//public class TestAppender: AppenderSkeleton
//{
//  private string _message;
//
//  public string Message
//  {
//    get { return _message; }
//  }
//
//  protected override bool PreAppendCheck()
//  {
//    return base.PreAppendCheck ();
//  }
//
//  protected override void Append (log4net.spi.LoggingEvent loggingEvent)
//  {
//    _message += loggingEvent.RenderedMessage;
//  }
//}

[TestFixture]
public class BocReferenceValueTest
{
  private BocReferenceValueMock _bocReferenceValue;
//  private TestAppender _errorAppender;
//
//  public TestAppender ErrorAppender
//  {
//    get { return _errorAppender; }
//  }
//  log4net.Repository.Hierarchy.Logger _logger;

  public BocReferenceValueTest()
  {
  }

  
  [SetUp]
  public virtual void SetUp()
  {
    _bocReferenceValue = new BocReferenceValueMock();
    _bocReferenceValue.ID = "BocReferenceValue";
    _bocReferenceValue.ShowOptionsMenu = false;
    _bocReferenceValue.Command.Type = CommandType.None;
    _bocReferenceValue.Command.Show = CommandShow.Always;
    _bocReferenceValue.InternalValue = Guid.Empty.ToString();
// 
//    //  Init the static logger
//    bool temp = WcagUtility.IsWcagDebuggingEnabled();
//    _errorAppender = new TestAppender();
//    _errorAppender.Name = "ErrorAppender";
//    LevelRangeFilter errorFilter = new LevelRangeFilter ();
//    errorFilter.AcceptOnMatch = true;
//    errorFilter.LevelMin = Level.ERROR;
//    errorFilter.LevelMax = Level.ERROR;
//    _errorAppender.AddFilter (errorFilter);
//
//    foreach(log4net.ILog log in log4net.LogManager.GetCurrentLoggers())
//    {
//      if (log.Logger.Name == typeof (WcagUtility).FullName)
//      {
//        _logger = (log4net.Repository.Hierarchy.Logger)log.Logger;
//        _logger.AddAppender (_errorAppender);
//        //((log4net.Repository.Hierarchy.Hierarchy)_logger.Repository).Root.AddAppender (_errorAppender);
//        break;
//      }
//    }
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocReferenceValue.EvaluateWaiConformity ();
    // Assert.Succeed();
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.EvaluateWaiConformity ();
    //Assert.Succeed();
  }


	[Test]
  [ExpectedException (typeof (WcagException))]
  public void EvaluateWaiConformityDebugExceptionLevelAWithShowOptionsMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.EvaluateWaiConformity ();
    Assert.Fail();
  }
//
//	[Test]
//  public void EvaluateWaiConformityDebugLoggerLevelAWithShowOptionsMenuTrue()
//  {
//    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLoggingLevelA();
//    _bocReferenceValue.ShowOptionsMenu = true;
//    _bocReferenceValue.EvaluateWaiConformity ();
//    bool hasProperty = ErrorAppender.Message.IndexOf ("ShowOptionsMenu") != -1;
//    bool hasError = ErrorAppender.Message.IndexOf ("does not comply with a priority 1 checkpoint") != -1;
//    Assert.IsTrue (hasProperty);
//    Assert.IsTrue (hasError);
//  }

  [Test]
  public void IsOptionsMenuInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsFalse (_bocReferenceValue.HasOptionsMenu);
  }

  [Test]
  public void IsOptionsMenuVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsTrue (_bocReferenceValue.HasOptionsMenu);
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'Command' for BocReferenceValueMock 'BocReferenceValue' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithEventCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command.Type = CommandType.Event;
    _bocReferenceValue.EvaluateWaiConformity ();
    Assert.Fail();
  }

  [Test]
  public void IsEventCommandDisabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.Command.Type = CommandType.Event;
    Assert.IsFalse (_bocReferenceValue.IsCommandEnabled (false));
  }

  [Test]
  public void IsEventCommandEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocReferenceValue.Command.Type = CommandType.Event;
    Assert.IsTrue (_bocReferenceValue.IsCommandEnabled (false));
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'Command' for BocReferenceValueMock 'BocReferenceValue' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithWxeFunctionCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    _bocReferenceValue.EvaluateWaiConformity ();
    Assert.Fail();
  }

  [Test]
  public void IsWxeFunctionCommandDisabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    Assert.IsFalse (_bocReferenceValue.IsCommandEnabled (false));
  }

  [Test]
  public void IsWxeFunctionCommandEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    Assert.IsTrue (_bocReferenceValue.IsCommandEnabled (false));
  }

	
  [Test]
  public void EvaluateWaiConformityDebugLevelAWithHrefCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command.Type = CommandType.Href;
    _bocReferenceValue.EvaluateWaiConformity ();
    // Assert.Succeed();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithoutCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command = null;
    _bocReferenceValue.EvaluateWaiConformity ();
    // Assert.Succeed();
  }
}

}
