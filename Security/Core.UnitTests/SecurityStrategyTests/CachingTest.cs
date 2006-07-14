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
  public class CachingTest
  {
    private MockRepository _mocks;
    private ISecurityService _mockSecurityService;
    private IGlobalAccessTypeCacheProvider _mockGlobalAccessTypeCacheProvider;
    private IAccessTypeCache<SecurityContext> _mockGlobalAccessTypeCache;
    private IAccessTypeCache<string> _mockLocalAccessTypeCache;
    private ISecurityContextFactory _mockContextFactory;
    private IPrincipal _user;
    private SecurityContext _context;
    private SecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      _mockGlobalAccessTypeCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider> ();
      _mockGlobalAccessTypeCache = _mocks.CreateMock<IAccessTypeCache<SecurityContext>> ();
      _mockLocalAccessTypeCache = _mocks.CreateMock<IAccessTypeCache<string>> ();
      _mockContextFactory = _mocks.CreateMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);

      _strategy = new SecurityStrategy (_mockLocalAccessTypeCache, _mockGlobalAccessTypeCacheProvider);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_mockLocalAccessTypeCache, _strategy.LocalCache);
      Assert.AreSame (_mockGlobalAccessTypeCacheProvider, _strategy.GlobalCacheProvider);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheAndNotInGlobalCacheAndAccessGranted ()
    {
      AccessType[] accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessType.Edit) };
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockLocalAccessTypeCache.Get ("user")).Return (null);
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetAccessTypeCache ()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.GetSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.Get (_context)).Return (null);
        Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (accessTypeResult);
        _mockGlobalAccessTypeCache.Add (_context, accessTypeResult);
        _mockLocalAccessTypeCache.Add ("user", accessTypeResult);
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheAndNotInGlobalCacheAndAccessDenied ()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockLocalAccessTypeCache.Get ("user")).Return (null);
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetAccessTypeCache ()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.GetSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.Get (_context)).Return (null);
        Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (accessTypeResult);
        _mockGlobalAccessTypeCache.Add (_context, accessTypeResult);
        _mockLocalAccessTypeCache.Add ("user", accessTypeResult);
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheButInGlobalCacheAndAccessGranted ()
    {
      AccessType[] accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessType.Edit) };
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockLocalAccessTypeCache.Get ("user")).Return (null);
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetAccessTypeCache ()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.GetSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.Get (_context)).Return (accessTypeResult);
        _mockLocalAccessTypeCache.Add ("user", accessTypeResult);
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheButInGlobalCacheAndAccessDenied ()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockLocalAccessTypeCache.Get ("user")).Return (null);
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetAccessTypeCache ()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.GetSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.Get (_context)).Return (accessTypeResult);
        _mockLocalAccessTypeCache.Add ("user", accessTypeResult);
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultInLocalCacheAndAccessGranted ()
    {
      Expect.Call (_mockLocalAccessTypeCache.Get ("user")).Return (new AccessType[] { AccessType.Get (GeneralAccessType.Edit) });
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultInLocalCacheAndAccessDenied ()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      Expect.Call (_mockLocalAccessTypeCache.Get ("user")).Return (accessTypeResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }
  }
}
