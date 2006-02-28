using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class ClassDefinitionCollectionWithUnresolvedTypeNamesTest
{
  // types

  // static members and constants

  // member fields

  private ClassDefinitionCollection _collection;
  private ClassDefinition _classDefinitionWithUnresolvedTypeName;

  // construction and disposing

  public ClassDefinitionCollectionWithUnresolvedTypeNamesTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _collection = new ClassDefinitionCollection (false);
    _classDefinitionWithUnresolvedTypeName = new ClassDefinition ("Order", "OrderTable", "StorageProvider", "UnresolvedTypeName", false);
  }

  
  [Test]
  public void InitializeWithUnresolvedTypeNames ()
  {
    Assert.IsFalse (_collection.AreResolvedTypeNamesRequired);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Collection allows only ClassDefinitions with resolved types and therefore this overload of the indexer cannot be used.")]
  public void TypeIndexer ()
  {
    ClassDefinition orderDefinition = _collection[typeof (Order)];    
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Collection allows only ClassDefinitions with resolved types and therefore Contains(Type) cannot be used.")]
  public void ContainsType ()
  {
    _collection.Contains (typeof (Order));    
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Collection allows only ClassDefinitions with resolved types and therefore GetMandatory(Type) cannot be used.")]
  public void GetMandatoryType ()
  {
    _collection.GetMandatory (typeof (Order));    
  }


  [Test]
  public void AddWithResolvedTypeName ()
  {
    Assert.AreEqual (0, _collection.Count);

    ClassDefinition classDefinition = new ClassDefinition ("Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order));
    _collection.Add (classDefinition);

    Assert.AreEqual (1, _collection.Count);
  }

  [Test]
  public void AddWithUnresolvedTypeName ()
  {
    Assert.AreEqual (0, _collection.Count);

    _collection.Add (_classDefinitionWithUnresolvedTypeName);

    Assert.AreEqual (1, _collection.Count);
  }
}
}
