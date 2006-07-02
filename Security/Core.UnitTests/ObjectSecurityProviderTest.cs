using System;
using System.Collections.Generic;
using System.Text;

using Rhino.Mocks;
using NUnit.Framework;

using Rubicon.Security;
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
    private ISecurableObject _mockSecurableObject;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private ISecurityService _securityService;
    private IUserProvider _userProvider;
    private IPrincipal _user;
    private IPermissionProvider _permissionProvider;

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

      _securityService = _mocks.CreateMock<ISecurityService> ();
      _userProvider = _mocks.CreateMock<IUserProvider> ();
      _permissionProvider = _mocks.CreateMock<IPermissionProvider> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      SetupResult.For (_userProvider.GetUser ()).Return (_user);
      
      SecurityConfiguration.Current.SecurityService = _securityService;
      SecurityConfiguration.Current.UserProvider = _userProvider;
      SecurityConfiguration.Current.PermissionProvider = _permissionProvider;

      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy> ();
      _mockSecurableObject = _mocks.CreateMock<ISecurableObject> ();
      SetupResult.For (_mockSecurableObject.GetSecurityStrategy ()).Return (_mockObjectSecurityStrategy);
    }

    [Test]
    public void HasAccessOnGetAccessor_ValidAccessType ()
    {
      SetupAccessForRead (true);
      SetupNamePropertyReadPermissions ();
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccessOnGetAccessor (_mockSecurableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessor_InvalidAccessType ()
    {
      SetupAccessForRead (false);
      SetupNamePropertyReadPermissions ();
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccessOnGetAccessor (_mockSecurableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_ValidAccessType ()
    {
      SetupAccessForWrite (true);
      SetupNamePropertyWritePermissions ();
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccessOnSetAccessor (_mockSecurableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_InvalidAccessType ()
    {
      SetupAccessForWrite (false);
      SetupNamePropertyWritePermissions ();
      _mocks.ReplayAll ();

      bool hasAccess = _securityProvider.HasAccessOnSetAccessor (_mockSecurableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    private void SetupAccessForRead (bool accessAllowed)
    {
      SetupAccess (accessAllowed, GeneralAccessType.Read);
    }

    private void SetupAccessForWrite (bool accessAllowed)
    {
      SetupAccess (accessAllowed, GeneralAccessType.Edit);
    }

    private void SetupAccess (bool accessAllowed, params Enum[] expectedAccessTypes)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (expectedAccessTypes, new Converter<Enum, AccessType> (AccessType.Get));
      Expect.Call (_mockObjectSecurityStrategy.HasAccess (_securityService, _user, accessTypes)).Return (accessAllowed);
    }

    private void SetupNamePropertyReadPermissions (params Enum[] accessTypes)
    {
      Expect.Call (_permissionProvider.GetRequiredPropertyReadPermissions (typeof (object), "Name")).IgnoreArguments ().Return (accessTypes);
    }

    private void SetupNamePropertyWritePermissions (params Enum[] accessTypes)
    {
      Expect.Call (_permissionProvider.GetRequiredPropertyWritePermissions (typeof (object), "Name")).IgnoreArguments ().Return (accessTypes);
    }
  }
}