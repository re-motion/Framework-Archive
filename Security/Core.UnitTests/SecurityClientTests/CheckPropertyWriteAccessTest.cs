using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CheckPropertyWriteAccessTest
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
    [ExpectedException (typeof (PermissionDeniedException))]
    public void OneCorrectAttribute_AccessDenied ()
    {
      SetupNamePropertyReadAccessTypes (GeneralAccessType.Find);
      SetupSecurityServiceResult (GeneralAccessType.Edit);
      _mocks.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (new SecurableObject (_contextFactoryMock), "Name", _user);
    }

    [Test]
    public void OneWrongAttribute_AccessAllowed ()
    {
      SetupNamePropertyReadAccessTypes (GeneralAccessType.Find);
      SetupSecurityServiceResult (GeneralAccessType.Edit, GeneralAccessType.Find);
      _mocks.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (new SecurableObject (_contextFactoryMock), "Name", _user);

      _mocks.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The member 'Name' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void NoAttributes_ThrowsException ()
    {
      SetupNamePropertyReadAccessTypes ();
      SetupSecurityServiceResult (GeneralAccessType.Edit, GeneralAccessType.Find);
      _mocks.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (new SecurableObject (_contextFactoryMock), "Name", _user);
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

    private void SetupNamePropertyReadAccessTypes (params Enum[] accessTypes)
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
