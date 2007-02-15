using System;
using NUnit.Framework;
using Rubicon.Security;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
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
      SecurityAdapterRegistry.Instance.SetAdapter<IWebSecurityAdapter> (null);
      SecurityAdapterRegistry.Instance.SetAdapter<IWxeSecurityAdapter> (null);
    }
  }
}