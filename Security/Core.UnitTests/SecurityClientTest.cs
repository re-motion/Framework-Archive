using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityClientTest
  {
    private Mockery _mocks;
    private ISecurityService _securityServiceMock;
    private SecurityContext _context;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _securityServiceMock = _mocks.NewMock<ISecurityService> ();
      _context = new SecurityContext ("Rubicon.Security.UnitTests.TestClass", "owner", "group", "client",
          new Dictionary<string, string> (), new List<string> ());
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner", new Enum[] { GeneralAccessType.Edit });

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Edit }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner", new Enum[] { GeneralAccessType.Create });

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasAccessWithMultipleAllowedAccessResults ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Create, GeneralAccessType.Delete, GeneralAccessType.Read }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner", new Enum[] { GeneralAccessType.Read });

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasAccessWithMultipleRequiredAccessTypes ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Create, GeneralAccessType.Delete, GeneralAccessType.Read }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner",
          new Enum[] { GeneralAccessType.Delete, GeneralAccessType.Create });

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasNotAccessWithMultipleRequiredAccessTypes ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new Enum[] { GeneralAccessType.Create, GeneralAccessType.Delete, GeneralAccessType.Read }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner",
          new Enum[] { GeneralAccessType.Delete, GeneralAccessType.Find });

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasMultipleAccessWithoutAllowedAccessResults ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new Enum[0]));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner",
          new Enum[] { GeneralAccessType.Find, GeneralAccessType.Edit, GeneralAccessType.Read });

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasMultipleAccessWithNull ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (null));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner",
          new Enum[] { GeneralAccessType.Find, GeneralAccessType.Edit, GeneralAccessType.Read });

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
