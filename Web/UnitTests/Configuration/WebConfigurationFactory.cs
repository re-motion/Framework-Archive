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

  public static WebConfiguration GetDebugLevelA()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debug = true;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
    return config;
  }

  public static WebConfiguration GetDebugLevelDoubleA()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debug = true;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.DoubleA;
    return config;
  }

  public static WebConfiguration GetDebugLevelUndefined()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debug = true;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.Undefined;
    return config;
  }

  public static WebConfiguration GetLevelA()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debug = false;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
    return config;
  }

  public static WebConfiguration GetLevelDoubleA()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debug = false;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.DoubleA;
    return config;
  }

  public static WebConfiguration GetLevelUndefined()
  {
    WebConfigurationMock config = new WebConfigurationMock();
    config.Wcag.Debug = false;
    config.Wcag.ConformanceLevel = WaiConformanceLevel.Undefined;
    return config;
  }
}

}
