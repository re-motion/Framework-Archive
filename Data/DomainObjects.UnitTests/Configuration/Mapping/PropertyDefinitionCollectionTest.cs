using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionCollectionTest : ReflectionBasedMappingTest
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

    public override void SetUp ()
    {
      base.SetUp ();

      _propertyDefinition = new ReflectionBasedPropertyDefinition ("Name", "Name", typeof (string), 100);
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

      try
      {
        _collection.Add (_propertyDefinition);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreSame (_propertyDefinition, eventReceiver.AddingPropertyDefinition);
        Assert.AreSame (null, eventReceiver.AddedPropertyDefinition);
      }
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
    public void ContainsPropertyNameTrue ()
    {
      _collection.Add (_propertyDefinition);
      Assert.IsTrue (_collection.Contains ("Name"));
    }

    [Test]
    public void ContainsPropertyNameFalse ()
    {
      Assert.IsFalse (_collection.Contains ("UndefinedPropertyName"));
    }

    [Test]
    public void ContainsPropertyDefinitionTrue ()
    {
      _collection.Add (_propertyDefinition);
      Assert.IsTrue (_collection.Contains (_propertyDefinition));
    }

    [Test]
    public void ContainsPropertyDefinitionFalse ()
    {
      _collection.Add (_propertyDefinition);

      PropertyDefinition copy = new ReflectionBasedPropertyDefinition (
          _propertyDefinition.PropertyName, _propertyDefinition.StorageSpecificName, _propertyDefinition.PropertyType,
          _propertyDefinition.IsNullable, _propertyDefinition.MaxLength);

      Assert.IsFalse (_collection.Contains (copy));
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

    [Test]
    public void ContainsColumName ()
    {
      _collection.Add (new ReflectionBasedPropertyDefinition ("PropertyName", "ColumnName", typeof (int)));

      Assert.IsTrue (_collection.ContainsColumnName ("ColumnName"));
    }

    [Test]
    public void InitializeWithClassDefinition ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];
      PropertyDefinitionCollection collection = new PropertyDefinitionCollection (orderDefinition);
      Assert.AreSame (orderDefinition, collection.ClassDefinition);
    }
  }
}
