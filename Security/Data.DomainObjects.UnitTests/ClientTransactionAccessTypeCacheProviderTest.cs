using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Collections;

namespace Rubicon.Security.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ClientTransactionAccessTypeCacheProviderTest
  {
    [TearDown]
    public void TearDown ()
    {
      ClientTransaction.SetCurrent (null);
    }

    [Test]
    public void GetCache_CacheIsRegistered ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction transaction = new ClientTransaction ();
      Cache<Tupel<SecurityContext, string>, AccessType[]> cache = new Cache<Tupel<SecurityContext, string>, AccessType[]> ();
      transaction.ApplicationData.Add (typeof (ClientTransactionAccessTypeCacheProvider).FullName, cache);
      ClientTransaction.SetCurrent (transaction);

      Assert.AreSame (cache, provider.GetCache ());
    }

    [Test]
    public void GetCache_GetSameCacheTwice ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction.SetCurrent (new ClientTransaction ());

      Assert.AreSame (provider.GetCache (), provider.GetCache ());
    }

    [Test]
    public void GetCache_GetSameCacheTwiceAfterRollback ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction transaction = new ClientTransaction ();
      ClientTransaction.SetCurrent (transaction);

      ICache<Tupel<SecurityContext, string>, AccessType[]> first = provider.GetCache ();
      transaction.Rollback ();
      ICache<Tupel<SecurityContext, string>, AccessType[]> second = provider.GetCache ();

      Assert.AreSame (first, second);
    }

    [Test]
    public void GetCache_CacheIsNotRegistered ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction transaction = new ClientTransaction ();
      ClientTransaction.SetCurrent (transaction);

      Assert.IsNotNull (provider.GetCache ());
    }

    [Test]
    public void GetCache_NoClientTransaction ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction.SetCurrent (null);

      Assert.IsInstanceOfType (typeof (NullCache<Tupel<SecurityContext, string>, AccessType[]>), provider.GetCache ());
    }

    [Test]
    public void GetCache_GetSameCacheTwiceWithNoClientTransaction ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction.SetCurrent (null);

      Assert.AreSame (provider.GetCache (), provider.GetCache ());
    }

    [Test]
    public void GetCache_GetNotSameCacheTwiceWhenCurrentTransactionIsReset ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction.SetCurrent (new ClientTransaction ());

      ICache<Tupel<SecurityContext, string>, AccessType[]> first = provider.GetCache ();
      ClientTransaction.SetCurrent (new ClientTransaction ());
      ICache<Tupel<SecurityContext, string>, AccessType[]> second = provider.GetCache ();

      Assert.IsNotNull (first);
      Assert.IsNotNull (second);
      Assert.AreNotSame (first, second);
    }
  }
}