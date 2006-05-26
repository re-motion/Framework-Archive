using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class ObjectSecurityStrategyTest
  {
    private Mockery _mocks;
    private ISecurityService _securityServiceMock;
    private ISecurityContextFactory _contextFactoryMock;
    private IPrincipal _user;
    private SecurityContext _context;
    private ObjectSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _securityServiceMock = _mocks.NewMock<ISecurityService> ();
      _contextFactoryMock = _mocks.NewMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      Expect.Once.On (_contextFactoryMock)
          .Method ("GetSecurityContext")
          .Will (Return.Value (_context));

      _strategy = new ObjectSecurityStrategy (_contextFactoryMock);
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      bool hasAccess = _strategy.HasAccess (_securityServiceMock, _user, AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) }));

      bool hasAccess = _strategy.HasAccess (_securityServiceMock, _user, AccessType.Get (GeneralAccessType.Create));

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
          .With (_context, _user)
          .Will (Return.Value (mockResult));

      bool hasAccess = _strategy.HasAccess (_securityServiceMock, _user, AccessType.Get (GeneralAccessType.Read));

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
          .With (_context, _user)
          .Will (Return.Value (mockResult));

      bool hasAccess = _strategy.HasAccess (_securityServiceMock, _user, 
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
          .With (_context, _user)
          .Will (Return.Value (mockResult));

      bool hasAccess = _strategy.HasAccess (_securityServiceMock, _user,
          AccessType.Get (GeneralAccessType.Delete), AccessType.Get (GeneralAccessType.Find));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasMultipleAccessWithoutAllowedAccessResults ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (new AccessType[0]));

      bool hasAccess = _strategy.HasAccess (_securityServiceMock, _user,
          AccessType.Get (GeneralAccessType.Find), AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void HasMultipleAccessWithNull ()
    {
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (null));

      bool hasAccess = _strategy.HasAccess (_securityServiceMock, _user,
          AccessType.Get (GeneralAccessType.Find), AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
