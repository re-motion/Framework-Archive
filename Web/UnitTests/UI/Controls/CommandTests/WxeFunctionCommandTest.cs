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
  public class WxeFunctionCommandTest : CommandTest
  {
    private CommandTestHelper _testHelper;
    private Command _command;

    [SetUp]
    public virtual void SetUp ()
    {
      _testHelper = new CommandTestHelper ();
      _command = _testHelper.CreateWxeFunctionCommand ();
      HttpContextHelper.SetCurrent (_testHelper.HttpContext);
    }

    [Test]
    public void IsActive_WithoutSeucrityProvider ()
    {
       _testHelper.ExpectWxeSecurityProviderToBeNeverCalled ();

      bool isActive = _command.IsActive ();

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isActive);
    }

    [Test]
    public void IsActive_WithAccessGranted ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_testHelper.WebSecurityProvider);
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_testHelper.WxeSecurityProvider);
      _testHelper.ExpectWxeSecurityProviderHasStatelessAccess (typeof (ExecutionEngine.TestFunction), true);

      bool isActive = _command.IsActive ();

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isActive);
    }

    [Test]
    public void IsActive_WithAccessDenied ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_testHelper.WebSecurityProvider);
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_testHelper.WxeSecurityProvider);
      _testHelper.ExpectWxeSecurityProviderHasStatelessAccess (typeof (ExecutionEngine.TestFunction), false);

      bool isActive = _command.IsActive ();

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isActive);
    }

    [Test]
    public void Render_WithoutSeucrityProvider ()
    {
      _testHelper.ExpectWxeSecurityProviderToBeNeverCalled ();
      
      _command.RenderBegin (_testHelper.HtmlWriter, _testHelper.PostBackEvent, new string[0], _testHelper.OnClick);

      string expectedOnClick = _testHelper.PostBackEvent + _testHelper.OnClick;

      Assert.IsTrue (_command.IsActive (), "Not active");

      Assert.IsNotNull (_testHelper.HtmlWriter.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _testHelper.HtmlWriter.Tag, "Wrong Tag");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
      Assert.AreEqual ("#", _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
      Assert.AreEqual (expectedOnClick, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
      Assert.AreEqual (_testHelper.ToolTip, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

      Assert.IsNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Target], "Has Target");

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}