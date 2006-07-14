using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain;

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
      SetupResult.For (_stubContextFactory.GetSecurityContext ()).Return (_context);

      _strategy = new SecurityStrategy (new NullAccessTypeCache<string> (), new NullGlobalAccessTypeCacheProvider ());
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) });
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) });
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Create));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasAccessWithMultipleAllowedAccessResults ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessType.Create),
          AccessType.Get (GeneralAccessType.Delete),
          AccessType.Get (GeneralAccessType.Read) };

      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (mockResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Read));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessType.Create),
          AccessType.Get (GeneralAccessType.Delete),
          AccessType.Get (GeneralAccessType.Read) };

      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (mockResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user,
          AccessType.Get (GeneralAccessType.Delete), AccessType.Get (GeneralAccessType.Create));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasNotAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessType.Create),
          AccessType.Get (GeneralAccessType.Delete),
          AccessType.Get (GeneralAccessType.Read) };

      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return(mockResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user,
          AccessType.Get (GeneralAccessType.Delete), AccessType.Get (GeneralAccessType.Find));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasMultipleAccessWithoutAllowedAccessResults ()
    {
      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (new AccessType[0]);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user,
          AccessType.Get (GeneralAccessType.Find), AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasMultipleAccessWithNull ()
    {
      Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (null);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityService, _user,
          AccessType.Get (GeneralAccessType.Find), AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }
  }
}
