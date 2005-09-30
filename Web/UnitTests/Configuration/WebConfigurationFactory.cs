using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Utilities;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.UnitTests.Configuration
{

public class WebConfigurationFactory
{
  private WebConfigurationFactory()
  {
  }

  public static WebConfiguration GetDebugExceptionLevelA()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debugging = WcagDebugMode.Exception;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
    return config;
  }

  public static WebConfiguration GetDebugLoggingLevelA()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debugging = WcagDebugMode.Logging;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
    return config;
  }

  public static WebConfiguration GetDebugExceptionLevelDoubleA()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debugging = WcagDebugMode.Exception;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.DoubleA;
    return config;
  }

  public static WebConfiguration GetDebugExceptionLevelUndefined()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debugging = WcagDebugMode.Exception;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.Undefined;
    return config;
  }

  public static WebConfiguration GetLevelA()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debugging = WcagDebugMode.Disabled;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
    return config;
  }

  public static WebConfiguration GetLevelDoubleA()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debugging = WcagDebugMode.Disabled;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.DoubleA;
    return config;
  }

  public static WebConfiguration GetLevelUndefined()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debugging = WcagDebugMode.Disabled;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.Undefined;
    return config;
  }
}

}
