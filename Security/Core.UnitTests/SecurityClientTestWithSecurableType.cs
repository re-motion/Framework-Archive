using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using NUnit.Framework;
using NMock2;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.TestDomain;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityClientTestWithSecurableType
  {
    private Mockery _mocks;
    private ISecurableObject _mockSecurableType;
    private IObjectSecurityStrategy _mockSecurityStrategy;
    private ISecurityService _mockSecurityService;
    private SecurityContext _context;
    private IPrincipal _user;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _mockSecurableType = _mocks.NewMock<ISecurableObject> ();
      _mockSecurityStrategy = _mocks.NewMock<IObjectSecurityStrategy> ();
      _mockSecurityService = _mocks.NewMock<ISecurityService> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (File), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      _securityClient = new SecurityClient (_mockSecurityService, new PermissionReflector (), new ThreadUserProvider (), new FunctionalSecurityStrategy ());
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Once.On (_mockSecurityStrategy)
          .Method ("HasAccess")
          .With (_mockSecurityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Edit) })
          .Will (Return.Value (true));
      Expect.Once.On (_mockSecurableType)
          .Method ("GetSecurityStrategy")
          .Will (Return.Value (_mockSecurityStrategy));

      bool hasAccess = _securityClient.HasAccess (_mockSecurableType, _user, AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Once.On (_mockSecurityStrategy)
          .Method ("HasAccess")
          .With (_mockSecurityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Edit) })
          .Will (Return.Value (false));
      Expect.Once.On (_mockSecurableType)
          .Method ("GetSecurityStrategy")
          .Will (Return.Value (_mockSecurityStrategy));

      bool hasAccess = _securityClient.HasAccess (_mockSecurableType, _user, AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The securable object did not return a IObjectSecurityStrategy.\r\nParameter name: securableObject")]
    public void SecurityStrategyIsNull ()
    {
      Expect.Once.On (_mockSecurableType)
          .Method ("GetSecurityStrategy")
          .Will (Return.Value (null));
      Expect.Never.On (_mockSecurityService)
          .Method ("GetAccess");

      bool hasAccess = _securityClient.HasAccess (_mockSecurableType, _user, AccessType.Get (GeneralAccessType.Edit));
    }
  }
}
