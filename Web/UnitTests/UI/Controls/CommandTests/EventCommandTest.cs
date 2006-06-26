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
  public class EventCommandTest : CommandTest
  {
    private CommandTestHelper _testHelper;
    private TestCommand _command;

    [SetUp]
    public virtual void SetUp ()
    {
      _testHelper = new CommandTestHelper ();
      _command = _testHelper.CreateEventCommand ();
      HttpContextHelper.SetCurrent (_testHelper.HttpContext);
    }

    [Test]
    public void IsActive_WithoutSeucrityProvider ()
    {
      _testHelper.ExpectWebSecurityProviderToBeNeverCalled ();

      bool isActive = _command.IsActive ();

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isActive);
    }

    [Test]
    public void IsActive_WithAccessGranted ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_testHelper.WebSecurityProvider);
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_testHelper.WxeSecurityProvider);
      _testHelper.ExpectWebSecurityProviderHasAccess (null, new CommandClickEventHandler (TestHandler), true);
      _command.Click += TestHandler;

      bool isActive = _command.IsActive ();

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isActive);
    }

    [Test]
    public void IsActive_WithAccessDenied ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_testHelper.WebSecurityProvider);
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_testHelper.WxeSecurityProvider);
      _testHelper.ExpectWebSecurityProviderHasAccess (null, new CommandClickEventHandler (TestHandler), false);
      _command.Click += TestHandler;

      bool isActive = _command.IsActive ();

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isActive);
    }

    [Test]
    public void Render_WithIsActiveTrue ()
    {
      _command.Active = true;
      _command.Click += TestHandler;
      string expectedOnClick = _testHelper.PostBackEvent + _testHelper.OnClick;
  
      _command.RenderBegin (_testHelper.HtmlWriter, _testHelper.PostBackEvent, new string[0], _testHelper.OnClick);

      _testHelper.VerifyAllExpectationsHaveBeenMet ();

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
    }

    [Test]
    public void Render_WithIsActiveFalse ()
    {
      _command.Active = false;
      _command.Click += TestHandler;

      _command.RenderBegin (_testHelper.HtmlWriter, _testHelper.PostBackEvent, new string[0], _testHelper.OnClick);

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (_command.IsActive (), "Active");
      Assert.IsNotNull (_testHelper.HtmlWriter.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _testHelper.HtmlWriter.Tag, "Wrong Tag");
      Assert.AreEqual (0, _testHelper.HtmlWriter.Attributes.Count, "Has Attributes");
    }

    private void TestHandler (object sender, CommandClickEventArgs e)
    {
    }
  }
}