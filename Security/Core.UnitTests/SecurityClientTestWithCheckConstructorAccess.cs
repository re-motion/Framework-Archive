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
  public class SecurityClientTestWithCheckConstructorAccess
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
    public void CheckSuccessfulAccess ()
    {
      Expect.Never.On (_permissionReflectorMock);
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create) }));

      _securityClient.CheckConstructorAccess (typeof (SecurableObject), _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test, ExpectedException (typeof (PermissionDeniedException))]
    public void CheckDeniedAccess ()
    {
      Expect.Never.On (_permissionReflectorMock);
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Read) }));

      _securityClient.CheckConstructorAccess (typeof (SecurableObject), _user);
    }

    [Test]
    public void CheckAccessForOverloadedConstructor ()
    {
      Expect.Never.On (_permissionReflectorMock);
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Create) }));

      _securityClient.CheckConstructorAccess (typeof (SecurableObject), _user);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
