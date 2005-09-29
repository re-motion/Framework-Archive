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

/// <summary> 
///   Provides helper methods for initalizing a <see cref="WebConfiguration"/> object when simulating ASP.NET 
///   request cycles. 
/// </summary>
public class WebConfigurationMock: WebConfiguration
{
  public WebConfigurationMock()
  {
  }
  
  public static new WebConfiguration Current
  {
    get { return WebConfiguration.Current; }
    set {PrivateInvoke.SetNonPublicStaticField (typeof (WebConfiguration), "s_current", value); }
  }
}

}
