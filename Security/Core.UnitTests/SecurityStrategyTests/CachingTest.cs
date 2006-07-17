using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Security.Configuration;
using Rubicon.Collections;

namespace Rubicon.Security.UnitTests.SecurityStrategyTests
{
  [TestFixture]
  public class CachingTest
  {
    private MockRepository _mocks;
    private ISecurityService _mockSecurityService;
    private IGlobalAccessTypeCacheProvider _mockGlobalAccessTypeCacheProvider;
    private IAccessTypeCache<Tupel<SecurityContext, string>> _mockGlobalAccessTypeCache;
    private IAccessTypeCache<string> _mockLocalAccessTypeCache;
    private ISecurityContextFactory _mockContextFactory;
    private IPrincipal _user;
    private SecurityContext _context;
    private Tupel<SecurityContext, string> _globalAccessTypeCacheKey;
    private SecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      _mockGlobalAccessTypeCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider> ();
      _mockGlobalAccessTypeCache = _mocks.CreateMock<IAccessTypeCache<Tupel<SecurityContext, string>>> ();
      _mockLocalAccessTypeCache = _mocks.CreateMock<IAccessTypeCache<string>> ();
      _mockContextFactory = _mocks.CreateMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);
      _globalAccessTypeCacheKey = new Tupel<SecurityContext, string> (_context, _user.Identity.Name);

      _strategy = new SecurityStrategy (_mockLocalAccessTypeCache, _mockGlobalAccessTypeCacheProvider);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_mockLocalAccessTypeCache, _strategy.LocalCache);
      Assert.AreSame (_mockGlobalAccessTypeCacheProvider, _strategy.GlobalCacheProvider);
    }

    [Test]
    public void Initialize_WithDefaults ()
    {
      IGlobalAccessTypeCacheProvider stubGlobalCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider> ();
      SecurityConfiguration.Current.GlobalAccessTypeCacheProvider = stubGlobalCacheProvider;
      SecurityStrategy strategy = new SecurityStrategy ();

      Assert.IsInstanceOfType (typeof (AccessTypeCache<string>), strategy.LocalCache);
      Assert.AreSame (stubGlobalCacheProvider, strategy.GlobalCacheProvider);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheAndNotInGlobalCacheAndAccessGranted ()
    {
      AccessType[] accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessType.Edit) };
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockLocalAccessTypeCache.Get ("user")).Return (null);
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetAccessTypeCache ()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.CreateSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.Get (_globalAccessTypeCacheKey)).Return (null);
        Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (accessTypeResult);
        _mockGlobalAccessTypeCache.Add (_globalAccessTypeCacheKey, accessTypeResult);
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
        Expect.Call (_mockContextFactory.CreateSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.Get (_globalAccessTypeCacheKey)).Return (null);
        Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (accessTypeResult);
        _mockGlobalAccessTypeCache.Add (_globalAccessTypeCacheKey, accessTypeResult);
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
        Expect.Call (_mockContextFactory.CreateSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.Get (_globalAccessTypeCacheKey)).Return (accessTypeResult);
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
        Expect.Call (_mockContextFactory.CreateSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.Get (_globalAccessTypeCacheKey)).Return (accessTypeResult);
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

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "IGlobalAccesTypeCacheProvider.GetAccessTypeCache() evaluated and returned null.")]
    public void HasAccess_WithGlobalCacheProviderReturningNull ()
    {
      SetupResult.For (_mockLocalAccessTypeCache.Get ("user")).Return (null);
      SetupResult.For (_mockGlobalAccessTypeCacheProvider.GetAccessTypeCache ()).Return (null);
      _mocks.ReplayAll ();

      _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.")]
    public void HasAccess_WithSecurityContextFactoryReturningNull ()
    {
      SetupResult.For (_mockLocalAccessTypeCache.Get ("user")).Return (null);
      SetupResult.For (_mockGlobalAccessTypeCacheProvider.GetAccessTypeCache ()).Return (_mockGlobalAccessTypeCache);
      SetupResult.For (_mockContextFactory.CreateSecurityContext ()).Return (null);
      _mocks.ReplayAll ();

      _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessType.Edit));
    }

    [Test]
    public void InvalidateLocalCache ()
    {
      _mockLocalAccessTypeCache.Clear ();
      _mocks.ReplayAll ();

      _strategy.InvalidateLocalCache ();

      _mocks.VerifyAll ();
    }
  }
}
