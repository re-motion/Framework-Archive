using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using NMock2;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.Web.UnitTests.UI.Controls.CommandTests
{
  [TestFixture]
  public class SecurityTests
  {
    private CommandTestHelper _testHelper;
    private HttpContext _currentHttpContext;
    private HtmlTextWriterSingleTagMock _writer;
    private Mockery _mocks;
    private IWebSecurityProvider _mockWebSecurityProvider;
    private IWxeSecurityProvider _mockWxeSecurityProvider;

    [SetUp]
    public virtual void SetUp ()
    {
      _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
      HttpContextHelper.SetCurrent (_currentHttpContext);

      _writer = new HtmlTextWriterSingleTagMock ();

      _testHelper = new CommandTestHelper ();
      _mocks = new Mockery ();
      _mockWebSecurityProvider = _mocks.NewMock<IWebSecurityProvider> ();
      _mockWxeSecurityProvider = _mocks.NewMock<IWxeSecurityProvider> ();

      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (_mockWebSecurityProvider);
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_mockWxeSecurityProvider);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      WebConfigurationMock.Current = null;
      Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
    }

    [Test]
    public void IsActive_FromEventCommandAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWebSecurityProvider> (null);

      Expect.Never.On (_mockWebSecurityProvider);

      Command command = _testHelper.CreateEventCommand ();
      bool isActive = command.IsActive ();

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isActive);
    }

    [Test]
    public void IsActive_FromEventCommandAndAccessGranted ()
    {
      Expect.Once.On (_mockWebSecurityProvider)
         .Method ("HasAccess")
         .With (null, new CommandClickEventHandler (TestHandler))
         .Will (Return.Value (true));

      Command command = _testHelper.CreateEventCommand ();
      command.Click += TestHandler;
      bool isActive = command.IsActive ();

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isActive);
    }

    [Test]
    public void IsActive_FromEventCommandAndAccessDenied ()
    {
      Expect.Once.On (_mockWebSecurityProvider)
         .Method ("HasAccess")
         .With (null, new CommandClickEventHandler (TestHandler))
         .Will (Return.Value (false));

      Command command = _testHelper.CreateEventCommand ();
      command.Click += TestHandler;
      bool isActive = command.IsActive ();

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isActive);
    }

    [Test]
    public void IsActive_FromWxeFunctionCommandAndWithoutWebSeucrityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (null);

      Expect.Never.On (_mockWxeSecurityProvider);

      Command command = _testHelper.CreateWxeFunctionCommand ();
      bool isActive = command.IsActive ();

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isActive);
    }

    [Test]
    public void IsActive_FromWxeFunctionCommandAndAccessGranted ()
    {
      Expect.Once.On (_mockWxeSecurityProvider)
         .Method ("HasStatelessAccess")
         .With (typeof (ExecutionEngine.TestFunction))
         .Will (Return.Value (true));

      Command command = _testHelper.CreateWxeFunctionCommand ();
      bool isActive = command.IsActive ();

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (isActive);
    }

    [Test]
    public void IsActive_FromWxeFunctionCommandAndAccessDenied ()
    {
      Expect.Once.On (_mockWxeSecurityProvider)
         .Method ("HasStatelessAccess")
         .With (typeof (ExecutionEngine.TestFunction))
         .Will (Return.Value (false));

      Command command = _testHelper.CreateWxeFunctionCommand ();
      bool isActive = command.IsActive ();

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (isActive);
    }

    private void TestHandler (object sender, CommandClickEventArgs e)
    {
    }
  }
}