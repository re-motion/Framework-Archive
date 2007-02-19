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
    private ISecurityProvider _mockSecurityProvider;
    private IGlobalAccessTypeCacheProvider _provider;

    [SetUp]
    public void SetUp()
    {
      _provider = new RevisionBasedAccessTypeCacheProvider();

      _mocks = new MockRepository();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      _mockSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
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
      Expect.Call (_mockSecurityProvider.GetRevision()).Return (0);
      _mocks.ReplayAll();

      ICache<Tuple<SecurityContext, string>, AccessType[]> actual = _provider.GetCache();

      _mocks.VerifyAll();
      Assert.IsNotNull (actual);
    }

    [Test]
    public void GetCache_SameCacheTwice()
    {
      Expect.Call (_mockSecurityProvider.GetRevision()).Return (0);
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
        Expect.Call (_mockSecurityProvider.GetRevision()).Return (0);
        Expect.Call (_mockSecurityProvider.GetRevision()).Return (1);
      }
      _mocks.ReplayAll();

      ICache<Tuple<SecurityContext, string>, AccessType[]> expected = _provider.GetCache();
      ICache<Tuple<SecurityContext, string>, AccessType[]> actual = null;

      ThreadRunner.Run (delegate() { actual = _provider.GetCache(); });

      _mocks.VerifyAll();
      Assert.AreNotSame (expected, actual);
    }

    [Test]
    public void GetCache_WithNullSecurityProvider()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider();

      Assert.AreSame (_provider.GetCache(), _provider.GetCache());
    }

    [Test]
    public void GetIsNull()
    {
      Assert.IsFalse (_provider.IsNull);
    }
  }
}