using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasConstructorAccessTest
  {
    private Mockery _mocks;
    private ISecurityService _securityServiceMock;
    private IPermissionProvider _permissionReflectorMock;
    private IPrincipal _user;
    private SecurityContext _statelessContext;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _securityServiceMock = _mocks.NewMock<ISecurityService> ();
      _permissionReflectorMock = _mocks.NewMock<IPermissionProvider> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _statelessContext = new SecurityContext (typeof (SecurableObject));

      _securityClient = new SecurityClient (_securityServiceMock, _permissionReflectorMock, new ThreadUserProvider (), new FunctionalSecurityStrategy ());
    }

    [Test]
    public void HasSuccessfulAccess ()
    {
      Expect.Never.On (_permissionReflectorMock);
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create) }));

      bool hasAccess =_securityClient.HasConstructorAccess (typeof (SecurableObject), _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasDeniedAccess ()
    {
      Expect.Never.On (_permissionReflectorMock);
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Read) }));

      bool hasAccess = _securityClient.HasConstructorAccess (typeof (SecurableObject), _user);
      
      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessForOverloadedConstructor ()
    {
      Expect.Never.On (_permissionReflectorMock);
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Create) }));

      bool hasAccess = _securityClient.HasConstructorAccess (typeof (SecurableObject), _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }
  }
}
