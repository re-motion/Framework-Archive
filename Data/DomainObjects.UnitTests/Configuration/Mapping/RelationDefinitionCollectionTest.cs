using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class RelationDefinitionCollectionTest
{
  // types

  // static members and constants

  // member fields

  private RelationDefinitionCollection _collection;
  private RelationDefinition _relationDefinition;

  // construction and disposing

  public RelationDefinitionCollectionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _relationDefinition = TestMappingConfiguration.Current.RelationDefinitions["OrderToOrderTicket"]; 
    _collection = new RelationDefinitionCollection ();
  }

  [Test]
  public void Add ()
  {
    _collection.Add (_relationDefinition);
    Assert.AreEqual (1, _collection.Count);
  }

  [Test]
  public void RelationDefinitionIndexer ()
  {
    _collection.Add (_relationDefinition);
    Assert.AreSame (_relationDefinition, _collection["OrderToOrderTicket"]);  
  }

  [Test]
  public void NumericIndexer ()
  {
    _collection.Add (_relationDefinition);
    Assert.AreSame (_relationDefinition, _collection[0]);  
  }

  [Test]
  public void ContainsTrue ()
  {
    _collection.Add (_relationDefinition);
    Assert.IsTrue (_collection.Contains ("OrderToOrderTicket"));
  }

  [Test]
  public void ContainsFalse ()
  {
    Assert.IsFalse (_collection.Contains ("OrderToOrderTicket"));
  }

  [Test]
  public void CopyConstructor ()
  {
    _collection.Add (_relationDefinition);

    RelationDefinitionCollection copiedCollection = new RelationDefinitionCollection (_collection, false);

    Assert.AreEqual (1, copiedCollection.Count);
    Assert.AreSame (_relationDefinition, copiedCollection[0]);
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Relation 'NonExistingRelationDefinitionID' does not exist.")]
  public void GetMandatoryWithNonExistingRelationDefintionID ()
  {
    _collection.GetMandatory ("NonExistingRelationDefinitionID");
  }

  [Test]
  public void ContainsRelationDefintion ()
  {
    _collection.Add (_relationDefinition);
    
    Assert.IsTrue (_collection.Contains (_relationDefinition));    
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNullRelationDefintion ()
  {
    _collection.Contains ((RelationDefinition) null);
  }
}
}
