using System;
using System.Collections.Specialized;
using System.Web;

using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.Web.UnitTests.UI.Controls.WebButtonTests
{
  public class BaseTest : WebControlTest
  {
    [TearDown]
    public override void TearDown ()
    {
      base.TearDown ();

      WebConfigurationMock.Current = null;
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (null);
    }
  }
}