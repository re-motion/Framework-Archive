using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class StorageProviderCollectionTest
{
  // types

  // static members and constants

  // member fields

  private StorageProviderCollection _collection;
  private StorageProvider _provider;

  // construction and disposing

  public StorageProviderCollectionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _provider = new SqlProvider (new RdbmsProviderDefinition ("TestDomain", typeof (SqlProvider), "ConnectionString"));
    _collection = new StorageProviderCollection ();
  }

  [Test]
  public void ContainsProviderTrue ()
  {
    _collection.Add (_provider);
    Assert.IsTrue (_collection.Contains (_provider));    
  }

  [Test]
  public void ContainsProviderFalse ()
  {
    _collection.Add (_provider);

    StorageProvider copy = new SqlProvider((RdbmsProviderDefinition) _provider.Definition);
    Assert.IsFalse (_collection.Contains (copy));    
  }

}
}
