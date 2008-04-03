using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionCollectionTest : StandardMappingTest
  {
    private PropertyDefinitionCollection _collection;
    private XmlBasedPropertyDefinition _propertyDefinition;
    private XmlBasedClassDefinition _classDefinition;


    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinition = new XmlBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order));
      _propertyDefinition = new XmlBasedPropertyDefinition (_classDefinition, "Name", "Name", "string", 100);
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

      XmlBasedPropertyDefinition copy = new XmlBasedPropertyDefinition (
          (XmlBasedClassDefinition) _propertyDefinition.ClassDefinition,  
          _propertyDefinition.PropertyName, _propertyDefinition.StorageSpecificName, _propertyDefinition.MappingTypeName, true,
          _propertyDefinition.IsNullable, _propertyDefinition.MaxLength, _propertyDefinition.IsPersistent);

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
      _collection.Add (new XmlBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", "int32"));

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
