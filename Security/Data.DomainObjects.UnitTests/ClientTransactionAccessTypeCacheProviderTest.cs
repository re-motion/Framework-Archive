using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rubicon.Data.DomainObjects;

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
    public void GetAccessTypeCache_ExtensionIsRegistered ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction transaction = new ClientTransaction ();
      AccessTypeCacheClientTransactionExtension extension = new AccessTypeCacheClientTransactionExtension ();
      transaction.Extensions.Add (typeof (AccessTypeCacheClientTransactionExtension).FullName, extension);
      ClientTransaction.SetCurrent (transaction);

      Assert.AreSame (extension.Cache, provider.GetAccessTypeCache ());
    }

    [Test]
    public void GetAccessTypeCache_GetSameCacheTwice ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction.SetCurrent (new ClientTransaction ());

      Assert.AreSame (provider.GetAccessTypeCache (), provider.GetAccessTypeCache ());
    }

    [Test]
    public void GetAccessTypeCache_GetSameCacheTwiceAfterRollback ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction transaction = new ClientTransaction ();
      ClientTransaction.SetCurrent (transaction);

      IAccessTypeCache<GlobalAccessTypeCacheKey> first = provider.GetAccessTypeCache ();
      transaction.Rollback ();
      IAccessTypeCache<GlobalAccessTypeCacheKey> second = provider.GetAccessTypeCache ();

      Assert.AreSame (first, second);
    }

    [Test]
    public void GetAccessTypeCache_ExtensionIsNotRegistered ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction transaction = new ClientTransaction ();
      ClientTransaction.SetCurrent (transaction);

      Assert.IsNotNull (provider.GetAccessTypeCache ());
    }

    [Test]
    public void GetAccessTypeCache_NoClientTransaction ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction.SetCurrent (null);

      Assert.IsInstanceOfType (typeof (NullAccessTypeCache<GlobalAccessTypeCacheKey>), provider.GetAccessTypeCache ());
    }

    [Test]
    public void GetAccessTypeCache_GetSameCacheTwiceWithNoClientTransaction ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction.SetCurrent (null);

      Assert.AreSame (provider.GetAccessTypeCache (), provider.GetAccessTypeCache ());
    }

    [Test]
    public void GetAccessTypeCache_GetNotSameCacheTwiceWhenCurrentTransactionIsReset ()
    {
      ClientTransactionAccessTypeCacheProvider provider = new ClientTransactionAccessTypeCacheProvider ();
      ClientTransaction.SetCurrent (new ClientTransaction ());

      IAccessTypeCache<GlobalAccessTypeCacheKey> first = provider.GetAccessTypeCache ();
      ClientTransaction.SetCurrent (new ClientTransaction ());
      IAccessTypeCache<GlobalAccessTypeCacheKey> second = provider.GetAccessTypeCache ();

      Assert.IsNotNull (first);
      Assert.IsNotNull (second);
      Assert.AreNotSame (first, second);
    }
  }
}