using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;

namespace Rubicon.Security.UnitTests.SecurityStrategyTests
{
  [TestFixture]
  public class CachingTest
  {
    private MockRepository _mocks;
    private ISecurityService _mockSecurityService;
    private IAccessTypeCache _mockAccessTypeCache;
    private IPrincipal _user;
    private SecurityContext _context;
    private SecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      _mockAccessTypeCache = _mocks.CreateMock<IAccessTypeCache> ();

      _user = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      _strategy = new SecurityStrategy (_mockAccessTypeCache);
    }

    [Test]
    public void HasAccess_WithResultNotInCacheAndAccessGranted ()
    {
      AccessType[] accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessType.Edit) };
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockAccessTypeCache.Get ("user")).Return (null);
        Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (accessTypeResult);
        _mockAccessTypeCache.Add ("user", accessTypeResult);
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_context, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_WithResultNotInCacheAndAccessDenied ()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockAccessTypeCache.Get ("user")).Return (null);
        Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (accessTypeResult);
        _mockAccessTypeCache.Add ("user", accessTypeResult);
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_context, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_WithResultAlreadyInCacheAndAccessGranted ()
    {
      Expect.Call (_mockAccessTypeCache.Get ("user")).Return (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) });
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_context, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_WithResultAlreadyInCacheAndAccessDenied ()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      Expect.Call (_mockAccessTypeCache.Get ("user")).Return (accessTypeResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_context, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAll ();
    }
  }
}
