using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class PropertyDefinitionCollectionTest
{
  // types

  // static members and constants

  // member fields

  private PropertyDefinitionCollection _collection;
  private PropertyDefinition _propertyDefinition;

  // construction and disposing

  public PropertyDefinitionCollectionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _propertyDefinition = new PropertyDefinition ("Name", "Name", "string", 100);
    _collection = new PropertyDefinitionCollection ();
  }

  [Test]
  public void Add ()
  {
    _collection.Add (_propertyDefinition);
    Assert.AreEqual (1, _collection.Count);
  }

  [Test]
  public void AddEvents ()
  {
    PropertyDefinitionCollectionEventReceiver eventReceiver = new PropertyDefinitionCollectionEventReceiver (
        _collection, false);

    _collection.Add (_propertyDefinition);    

    Assert.AreSame (_propertyDefinition, eventReceiver.AddingPropertyDefinition);
    Assert.AreSame (_propertyDefinition, eventReceiver.AddedPropertyDefinition);
  }

  [Test]
  public void CancelAdd ()
  {
    PropertyDefinitionCollectionEventReceiver eventReceiver = new PropertyDefinitionCollectionEventReceiver (
        _collection, true);

    _collection.Add (_propertyDefinition);    

    Assert.AreSame (_propertyDefinition, eventReceiver.AddingPropertyDefinition);
    Assert.AreSame (null, eventReceiver.AddedPropertyDefinition);
  }

  [Test]
  public void PropertyNameIndexer ()
  {
    _collection.Add (_propertyDefinition);
    Assert.AreSame (_propertyDefinition, _collection["Name"]);  
  }

  [Test]
  public void NumericIndexer ()
  {
    _collection.Add (_propertyDefinition);
    Assert.AreSame (_propertyDefinition, _collection[0]);  
  }

  [Test]
  public void ContainsTrue ()
  {
    _collection.Add (_propertyDefinition);
    Assert.IsTrue (_collection.Contains ("Name"));
  }

  [Test]
  public void ContainsFalse ()
  {
    Assert.IsFalse (_collection.Contains ("UndefinedPropertyName"));
  }

  [Test]
  public void CopyConstructor ()
  {
    _collection.Add (_propertyDefinition);

    PropertyDefinitionCollection copiedCollection = new PropertyDefinitionCollection (_collection, false);

    Assert.AreEqual (1, copiedCollection.Count);
    Assert.AreSame (_propertyDefinition, copiedCollection[0]);
  }

  [Test]
  public void ContainsPropertyDefinition ()
  {
    _collection.Add (_propertyDefinition);
    
    Assert.IsTrue (_collection.Contains (_propertyDefinition));    
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNullPropertyDefinition ()
  {
    _collection.Contains ((PropertyDefinition) null);
  }
}
}
