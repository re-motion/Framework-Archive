using System;
using System.Collections.Generic;
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Collections;
using Rubicon.Security.Configuration;
using Rubicon.Security.UnitTests.Configuration;
using Rubicon.Security.UnitTests.SampleDomain;
using Mocks_Is = Rhino.Mocks.Is;
using Mocks_List = Rhino.Mocks.List;
using Mocks_Property = Rhino.Mocks.Property;

namespace Rubicon.Security.UnitTests.SecurityStrategyTests
{
  using GlobalCacheKey = Tuple<SecurityContext, string>;

  [TestFixture]
  public class CachingTest
  {
    private MockRepository _mocks;
    private ISecurityProvider _mockSecurityProvider;
    private IGlobalAccessTypeCacheProvider _mockGlobalAccessTypeCacheProvider;
    private ICache<GlobalCacheKey, AccessType[]> _mockGlobalAccessTypeCache;
    private ICache<string, AccessType[]> _mockLocalAccessTypeCache;
    private ISecurityContextFactory _mockContextFactory;
    private IPrincipal _user;
    private SecurityContext _context;
    private GlobalCacheKey _globalAccessTypeCacheKey;
    private SecurityStrategy _strategy;

    [SetUp]
    public void SetUp()
    {
      _mocks = new MockRepository();
      _mockSecurityProvider = _mocks.CreateMock<ISecurityProvider>();
      _mockGlobalAccessTypeCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider>();
      _mockGlobalAccessTypeCache = _mocks.CreateMock<ICache<GlobalCacheKey, AccessType[]>>();
      _mockLocalAccessTypeCache = _mocks.CreateMock<ICache<string, AccessType[]>>();
      _mockContextFactory = _mocks.CreateMock<ISecurityContextFactory>();

      _user = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum>(), new Enum[0]);
      _globalAccessTypeCacheKey = new GlobalCacheKey (_context, _user.Identity.Name);

      _strategy = new SecurityStrategy (_mockLocalAccessTypeCache, _mockGlobalAccessTypeCacheProvider);

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [TearDown]
    public void TearDown()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [Test]
    public void Initialize()
    {
      Assert.AreSame (_mockLocalAccessTypeCache, _strategy.LocalCache);
      Assert.AreSame (_mockGlobalAccessTypeCacheProvider, _strategy.GlobalCacheProvider);
    }

    [Test]
    public void Initialize_WithDefaults()
    {
      IGlobalAccessTypeCacheProvider stubGlobalCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider>();
      SecurityConfiguration.Current.GlobalAccessTypeCacheProvider = stubGlobalCacheProvider;
      SecurityStrategy strategy = new SecurityStrategy();

      Assert.IsInstanceOfType (typeof (Cache<string, AccessType[]>), strategy.LocalCache);
      Assert.AreSame (stubGlobalCacheProvider, strategy.GlobalCacheProvider);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheAndNotInGlobalCacheAndAccessGranted()
    {
      AccessType[] accessTypeResult = new AccessType[] {AccessType.Get (GeneralAccessTypes.Edit)};
      using (_mocks.Ordered())
      {
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Mocks_Is.Equal ("user"), Mocks_Is.NotNull())
            .Do (GetOrCreateValueFromValueFactory<string>());
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetCache()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.CreateSecurityContext()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.NotNull())
            .Do (GetOrCreateValueFromValueFactory<GlobalCacheKey>());
        Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return (accessTypeResult);
      }
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheAndNotInGlobalCacheAndAccessDenied()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      using (_mocks.Ordered()){
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Mocks_Is.Equal ("user"), Mocks_Is.NotNull())
            .Do (GetOrCreateValueFromValueFactory<string>());
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetCache()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.CreateSecurityContext()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.NotNull())
            .Do (GetOrCreateValueFromValueFactory<GlobalCacheKey>());
        Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return (accessTypeResult);
      }
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheButInGlobalCacheAndAccessGranted()
    {
      AccessType[] accessTypeResult = new AccessType[] {AccessType.Get (GeneralAccessTypes.Edit)};
      using (_mocks.Ordered())
      {
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Mocks_Is.Equal ("user"), Mocks_Is.NotNull())
            .Do (GetOrCreateValueFromValueFactory<string>());
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetCache()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.CreateSecurityContext()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.NotNull())
            .Do (GetOrCreateValueFromFixedResult<GlobalCacheKey> (accessTypeResult));
      }
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheButInGlobalCacheAndAccessDenied()
    {
      using (_mocks.Ordered())
      {
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Mocks_Is.Equal ("user"), Mocks_Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactory<string>());
        Expect.Call (_mockGlobalAccessTypeCacheProvider.GetCache()).Return (_mockGlobalAccessTypeCache);
        Expect.Call (_mockContextFactory.CreateSecurityContext()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.NotNull ())
            .Do (GetOrCreateValueFromFixedResult<GlobalCacheKey> (new AccessType[0]));
      }
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.AreEqual (false, hasAccess);
    }
   
    [Test]
    public void HasAccess_WithResultInLocalCacheAndAccessGranted()
    {
      AccessType[] accessTypeResult = new AccessType[] {AccessType.Get (GeneralAccessTypes.Edit)};
      Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
          .Constraints (Mocks_Is.Equal ("user"), Mocks_Is.NotNull())
          .Do (GetOrCreateValueFromFixedResult<string> (accessTypeResult));
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithResultInLocalCacheAndAccessDenied()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
          .Constraints (Mocks_Is.Equal ("user"), Mocks_Is.NotNull())
          .Do (GetOrCreateValueFromFixedResult<string> (accessTypeResult));
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "IGlobalAccesTypeCacheProvider.GetAccessTypeCache() evaluated and returned null.")]
    public void HasAccess_WithGlobalCacheProviderReturningNull()
    {
      SetupResult.For (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
          .Constraints (Mocks_Is.Equal ("user"), Mocks_Is.NotNull())
          .Do (GetOrCreateValueFromValueFactory<string>());
      SetupResult.For (_mockGlobalAccessTypeCacheProvider.GetCache()).Return (null);
      _mocks.ReplayAll();

      _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Edit));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.")]
    public void HasAccess_WithSecurityContextFactoryReturningNull()
    {
      SetupResult.For (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
          .Constraints (Mocks_Is.Equal ("user"), Mocks_Is.NotNull())
          .Do (GetOrCreateValueFromValueFactory<string>());
      SetupResult.For (_mockGlobalAccessTypeCacheProvider.GetCache()).Return (_mockGlobalAccessTypeCache);
      SetupResult.For (_mockContextFactory.CreateSecurityContext()).Return (null);
      _mocks.ReplayAll();

      _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Edit));
    }

    [Test]
    public void InvalidateLocalCache()
    {
      _mockLocalAccessTypeCache.Clear();
      _mocks.ReplayAll();

      _strategy.InvalidateLocalCache();

      _mocks.VerifyAll();
    }

    /// <summary>
    /// Creates a delegate that simply returns the access types passed to this method.
    /// </summary>
    private Func<TKey, Func<TKey, AccessType[]>, AccessType[]> GetOrCreateValueFromFixedResult<TKey> (AccessType[] result)
    {
      return delegate { return result; };
    }

    /// <summary>
    /// Creates a delegate for ICache.GetOrCreateValue that calls the valueFactory and returns its result.
    /// </summary>
    private Func<TKey, Func<TKey, AccessType[]>, AccessType[]> GetOrCreateValueFromValueFactory<TKey> ()
    {
      return delegate (TKey key, Func<TKey, AccessType[]> valueFactory) { return valueFactory (key); };
    }
  }
}