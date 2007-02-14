using System;
using System.Collections.Generic;
using System.Text;

using Rhino.Mocks;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Security.UnitTests.Configuration;
using Rubicon.Security.UnitTests.SampleDomain;
using System.Security.Principal;
using Rubicon.Security.Configuration;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class ObjectSecurityProviderTest
  {
    // types

    // static members

    // member fields

    private IObjectSecurityProvider _securityProvider;
    private MockRepository _mocks;
    private SecurableObject _securableObject;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private ISecurityService _mockSecurityService;
    private IUserProvider _mockUserProvider;
    private IPrincipal _user;
    private IPermissionProvider _mockPermissionProvider;

    // construction and disposing

    public ObjectSecurityProviderTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new ObjectSecurityProvider ();

      _mocks = new MockRepository ();

      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      SetupResult.For (_mockSecurityService.IsNull).Return (false);
      _mockUserProvider = _mocks.CreateMock<IUserProvider> ();
      _mockPermissionProvider = _mocks.CreateMock<IPermissionProvider> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      SetupResult.For (_mockUserProvider.GetUser ()).Return (_user);

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityService = _mockSecurityService;
      SecurityConfiguration.Current.UserProvider = _mockUserProvider;
      SecurityConfiguration.Current.PermissionProvider = _mockPermissionProvider;

      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy> ();
      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void HasAccessOnGetAccessor_AccessGranted ()
    {
      ExpectGetRequiredPropertyReadPermissions ("Name");
      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccessOnGetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessor_AccessDenied ()
    {
      ExpectGetRequiredPropertyReadPermissions ("Name");
      ExpectExpectObjectSecurityStrategyHasAccess (false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccessOnGetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessor_WithinSecurityFreeSeciton_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityProvider.HasAccessOnGetAccessor (_securableObject, "Name");
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_AccessGranted ()
    {
      ExpectGetRequiredPropertyWritePermissions ("Name");
      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccessOnSetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_AccessDenied ()
    {
      ExpectGetRequiredPropertyWritePermissions ("Name");
      ExpectExpectObjectSecurityStrategyHasAccess (false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccessOnSetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_WithinSecurityFreeSeciton_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityProvider.HasAccessOnSetAccessor (_securableObject, "Name");
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    private void ExpectExpectObjectSecurityStrategyHasAccess (bool accessAllowed)
    {
      AccessType[] accessTypes = new AccessType[] { AccessType.Get (TestAccessTypes.First) };
      Expect.Call (_mockObjectSecurityStrategy.HasAccess (_mockSecurityService, _user, accessTypes)).Return (accessAllowed);
    }

    private void ExpectGetRequiredPropertyReadPermissions (string propertyName)
    {
      Expect.Call (_mockPermissionProvider.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyName)).Return (new Enum[] { TestAccessTypes.First });
    }

    private void ExpectGetRequiredPropertyWritePermissions (string propertyName)
    {
      Expect.Call (_mockPermissionProvider.GetRequiredPropertyWritePermissions (typeof (SecurableObject), propertyName)).Return (new Enum[] { TestAccessTypes.First });
    }
  }
}