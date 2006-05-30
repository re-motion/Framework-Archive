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
  public class WxeSecurityProviderTest
  {
    // types

    // static members

    // member fields

    private WxeSecurityProvider _securityProvider;
    private Mockery _mocks;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityService _securityService;
    private IUserProvider _userProvider;
    private IPrincipal _user;
    
    // construction and disposing

    public WxeSecurityProviderTest ()
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
    public void CheckAccessWithoutWxeDemandPermissionAttribute ()
    {
      TestFunctionWithoutPermissions function = new TestFunctionWithoutPermissions ();
      _securityProvider.CheckAccess (function);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void CheckAccessWithValidPermissionsFromInstanceMethod ()
    {
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
    public void CheckAccessWithInvalidPermissionsFromInstanceMethod ()
    {
      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (false));

      SecurableObject thisObject = new SecurableObject (_mockObjectSecurityStrategy);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod (thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      _securityProvider.CheckAccess (function);
    }

    [Test]
    public void CheckAccessWithValidPermissionsFromStaticMethod ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Search) })
          .Will (Return.Value (true));

      TestFunctionWithPermissionsFromStaticMethod function = new TestFunctionWithPermissionsFromStaticMethod ();
      _securityProvider.CheckAccess (function);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckAccessWithInvalidPermissionsFromStaticMethod ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Search) })
          .Will (Return.Value (false));

      TestFunctionWithPermissionsFromStaticMethod function = new TestFunctionWithPermissionsFromStaticMethod ();
      _securityProvider.CheckAccess (function);
    }

    [Test]
    public void CheckAccessWithValidPermissionsFromConstructor ()
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
          .Method ("HasAccess")
          .With (typeof (SecurableObject), _securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Search) })
          .Will (Return.Value (true));

      TestFunctionWithPermissionsFromStaticMethod function = new TestFunctionWithPermissionsFromStaticMethod ();
      _securityProvider.CheckAccess (function);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

  }
}