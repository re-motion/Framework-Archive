using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class PropertyValueCollectionTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private PropertyValueCollection _collection;

    // construction and disposing

    public PropertyValueCollectionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _collection = new PropertyValueCollection ();
    }

    [Test]
    public void Events ()
    {
      PropertyValue propertyValue1 = CreatePropertyValue ("Property 1", "int32", 42);
      PropertyValue propertyValue2 = CreatePropertyValue ("Property 2", "string", "Arthur Dent");
      PropertyValue propertyValue3 = CreatePropertyValue ("Property 3", "string", true, null);

      _collection.Add (propertyValue1);
      _collection.Add (propertyValue2);
      _collection.Add (propertyValue3);

      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (_collection, false);

      _collection["Property 2"].Value = "Zaphod Beeblebrox";

      Assert.AreSame (propertyValue2, eventReceiver.ChangingPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);

      Assert.AreSame (propertyValue2, eventReceiver.ChangedPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangedOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangedNewValue);
    }

    [Test]
    public void CancelEvents ()
    {
      PropertyValue propertyValue1 = CreatePropertyValue ("Property 1", "int32", 42);
      PropertyValue propertyValue2 = CreatePropertyValue ("Property 2", "string", "Arthur Dent");
      PropertyValue propertyValue3 = CreatePropertyValue ("Property 3", "string", true, null);

      _collection.Add (propertyValue1);
      _collection.Add (propertyValue2);
      _collection.Add (propertyValue3);

      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (_collection, true);

      try
      {
        _collection["Property 2"].Value = "Zaphod Beeblebrox";
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreSame (propertyValue2, eventReceiver.ChangingPropertyValue);
        Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
        Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);
        Assert.AreSame (null, eventReceiver.ChangedPropertyValue);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Property 'DoesNotExist' does not exist.\r\nParameter name: propertyName")]
    public void NonExistingPropertyName ()
    {
      _collection.Add (CreatePropertyValue ("PropertyName 1", "int32", 42));
      _collection.Add (CreatePropertyValue ("PropertyName 2", "int32", 43));

      PropertyValue propertyValue = _collection["DoesNotExist"];
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Property 'PropertyName' already exists in collection.\r\nParameter name: value")]
    public void DuplicatePropertyNames ()
    {
      _collection.Add (CreatePropertyValue ("PropertyName", "int32", 42));
      _collection.Add (CreatePropertyValue ("PropertyName", "int32", 43));
    }

    [Test]
    public void PreviousEventCancels ()
    {
      PropertyValue value = CreatePropertyValue ("PropertyName", "int32", 42);
      PropertyValueEventReceiver valueEventReceiver = new PropertyValueEventReceiver (value, true);

      _collection.Add (value);

      try
      {
        value.Value = 45;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (42, value.Value, "Value");
        Assert.AreEqual (false, valueEventReceiver.HasChangedEventBeenCalled, "HasChangedEventBeenCalled");
      }
    }

    [Test]
    public void PropertyValueInTwoCollections ()
    {
      PropertyValue value = CreatePropertyValue ("PropertyName", "int32", 42);
      PropertyValueCollection collection1 = new PropertyValueCollection ();
      PropertyValueCollection collection2 = new PropertyValueCollection ();

      collection1.Add (value);
      collection2.Add (value);

      PropertyValueContainerEventReceiver receiver1 = new PropertyValueContainerEventReceiver (collection1, false);
      PropertyValueContainerEventReceiver receiver2 = new PropertyValueContainerEventReceiver (collection2, false);

      value.Value = 43;

      Assert.IsNotNull (receiver1.ChangingPropertyValue);
      Assert.IsNotNull (receiver1.ChangedPropertyValue);

      Assert.IsNotNull (receiver2.ChangingPropertyValue);
      Assert.IsNotNull (receiver2.ChangedPropertyValue);
    }

    [Test]
    public void ContainsPropertyValueTrue ()
    {
      PropertyValue value = CreatePropertyValue ("PropertyName", "int32", 42);

      _collection.Add (value);

      Assert.IsTrue (_collection.Contains (value));
    }

    [Test]
    public void ContainsPropertyValueFalse ()
    {
      PropertyValue value = CreatePropertyValue ("PropertyName", "int32", 42);
      _collection.Add (value);

      PropertyValue copy = CreatePropertyValue ("PropertyName", "int32", 42);

      Assert.IsFalse (_collection.Contains (copy));

    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullPropertyValue ()
    {
      _collection.Contains ((PropertyValue) null);
    }

    private PropertyValue CreatePropertyValue (string name, string mappingType, object value)
    {
      return CreatePropertyValue (name, mappingType, false, value);
    }

    private PropertyValue CreatePropertyValue (string name, string mappingType, bool isNullable, object value)
    {
      NaInt32 maxLength = NaInt32.Null;
      if (mappingType == "string")
        maxLength = new NaInt32 (100);

      PropertyDefinition definition = new PropertyDefinition (name, name, mappingType, true, isNullable, maxLength);
      return new PropertyValue (definition, value);
    }
  }
}
