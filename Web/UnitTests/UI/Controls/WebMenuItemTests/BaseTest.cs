using System;
using System.Collections.Specialized;
using System.Web;

using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.Web.UnitTests.UI.Controls.WebMenuItemTests
{
  public class BaseTest
  {
    [TearDown]
    public virtual void TearDown ()
    {
      WebConfigurationMock.Current = null;
      SecurityAdapterRegistry.Instance.SetAdapter<IWebSecurityAdapter> (null);
      SecurityAdapterRegistry.Instance.SetAdapter<IWxeSecurityAdapter> (null);
    }
  }
}