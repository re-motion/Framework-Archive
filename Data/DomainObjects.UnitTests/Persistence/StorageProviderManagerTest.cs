using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class StorageProviderManagerTest
{
  // types

  // static members and constants

  // member fields

  private StorageProviderManager _storageProviderManager;

  // construction and disposing

  public StorageProviderManagerTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _storageProviderManager = new StorageProviderManager ();
  }

  [TearDown]
  public void TearDown ()
  {
    _storageProviderManager.Dispose ();
  }

  [Test]
  public void LookUp ()
  {
    StorageProvider provider = _storageProviderManager[DatabaseTest.c_testDomainProviderID];

    Assert.IsNotNull (provider);
    Assert.AreEqual (typeof (SqlProvider), provider.GetType ());
    Assert.AreEqual (DatabaseTest.c_testDomainProviderID, provider.ID);
  }

  [Test]
  public void Reference ()
  {
    StorageProvider provider1 = _storageProviderManager[DatabaseTest.c_testDomainProviderID];
    StorageProvider provider2 = _storageProviderManager[DatabaseTest.c_testDomainProviderID];

    Assert.AreSame (provider1, provider2);
  }

  [Test]
  public void Disposing ()
  {
    RdbmsProvider provider = null;

    using (_storageProviderManager)
    {
      provider = (RdbmsProvider) _storageProviderManager[DatabaseTest.c_testDomainProviderID];
      provider.LoadDataContainer (DomainObjectIDs.Order1);

      Assert.IsTrue (provider.IsConnected);
    }

    Assert.IsFalse (provider.IsConnected);
  }
}
}
