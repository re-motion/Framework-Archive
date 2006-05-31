using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityClientTestWithHasMethodAccess
  {
    private Mockery _mocks;
    private ISecurityService _securityServiceMock;
    private IPrincipal _user;
    private SecurityContext _context;
    private IPermissionProvider _permissionReflectorMock;
    private ISecurityContextFactory _contextFactoryMock;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _securityServiceMock = _mocks.NewMock<ISecurityService> ();
      _permissionReflectorMock = _mocks.NewMock<IPermissionProvider> ();
      _contextFactoryMock = _mocks.NewMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      Stub.On (_contextFactoryMock)
          .Method ("GetSecurityContext")
          .Will (Return.Value (_context));

      _securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock, new ThreadUserProvider (), new FunctionalSecurityStrategy ());
    }

    [Test]
    public void HasSuccessfulAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableObject), "Record")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (_contextFactoryMock), "Record", _user);
      
      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasDeniedAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableObject), "Record")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Read) }));

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (_contextFactoryMock), "Record", _user);
      
      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        "The method 'Save' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void HasAccessForMethodWithoutRequiredPermissions ()
    {
      Stub.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableObject), "Save")
          .Will (Return.Value (new Enum[0]));

      _securityClient.HasMethodAccess (new SecurableObject (_contextFactoryMock), "Save", _user);
    }

    [Test]
    public void HasSuccessfulStaticMethodAccess ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredStaticMethodPermissions")
          .With (typeof (SecurableObject), "CreateForSpecialCase")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      bool hasAccess = _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasDeniedAccessForStaticMethod ()
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredStaticMethodPermissions")
          .With (typeof (SecurableObject), "CreateForSpecialCase")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Read) }));

      bool hasAccess = _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The method 'CreateForSpecialCase' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void HasAccessForStaticMethodWithoutRequiredPermissions ()
    {
      Stub.On (_permissionReflectorMock)
          .Method ("GetRequiredStaticMethodPermissions")
          .With (typeof (SecurableObject), "CreateForSpecialCase")
          .Will (Return.Value (new Enum[0]));

      _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }
  }
}
