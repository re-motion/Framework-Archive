using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.Web.UnitTests.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Security.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityProviderTestWithPermissionsFromInstanceMethod
  {
    // types

    // static members

    // member fields

    private IWxeSecurityProvider _securityProvider;
    private Mockery _mocks;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityService _securityService;
    private IUserProvider _userProvider;
    private IPrincipal _user;

    // construction and disposing

    public WxeSecurityProviderTestWithPermissionsFromInstanceMethod ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new WxeSecurityProvider ();

      _mocks = new Mockery ();

      _securityService = _mocks.NewMock<ISecurityService> ();
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _userProvider = _mocks.NewMock<IUserProvider> ();
      Stub.On (_userProvider)
          .Method ("GetUser")
          .WithNoArguments ()
          .Will (Return.Value (_user));

      _mockObjectSecurityStrategy = _mocks.NewMock<IObjectSecurityStrategy> ();
      _mockFunctionalSecurityStrategy = _mocks.NewMock<IFunctionalSecurityStrategy> ();

      SecurityConfiguration.Current.SecurityService = _securityService;
      SecurityConfiguration.Current.UserProvider = _userProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
    }

    [Test]
    public void CheckAccessGranted ()
    {
      Expect.Never.On (_mockFunctionalSecurityStrategy);
      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (true));

      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _securityProvider.CheckAccess (function);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckAccessDenied ()
    {
      Stub.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (false));

      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _securityProvider.CheckAccess (function);
    }

    [Test]
    public void HasAccessGranted ()
    {
      Expect.Never.On (_mockFunctionalSecurityStrategy);
      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (true));

      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      bool hasAccess = _securityProvider.HasAccess (function);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      Expect.Never.On (_mockFunctionalSecurityStrategy);
      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (false));

      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      bool hasAccess = _securityProvider.HasAccess (function);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasStatelessAccessGranted ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (true));

      bool hasAccess = _securityProvider.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromInstanceMethod));

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccessDenied ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (false));

      bool hasAccess = _securityProvider.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromInstanceMethod));

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException ), 
        "The property 'SecurityService' of the current 'Rubicon.Security.Configuration.SecurityConfiguration' evaluated and returned null.")]
    public void HasStatelessAccessWithSecurityServiceNull ()
    {
      Stub.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (true));

      SecurityConfiguration.Current.SecurityService = null;
      _securityProvider.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromInstanceMethod));
    }  
  }
}