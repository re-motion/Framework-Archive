using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Security.Configuration;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class RevisionBasedAccessTypeCacheProviderTest
  {
    private MockRepository _mocks;
    private ISecurityService _mockSecurityService;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();

      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();

      SecurityConfiguration.Current.SecurityService = _mockSecurityService;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityService = new NullSecurityService ();
      System.Runtime.Remoting.Messaging.CallContext.SetData (typeof (RevisionBasedAccessTypeCacheProvider).AssemblyQualifiedName + "_Revision", null);
    }

    [Test]
    public void GetCache ()
    {
      IGlobalAccessTypeCacheProvider provider = new RevisionBasedAccessTypeCacheProvider ();
      Expect.Call (_mockSecurityService.GetRevision ()).Return (0);
      _mocks.ReplayAll ();

      ICache<Tuple<SecurityContext, string>, AccessType[]> actual = provider.GetCache ();

      _mocks.VerifyAll ();
      Assert.IsNotNull (actual);
    }

    [Test]
    public void GetCache_SameCacheTwice ()
    {
      IGlobalAccessTypeCacheProvider provider = new RevisionBasedAccessTypeCacheProvider ();
      Expect.Call (_mockSecurityService.GetRevision ()).Return (0);
      _mocks.ReplayAll ();

      ICache<Tuple<SecurityContext, string>, AccessType[]> expected = provider.GetCache ();
      ICache<Tuple<SecurityContext, string>, AccessType[]> actual = provider.GetCache ();

      _mocks.VerifyAll ();
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void GetCache_InvalidateFromOtherThread ()
    {
      IGlobalAccessTypeCacheProvider provider = new RevisionBasedAccessTypeCacheProvider ();
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockSecurityService.GetRevision ()).Return (0);
        Expect.Call (_mockSecurityService.GetRevision ()).Return (1);
      }
      _mocks.ReplayAll ();

      ICache<Tuple<SecurityContext, string>, AccessType[]> expected = provider.GetCache ();
      ICache<Tuple<SecurityContext, string>, AccessType[]> actual = null;

      ThreadRunner.Run (delegate ()
          {
            actual = provider.GetCache ();
          });

      _mocks.VerifyAll ();
      Assert.AreNotSame (expected, actual);
    }

    [Test]
    [ExpectedException (typeof (SecurityConfigurationException), "The security service has not been configured.")]
    public void GetCache_WithoutSecurityService ()
    {
      IGlobalAccessTypeCacheProvider provider = new RevisionBasedAccessTypeCacheProvider ();
      SecurityConfiguration.Current.SecurityService = new NullSecurityService ();

      provider.GetCache ();
    }

  }
}