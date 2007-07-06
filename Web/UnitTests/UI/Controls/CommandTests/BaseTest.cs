using System;
using NUnit.Framework;
using Rubicon.Security;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.Web.UnitTests.UI.Controls.CommandTests
{
  public class BaseTest
  {
    [TearDown]
    public virtual void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
      WebConfigurationMock.Current = null;
      Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
    }
  }
}