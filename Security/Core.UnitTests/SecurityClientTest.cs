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
          new Dictionary<string, Enum> (), new Enum[0]);
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner", AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner", AccessType.Get (GeneralAccessType.Create));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasAccessWithMultipleAllowedAccessResults ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessType.Create),
          AccessType.Get (GeneralAccessType.Delete),
          AccessType.Get (GeneralAccessType.Read) };

      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (mockResult));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner", AccessType.Get (GeneralAccessType.Read));

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessType.Create),
          AccessType.Get (GeneralAccessType.Delete),
          AccessType.Get (GeneralAccessType.Read) };

      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (mockResult));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner", 
          AccessType.Get (GeneralAccessType.Delete), AccessType.Get (GeneralAccessType.Create));

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasNotAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessType.Create),
          AccessType.Get (GeneralAccessType.Delete),
          AccessType.Get (GeneralAccessType.Read) };

      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (mockResult));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner", 
          AccessType.Get (GeneralAccessType.Delete), AccessType.Get (GeneralAccessType.Find));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasMultipleAccessWithoutAllowedAccessResults ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, "owner")
          .Will (Return.Value (new AccessType[0]));

      SecurityClient securityClient = new SecurityClient (_securityServiceMock);
      bool hasAccess = securityClient.HasAccess (_context, "owner",
          AccessType.Get (GeneralAccessType.Find), AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read));

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
          AccessType.Get (GeneralAccessType.Find), AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
