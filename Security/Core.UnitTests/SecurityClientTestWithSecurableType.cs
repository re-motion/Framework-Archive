using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security.UnitTests.Domain;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityClientTestWithSecurableType
  {
    private Mockery _mocks;
    private ISecurableType _mockSecurableType;
    private ISecurityContextFactory _mockSecurityContextFactory;
    private ISecurityService _mockSecurityService;
    private SecurityContext _context;
    private IPrincipal _user;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _mockSecurityService = _mocks.NewMock<ISecurityService> ();
      _mockSecurableType = _mocks.NewMock<ISecurableType> ();
      _mockSecurityContextFactory = _mocks.NewMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (File), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      Expect.Once.On (_mockSecurityContextFactory)
          .Method ("GetSecurityContext")
          .Will (Return.Value (_context));
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Once.On (_mockSecurableType)
          .Method ("GetSecurityContextFactory")
          .Will (Return.Value (_mockSecurityContextFactory));
      Expect.Once.On (_mockSecurityService)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      SecurityClient securityClient = new SecurityClient (_mockSecurityService);
      bool hasAccess = securityClient.HasAccess (_mockSecurableType, _user, AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Once.On (_mockSecurableType)
          .Method ("GetSecurityContextFactory")
          .Will (Return.Value (_mockSecurityContextFactory));
      Expect.Once.On (_mockSecurityService)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create), AccessType.Get (GeneralAccessType.Delete) }));

      SecurityClient securityClient = new SecurityClient (_mockSecurityService);
      bool hasAccess = securityClient.HasAccess (_mockSecurableType, _user, AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof(ArgumentException),
         "The securable type did not return a ISecurityContextFactory.\r\nParameter name: securableType")]
    public void SecurityContextFactoryIsNull ()
    {
      Expect.Once.On (_mockSecurableType)
          .Method ("GetSecurityContextFactory")
          .Will (Return.Value (null));
      Expect.Never.On (_mockSecurityService)
          .Method ("GetAccess");

      SecurityClient securityClient = new SecurityClient (_mockSecurityService);
      bool hasAccess = securityClient.HasAccess (_mockSecurableType, _user, AccessType.Get (GeneralAccessType.Edit));
    }
  }
}
