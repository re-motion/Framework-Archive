using System;
using System.Security.Principal;

using Rhino.Mocks;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.UnitTests.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

  [TestFixture]
  public class WxeFunctionSecurityTest : WxeTest
  {
    private MockRepository _mocks;
    private IWxeSecurityProvider _mockWxeSecurityProvider;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _mocks = new MockRepository ();
      _mockWxeSecurityProvider = _mocks.CreateMock<IWxeSecurityProvider> ();

      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_mockWxeSecurityProvider);
    }

    [Test]
    public void ExecuteFunctionWithAccessGranted ()
    {
      TestFunction function = new TestFunction ();
      _mockWxeSecurityProvider.CheckAccess (function);
      _mocks.ReplayAll ();

      function.Execute ();

      _mocks.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException), "Test Exception")]
    public void ExecuteFunctionWithAccessDenied ()
    {
      TestFunction function = new TestFunction ();
      _mockWxeSecurityProvider.CheckAccess (function);
      LastCall.Throw (new PermissionDeniedException ("Test Exception"));
      _mocks.ReplayAll ();

      function.Execute ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void ExecuteFunctionWithoutWxeSecurityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (null);

      TestFunction function = new TestFunction ();
      _mocks.ReplayAll ();

      function.Execute ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void HasStatelessAccessGranted ()
    {
      Expect.Call (_mockWxeSecurityProvider.HasStatelessAccess (typeof (TestFunction))).Return (true);
      _mocks.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccessDenied ()
    {
      Expect.Call (_mockWxeSecurityProvider.HasStatelessAccess (typeof (TestFunction))).Return (false);
      _mocks.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasStatelessAccessGrantedWithoutWxeSecurityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (null);
      _mocks.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}
