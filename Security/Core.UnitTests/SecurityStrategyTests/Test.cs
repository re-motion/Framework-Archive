using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Collections;

namespace Rubicon.Security.UnitTests.SecurityStrategyTests
{
  [TestFixture]
  public class Test
  {
    private MockRepository _mocks;
    private ISecurityService _mockSecurityService;
    private ISecurityContextFactory _stubContextFactory;
    private IPrincipal _user;
    private SecurityContext _context;
    private SecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      _stubContextFactory = _mocks.CreateMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);
      SetupResult.For (_stubContextFactory.CreateSecurityContext ()).Return (_context);

      _strategy = new SecurityStrategy (new NullCache<string, AccessType[]> (), new NullGlobalAccessTypeCacheProvider ());
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (new AccessType[] { AccessType.Get (GeneralAccessTypes.Edit) });
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (new AccessType[] { AccessType.Get (GeneralAccessTypes.Edit) });
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Create));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasAccessWithMultipleAllowedAccessResults ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessTypes.Create),
          AccessType.Get (GeneralAccessTypes.Delete),
          AccessType.Get (GeneralAccessTypes.Read) };

      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (mockResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Read));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessTypes.Create),
          AccessType.Get (GeneralAccessTypes.Delete),
          AccessType.Get (GeneralAccessTypes.Read) };

      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (mockResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user,
          AccessType.Get (GeneralAccessTypes.Delete), AccessType.Get (GeneralAccessTypes.Create));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasNotAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessTypes.Create),
          AccessType.Get (GeneralAccessTypes.Delete),
          AccessType.Get (GeneralAccessTypes.Read) };

      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return(mockResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user,
          AccessType.Get (GeneralAccessTypes.Delete), AccessType.Get (GeneralAccessTypes.Find));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasMultipleAccessWithoutAllowedAccessResults ()
    {
      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (new AccessType[0]);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user,
          AccessType.Get (GeneralAccessTypes.Find), AccessType.Get (GeneralAccessTypes.Edit), AccessType.Get (GeneralAccessTypes.Read));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasMultipleAccessWithNull ()
    {
      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (null);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user,
          AccessType.Get (GeneralAccessTypes.Find), AccessType.Get (GeneralAccessTypes.Edit), AccessType.Get (GeneralAccessTypes.Read));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }
  }
}
