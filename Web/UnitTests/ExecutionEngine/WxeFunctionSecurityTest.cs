using System;
using System.Security.Principal;

using NMock2;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.UnitTests.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

  [TestFixture]
  public class WxeFunctionSecurityTest : WxeTest
  {
    private Mockery _mocks;
    private IWxeSecurityProvider _mockWxeSecurityProvider;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _mocks = new Mockery ();
      _mockWxeSecurityProvider = _mocks.NewMock<IWxeSecurityProvider> ();

      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (_mockWxeSecurityProvider);
    }

    [Test]
    public void ExecuteFunction ()
    {
      TestFunction function = new TestFunction ();
      Expect.Once.On (_mockWxeSecurityProvider)
          .Method ("CheckAccess")
          .With (function);

      function.Execute ();

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException), "Test Exception")]
    public void ExecuteFunctionWithInvalidPermissions ()
    {
      TestFunction function = new TestFunction ();
      Expect.Once.On (_mockWxeSecurityProvider)
          .Method ("CheckAccess")
          .With (function)
          .Will (Throw.Exception (new PermissionDeniedException ("Test Exception")));

      function.Execute ();

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
    
    [Test]
    public void ExecuteFunctionWithoutWxeSecurityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IWxeSecurityProvider> (null);

      TestFunction function = new TestFunction ();
      Expect.Never.On (_mockWxeSecurityProvider);

      function.Execute ();

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
