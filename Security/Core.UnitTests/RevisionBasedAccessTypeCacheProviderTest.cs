using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Collections;
using Rubicon.Development.UnitTesting;
using Rubicon.Security.Configuration;
using Rubicon.Security.UnitTests.Configuration;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class RevisionBasedAccessTypeCacheProviderTest
  {
    private MockRepository _mocks;
    private ISecurityService _mockSecurityService;
    private IGlobalAccessTypeCacheProvider _provider;

    [SetUp]
    public void SetUp()
    {
      _provider = new RevisionBasedAccessTypeCacheProvider();

      _mocks = new MockRepository();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      SecurityConfiguration.Current.SecurityService = _mockSecurityService;
      SetupResult.For (_mockSecurityService.IsNull).Return (false);
    }

    [TearDown]
    public void TearDown()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      System.Runtime.Remoting.Messaging.CallContext.SetData (typeof (RevisionBasedAccessTypeCacheProvider).AssemblyQualifiedName + "_Revision", null);
    }

    [Test]
    public void GetCache()
    {
      Expect.Call (_mockSecurityService.GetRevision()).Return (0);
      _mocks.ReplayAll();

      ICache<Tuple<SecurityContext, string>, AccessType[]> actual = _provider.GetCache();

      _mocks.VerifyAll();
      Assert.IsNotNull (actual);
    }

    [Test]
    public void GetCache_SameCacheTwice()
    {
      Expect.Call (_mockSecurityService.GetRevision()).Return (0);
      _mocks.ReplayAll();

      ICache<Tuple<SecurityContext, string>, AccessType[]> expected = _provider.GetCache();
      ICache<Tuple<SecurityContext, string>, AccessType[]> actual = _provider.GetCache();

      _mocks.VerifyAll();
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void GetCache_InvalidateFromOtherThread()
    {
      using (_mocks.Ordered())
      {
        Expect.Call (_mockSecurityService.GetRevision()).Return (0);
        Expect.Call (_mockSecurityService.GetRevision()).Return (1);
      }
      _mocks.ReplayAll();

      ICache<Tuple<SecurityContext, string>, AccessType[]> expected = _provider.GetCache();
      ICache<Tuple<SecurityContext, string>, AccessType[]> actual = null;

      ThreadRunner.Run (delegate() { actual = _provider.GetCache(); });

      _mocks.VerifyAll();
      Assert.AreNotSame (expected, actual);
    }

    [Test]
    public void GetCache_WithNullSecurityService()
    {
      SecurityConfiguration.Current.SecurityService = new NullSecurityService();

      Assert.AreSame (_provider.GetCache(), _provider.GetCache());
    }

    [Test]
    public void GetIsNull()
    {
      Assert.IsFalse (_provider.IsNull);
    }
  }
}