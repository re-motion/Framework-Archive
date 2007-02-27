using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ClassDefinitionCollectionWithUnresolvedTypesTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinitionCollection _collection;
    private ClassDefinition _classDefinitionWithUnresolvedType;

    // construction and disposing

    public ClassDefinitionCollectionWithUnresolvedTypesTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _collection = new ClassDefinitionCollection (false);
      _classDefinitionWithUnresolvedType = new ClassDefinition ("Order", "OrderTable", "StorageProvider", "UnresolvedType", false);
    }


    [Test]
    public void InitializeWithUnresolvedTypes ()
    {
      Assert.IsFalse (_collection.AreResolvedTypesRequired);
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
    public void AddWithResolvedType ()
    {
      Assert.AreEqual (0, _collection.Count);

      ClassDefinition classDefinition = new ClassDefinition ("Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order));
      _collection.Add (classDefinition);

      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void AddWithUnresolvedType ()
    {
      Assert.AreEqual (0, _collection.Count);

      _collection.Add (_classDefinitionWithUnresolvedType);

      Assert.AreEqual (1, _collection.Count);
    }
  }
}
