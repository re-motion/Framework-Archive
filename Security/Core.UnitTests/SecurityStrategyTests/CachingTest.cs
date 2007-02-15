using System;
using System.Collections.Generic;
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Collections;
using Rubicon.Security.Configuration;
using Rubicon.Security.UnitTests.Configuration;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests.SecurityStrategyTests
{
  [TestFixture]
  public class CachingTest
  {
    private MockRepository _mocks;
    private ISecurityService _mockSecurityService;
    private IGlobalAccessTypeCacheProvider _mockGlobalAccessTypeCacheProvider;
    private ICache<Tuple<SecurityContext, string>, AccessType[]> _mockGlobalAccessTypeCache;
    private ICache<string, AccessType[]> _mockLocalAccessTypeCache;
    private ISecurityContextFactory _mockContextFactory;
    private IPrincipal _user;
    private SecurityContext _context;
    private Tuple<SecurityContext, string> _globalAccessTypeCacheKey;
    private SecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      _mockGlobalAccessTypeCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider> ();
      _mockGlobalAccessTypeCache = _mocks.CreateMock<ICache<Tuple<SecurityContext, string>, AccessType[]>> ();
      _mockLocalAccessTypeCache = _mocks.CreateMock<ICache<string, AccessType[]>> ();
      _mockContextFactory = _mocks.CreateMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);
      _globalAccessTypeCacheKey = new Tuple<SecurityContext, string> (_context, _user.Identity.Name);

      _strategy = new SecurityStrategy (_mockLocalAccessTypeCache, _mockGlobalAccessTypeCacheProvider);
    
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
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

      Assert.IsInstanceOfType (typeof (Cache<string, AccessType[]>), strategy.LocalCache);
      Assert.AreSame (stubGlobalCacheProvider, strategy.GlobalCacheProvider);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheAndNotInGlobalCacheAndAccessGranted ()
    {
      AccessType[] accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessTypes.Edit) };
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Is.Equal ("user"), Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactoryForLocalCache ());
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetCache ()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.CreateSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Is.Equal (_globalAccessTypeCacheKey), Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactoryForGlobalCache ());
        Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (accessTypeResult);
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheAndNotInGlobalCacheAndAccessDenied ()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Is.Equal ("user"), Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactoryForLocalCache ());
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetCache ()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.CreateSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Is.Equal (_globalAccessTypeCacheKey), Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactoryForGlobalCache ());
        Expect.Call (_mockSecurityService.GetAccess (_context, _user)).Return (accessTypeResult);
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheButInGlobalCacheAndAccessGranted ()
    {
      AccessType[] accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessTypes.Edit) };
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Is.Equal ("user"), Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactoryForLocalCache ());
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetCache ()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.CreateSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Is.Equal (_globalAccessTypeCacheKey), Is.NotNull ())
            .Do (GetOrCreateValueFromFixedResultForGlobalCache (accessTypeResult));
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheButInGlobalCacheAndAccessDenied ()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Is.Equal ("user"), Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactoryForLocalCache ());
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetCache ()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.CreateSecurityContext ()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Is.Equal (_globalAccessTypeCacheKey), Is.NotNull ())
            .Do (GetOrCreateValueFromFixedResultForGlobalCache (accessTypeResult));
      }
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultInLocalCacheAndAccessGranted ()
    {
      AccessType[] accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessTypes.Edit) };
      Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
          .Constraints (Is.Equal ("user"), Is.NotNull ())
          .Do (GetOrCreateValueFromFixedResultForLocalCache (accessTypeResult));
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultInLocalCacheAndAccessDenied ()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
          .Constraints (Is.Equal ("user"), Is.NotNull ())
          .Do (GetOrCreateValueFromFixedResultForLocalCache (accessTypeResult));
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }
    
    [Test]
    [ExpectedException (typeof (InvalidOperationException), "IGlobalAccesTypeCacheProvider.GetAccessTypeCache() evaluated and returned null.")]
    public void HasAccess_WithGlobalCacheProviderReturningNull ()
    {
      SetupResult.For (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
          .Constraints (Is.Equal ("user"), Is.NotNull ())
          .Do (GetOrCreateValueFromValueFactoryForLocalCache ());
      SetupResult.For (_mockGlobalAccessTypeCacheProvider.GetCache ()).Return (null);
      _mocks.ReplayAll ();

      _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Edit));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.")]
    public void HasAccess_WithSecurityContextFactoryReturningNull ()
    {
      SetupResult.For (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
          .Constraints (Is.Equal ("user"), Is.NotNull ())
          .Do (GetOrCreateValueFromValueFactoryForLocalCache ());
      SetupResult.For (_mockGlobalAccessTypeCacheProvider.GetCache ()).Return (_mockGlobalAccessTypeCache);
      SetupResult.For (_mockContextFactory.CreateSecurityContext ()).Return (null);
      _mocks.ReplayAll ();

      _strategy.HasAccess (_mockContextFactory, _mockSecurityService, _user, AccessType.Get (GeneralAccessTypes.Edit));
    }

    [Test]
    public void InvalidateLocalCache ()
    {
      _mockLocalAccessTypeCache.Clear ();
      _mocks.ReplayAll ();

      _strategy.InvalidateLocalCache ();

      _mocks.VerifyAll ();
    }

    private Func<string, Func<AccessType[]>, AccessType[]> GetOrCreateValueFromFixedResultForLocalCache (AccessType[] accessTypeResult)
    {
      return delegate { return accessTypeResult; };
    }

    private Func<Tuple<SecurityContext, string>, Func<AccessType[]>, AccessType[]> GetOrCreateValueFromFixedResultForGlobalCache (AccessType[] accessTypeResult)
    {
      return delegate { return accessTypeResult; };
    }

    private Func<string, Func<AccessType[]>, AccessType[]> GetOrCreateValueFromValueFactoryForLocalCache ()
    {
      return delegate (string key, Func<AccessType[]> valueFactory) { return valueFactory (); };
    }

    private Func<Tuple<SecurityContext, string>, Func<AccessType[]>, AccessType[]> GetOrCreateValueFromValueFactoryForGlobalCache ()
    {
      return delegate (Tuple<SecurityContext, string> key, Func<AccessType[]> valueFactory) { return valueFactory (); };
    }
  }
}
