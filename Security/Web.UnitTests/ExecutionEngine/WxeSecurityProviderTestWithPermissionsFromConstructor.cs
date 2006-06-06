using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityProviderTestWithPermissionsFromConstructor
  {
    // types

    // static members

    // member fields

    private IWxeSecurityProvider _securityProvider;
    private Mockery _mocks;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityService _securityService;
    private IUserProvider _userProvider;
    private IPrincipal _user;

    // construction and disposing

    public WxeSecurityProviderTestWithPermissionsFromConstructor ()
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
      _mockFunctionalSecurityStrategy = _mocks.NewMock<IFunctionalSecurityStrategy> ();

      SecurityConfiguration.Current.SecurityService = _securityService;
      SecurityConfiguration.Current.UserProvider = _userProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityService = null;
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      SecurityConfiguration.Current.FunctionalSecurityStrategy = new FunctionalSecurityStrategy ();
    }

    [Test]
    public void CheckAccessGranted ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Create) })
          .Will (Return.Value (true));

      _securityProvider.CheckAccess (new TestFunctionWithPermissionsFromConstructor ());
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckAccessDenied ()
    {
      Stub.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Create) })
          .Will (Return.Value (false));

      _securityProvider.CheckAccess (new TestFunctionWithPermissionsFromConstructor ());
    }

    [Test]
    public void HasAccessGranted ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Create) })
          .Will (Return.Value (true));

      bool hasAccess = _securityProvider.HasAccess (new TestFunctionWithPermissionsFromConstructor ());

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Create) })
          .Will (Return.Value (false));

      bool hasAccess = _securityProvider.HasAccess (new TestFunctionWithPermissionsFromConstructor ());

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasStatelessAccessGranted ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Create) })
          .Will (Return.Value (true));

      bool hasAccess = _securityProvider.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromConstructor));

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccessDenied ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Create) })
          .Will (Return.Value (false));

      bool hasAccess = _securityProvider.HasStatelessAccess (typeof (TestFunctionWithPermissionsFromConstructor));

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }
  }
}