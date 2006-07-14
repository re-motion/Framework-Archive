using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasPropertyWriteAccessTest
  {
    private MockRepository _mocks;
    private ISecurityService _securityServiceMock;
    private IPrincipal _user;
    private SecurityContext _context;
    private IPermissionProvider _permissionReflectorMock;
    private ISecurityContextFactory _contextFactoryMock;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _securityServiceMock = _mocks.CreateMock<ISecurityService> ();
      _permissionReflectorMock = _mocks.CreateMock<IPermissionProvider> ();
      _contextFactoryMock = _mocks.CreateMock<ISecurityContextFactory> ();

      _user = CreateDummyUser ();
      _context = CreateSimpleContext ();
      SetupResult.For (_contextFactoryMock.CreateSecurityContext ()).Return (_context);

      _securityClient = CreateSecurityClient ();
    }

    [Test]
    public void OneCorrentAccessType_HasAccess ()
    {
      SetupNamePropertyWriteAccessTypes (GeneralAccessType.Read);
      SetupSecurityServiceResult (GeneralAccessType.Edit, GeneralAccessType.Read);
      _mocks.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (new SecurableObject (_contextFactoryMock), "Name", _user);

      Assert.IsTrue (hasAccess);
      _mocks.VerifyAll ();
    }

    [Test]
    public void OneWrongAccessType_HasNotAccess ()
    {
      SetupNamePropertyWriteAccessTypes (GeneralAccessType.Read);
      SetupSecurityServiceResult (GeneralAccessType.Edit, GeneralAccessType.Find);
      _mocks.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (new SecurableObject (_contextFactoryMock), "Name", _user);

      Assert.IsFalse (hasAccess);
      _mocks.VerifyAll ();
    }

    [Test]
    public void DefaultAccessType_HasAccess ()
    {
      SetupNamePropertyWriteAccessTypes ();
      SetupSecurityServiceResult (GeneralAccessType.Edit);
      _mocks.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (new SecurableObject (_contextFactoryMock), "Name", _user);

      Assert.IsTrue (hasAccess);
      _mocks.VerifyAll ();
    }

    private IPrincipal CreateDummyUser ()
    {
      return new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
    }

    private SecurityContext CreateSimpleContext ()
    {
      return new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityClient CreateSecurityClient ()
    {
      return new SecurityClient (_securityServiceMock, _permissionReflectorMock, new ThreadUserProvider (), new FunctionalSecurityStrategy ());
    }

    private void SetupNamePropertyWriteAccessTypes (params Enum[] accessTypes)
    {
      Expect.Call (_permissionReflectorMock.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "Name")).Return (accessTypes);
    }

    private void SetupSecurityServiceResult (params Enum[] accessTypeEnums)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (accessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
      Expect.Call (_securityServiceMock.GetAccess (_context, _user)).Return (accessTypes);
    }
  }
}
