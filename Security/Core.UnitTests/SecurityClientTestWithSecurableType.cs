using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NMock2;

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

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _mockSecurityService = _mocks.NewMock<ISecurityService> ();
      _mockSecurableType = _mocks.NewMock<ISecurableType> ();
      _mockSecurityContextFactory = _mocks.NewMock<ISecurityContextFactory> ();
      _context = new SecurityContext ("Rubicon.Security.UnitTests.TestClass", "owner", "group", "client",
          new Dictionary<string, Enum> (), new Enum[0]);

      Expect.Once.On (_mockSecurityContextFactory)
          .Method ("GetSecurityContext")
          .Will (Return.Value (_context));
      Expect.Once.On (_mockSecurableType)
          .Method ("GetSecurityContextFactory")
          .Will (Return.Value (_mockSecurityContextFactory));
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Once.On (_mockSecurityService)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      SecurityClient securityClient = new SecurityClient (_mockSecurityService);
      bool hasAccess = securityClient.HasAccess (_mockSecurableType, "owner", AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Once.On (_mockSecurityService)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create), AccessType.Get (GeneralAccessType.Delete) }));

      SecurityClient securityClient = new SecurityClient (_mockSecurityService);
      bool hasAccess = securityClient.HasAccess (_mockSecurableType, "owner", AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
