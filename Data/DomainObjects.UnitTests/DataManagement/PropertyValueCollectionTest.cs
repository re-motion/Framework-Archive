using System;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class PropertyValueCollectionTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public PropertyValueCollectionTest ()
  {
  }

  // methods and properties

  [Test]
  public void Events ()
  {
    PropertyValue propertyValue1 = CreatePropertyValue ("Property 1", "int32", 42);
    PropertyValue propertyValue2 = CreatePropertyValue ("Property 2", "string", "Arthur Dent");
    PropertyValue propertyValue3 = CreatePropertyValue ("Property 3", "string", true, null);
    PropertyValueCollection propertyValueCollection = new PropertyValueCollection ();

    propertyValueCollection.Add (propertyValue1);
    propertyValueCollection.Add (propertyValue2);
    propertyValueCollection.Add (propertyValue3);

    PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver
        (propertyValueCollection, false);

    propertyValueCollection["Property 2"].Value = "Zaphod Beeblebrox";

    Assert.AreSame (propertyValue2, eventReceiver.ChangingPropertyValue);
    Assert.AreSame (propertyValue2, eventReceiver.ChangedPropertyValue);
    Assert.AreEqual ("Arthur Dent", eventReceiver.OldValue);
    Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.NewValue);
  }

  [Test]
  public void CancelEvents ()
  {
    PropertyValue propertyValue1 = CreatePropertyValue ("Property 1", "int32", 42);
    PropertyValue propertyValue2 = CreatePropertyValue ("Property 2", "string", "Arthur Dent");
    PropertyValue propertyValue3 = CreatePropertyValue ("Property 3", "string", true, null);
    PropertyValueCollection propertyValueCollection = new PropertyValueCollection ();

    propertyValueCollection.Add (propertyValue1);
    propertyValueCollection.Add (propertyValue2);
    propertyValueCollection.Add (propertyValue3);

    PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver
      (propertyValueCollection, true);

    propertyValueCollection["Property 2"].Value = "Zaphod Beeblebrox";

    Assert.AreSame (propertyValue2, eventReceiver.ChangingPropertyValue);
    Assert.AreSame (null, eventReceiver.ChangedPropertyValue);
    Assert.AreEqual ("Arthur Dent", eventReceiver.OldValue);
    Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.NewValue);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), "Property 'DoesNotExist' does not exist.\r\nParameter name: propertyName")]
  public void NonExistingPropertyName ()
  {
    PropertyValueCollection propertyValueCollection = new PropertyValueCollection ();
    propertyValueCollection.Add (CreatePropertyValue ("PropertyName 1", "int32", 42));
    propertyValueCollection.Add (CreatePropertyValue ("PropertyName 2", "int32", 43));

    PropertyValue propertyValue = propertyValueCollection["DoesNotExist"];
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), "Property 'PropertyName' already exists in collection.\r\nParameter name: value")]
  public void DuplicatePropertyNames ()
  {
    PropertyValueCollection propertyValueCollection = new PropertyValueCollection ();
    propertyValueCollection.Add (CreatePropertyValue ("PropertyName", "int32", 42));
    propertyValueCollection.Add (CreatePropertyValue ("PropertyName", "int32", 43));
  }

  [Test]
  public void DoNotOverrideCancelFlagOfPreviousEvents ()
  {
    PropertyValue value = CreatePropertyValue ("PropertyName", "int32", 42);
    PropertyValueEventReceiver valueEventReceiver = new PropertyValueEventReceiver (value, true);

    PropertyValueCollection collection = new PropertyValueCollection ();
    collection.Add (value);

    PropertyValueContainerEventReceiver collectionEventReceiver =
        new PropertyValueContainerEventReceiver (collection, false);

    value.Value = 45;

    Assert.AreEqual (42, value.Value, "Value");
    Assert.AreEqual (false, valueEventReceiver.HasChangedEventBeenCalled, "HasChangedEventBeenCalled");
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

    PropertyDefinition definition = new PropertyDefinition (name, name, mappingType, isNullable, maxLength);
    return new PropertyValue (definition, value);
  }

  [Test]
  public void ContainsPropertyValue ()
  {
    PropertyValueCollection collection = new PropertyValueCollection ();
    PropertyValue value = CreatePropertyValue ("PropertyName", "int32", 42);

    collection.Add (value);
    
    Assert.IsTrue (collection.Contains (value));    
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNullPropertyValue ()
  {
    PropertyValueCollection collection = new PropertyValueCollection ();
    collection.Contains ((PropertyValue) null);
  }
}
}
