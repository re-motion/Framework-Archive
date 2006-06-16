using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UnitTests.UI.Controls.CommandTests
{
  [TestFixture]
  public class NoneCommandTest
  {
    private CommandTestHelper _testHelper;
    private Command _command;

    [SetUp]
    public virtual void SetUp ()
    {
      _testHelper = new CommandTestHelper ();
      _command = _testHelper.CreateNoneCommand ();
      HttpContextHelper.SetCurrent (_testHelper.HttpContext);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
      WebConfigurationMock.Current = null;
      Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (null);
    }

    [Test]
    public void IsActive_FromNoneCommand ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_testHelper.WebSecurityProvider);
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_testHelper.WxeSecurityProvider);
      _testHelper.ExpectWxeSecurityProviderToBeNeverCalled ();
      _testHelper.ExpectWebSecurityProviderToBeNeverCalled ();
      
      bool isActive = _command.IsActive ();

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isActive);
    }

    [Test]
    public void RenderNoneCommand ()
    {
      _command.RenderBegin (_testHelper.HtmlWriter, _testHelper.PostBackEvent, new string[0], _testHelper.OnClick);

      Assert.IsTrue (_command.IsActive (), "Not active");

      Assert.IsNotNull (_testHelper.HtmlWriter.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _testHelper.HtmlWriter.Tag, "Wrong Tag");
      Assert.AreEqual (0, _testHelper.HtmlWriter.Attributes.Count, "Has Attributes");
    }
  }
}