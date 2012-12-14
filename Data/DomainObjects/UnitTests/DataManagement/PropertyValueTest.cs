using System;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class PropertyValueTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public PropertyValueTest ()
  {
  }

  // methods and properties

  [Test]
  public void TestEquals ()
  {
    PropertyDefinition intDefinition = CreateIntPropertyDefinition ("test");
    PropertyValue propertyValue1 = new PropertyValue (intDefinition, 5);
    PropertyValue propertyValue2 = new PropertyValue (intDefinition, 5);
    Assert.IsTrue (propertyValue1.Equals (propertyValue2), "Initial values");

    propertyValue1.Value = 10;
    Assert.IsFalse (propertyValue1.Equals (propertyValue2), "After changing first value.");

    propertyValue1.Value = 5;
    Assert.IsTrue (propertyValue1.Equals (propertyValue2), "After changing first value back to initial value.");

    propertyValue1.Value = 10;
    propertyValue2.Value = 10;
    Assert.IsTrue (propertyValue1.Equals (propertyValue2), "After changing both values.");

    PropertyValue propertyValue3 = CreateIntPropertyValue ("test", 10);
    Assert.IsFalse (propertyValue1.Equals (propertyValue3), "Different original values.");
  }

  [Test]
  public void HashCode ()
  {
    PropertyValue propertyValue1 = CreateIntPropertyValue ("test", 5);
    PropertyValue propertyValue2 = CreateIntPropertyValue ("test", 5);
    Assert.IsTrue (propertyValue1.GetHashCode () == propertyValue2.GetHashCode (), "Initial values");

    propertyValue1.Value = 10;
    Assert.IsFalse (propertyValue1.GetHashCode () == propertyValue2.GetHashCode (), "After changing first value.");

    propertyValue1.Value = 5;
    Assert.IsTrue (propertyValue1.GetHashCode () == propertyValue2.GetHashCode (), "After changing first value back to initial value.");

    propertyValue1.Value = 10;
    propertyValue2.Value = 10;
    Assert.IsTrue (propertyValue1.GetHashCode () == propertyValue2.GetHashCode (), "After changing both values.");

    PropertyValue propertyValue3 = CreateIntPropertyValue ("test", 10);
    Assert.IsFalse (propertyValue1.GetHashCode () == propertyValue3.GetHashCode (), "Different original values.");
  }

  [Test]
  public void SettingOfValue ()
  {
    PropertyValue propertyValue = CreateIntPropertyValue ("test", 5);

    Assert.AreEqual ("test", propertyValue.Name, "Name after initialization");
    Assert.AreEqual (5, propertyValue.Value, "Value after initialization");
    Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after initialization");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged after initialization");

    propertyValue.Value = 5;

    Assert.AreEqual ("test", propertyValue.Name, "Name after change #1");
    Assert.AreEqual (5, propertyValue.Value, "Value after change #1");
    Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #1");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged after change #1");

    propertyValue.Value = 10;

    Assert.AreEqual ("test", propertyValue.Name, "Name after change #2");
    Assert.AreEqual (10, propertyValue.Value, "Value after change #2");
    Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #2");
    Assert.AreEqual (true, propertyValue.HasChanged, "HasChanged after change #2");

    propertyValue.Value = 20;

    Assert.AreEqual ("test", propertyValue.Name, "Name after change #3");
    Assert.AreEqual (20, propertyValue.Value, "Value after change #3");
    Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #3");
    Assert.AreEqual (true, propertyValue.HasChanged, "HasChanged after change #3");

    propertyValue.Value = 5;

    Assert.AreEqual ("test", propertyValue.Name, "Name after change #4");
    Assert.AreEqual (5, propertyValue.Value, "Value after change #4");
    Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #4");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged after change #4");
  }

  [Test]
  public void SettingOfNullValue ()
  {
    PropertyValue propertyValue = CreateStringPropertyValue ("test", null);

    Assert.AreEqual ("test", propertyValue.Name, "Name after initialization");
    Assert.AreEqual (null, propertyValue.Value, "Value after initialization");
    Assert.AreEqual (null, propertyValue.OriginalValue, "OriginalValue after initialization");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged after initialization");

    propertyValue.Value = null;

    Assert.AreEqual ("test", propertyValue.Name, "Name after change #1");
    Assert.AreEqual (null, propertyValue.Value, "Value after change #1");
    Assert.AreEqual (null, propertyValue.OriginalValue, "OriginalValue after change #1");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged after change #1");

    propertyValue.Value = "Test Value";

    Assert.AreEqual ("test", propertyValue.Name, "Name after change #2");
    Assert.AreEqual ("Test Value", propertyValue.Value, "Value after change #2");
    Assert.AreEqual (null, propertyValue.OriginalValue, "OriginalValue after change #2");
    Assert.AreEqual (true, propertyValue.HasChanged, "HasChanged after change #2");

    propertyValue.Value = null;

    Assert.AreEqual ("test", propertyValue.Name, "Name after change #3");
    Assert.AreEqual (null, propertyValue.Value, "Value after change #3");
    Assert.AreEqual (null, propertyValue.OriginalValue, "OriginalValue after change #3");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged after change #3");
  }

  [Test]
  public void Events ()
  {
    PropertyValue propertyValue = CreateIntPropertyValue ("test", 5);
    PropertyValueEventReceiver eventReceiver = new PropertyValueEventReceiver (propertyValue, false);

    propertyValue.Value = 5;

    Assert.AreEqual (5, propertyValue.Value, "Value");
    Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged");
    Assert.AreEqual (false, eventReceiver.HasChangingEventBeenCalled, "Changing event has been called.");
    Assert.AreEqual (false, eventReceiver.HasChangedEventBeenCalled, "Changed event has been called.");

    propertyValue.Value = 10;

    Assert.AreEqual (10, propertyValue.Value, "Value");
    Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue");
    Assert.AreEqual (true, propertyValue.HasChanged, "HasChanged");
    Assert.AreEqual (true, eventReceiver.HasChangingEventBeenCalled, "Changing event has not been called.");
    Assert.AreEqual (true, eventReceiver.HasChangedEventBeenCalled, "Changed event has not been called.");
    Assert.AreEqual (5, eventReceiver.OldValue);
    Assert.AreEqual (10, eventReceiver.NewValue);
  }

  [Test]
  public void EventsWithNullValue ()
  {
    PropertyValue propertyValue = CreateStringPropertyValue ("test", null);
    PropertyValueEventReceiver eventReceiver = new PropertyValueEventReceiver (propertyValue, false);

    propertyValue.Value = null;

    Assert.AreEqual (null, propertyValue.Value, "Value");
    Assert.AreEqual (null, propertyValue.OriginalValue, "OriginalValue");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged");
    Assert.AreEqual (false, eventReceiver.HasChangingEventBeenCalled, "Changing event has not been called.");
    Assert.AreEqual (false, eventReceiver.HasChangedEventBeenCalled, "Changed event has not been called.");

    propertyValue.Value = "Test string";

    Assert.AreEqual ("Test string", propertyValue.Value, "Value");
    Assert.AreEqual (null, propertyValue.OriginalValue, "OriginalValue");
    Assert.AreEqual (true, propertyValue.HasChanged, "HasChanged");
    Assert.AreEqual (true, eventReceiver.HasChangingEventBeenCalled, "Changing event has not been called.");
    Assert.AreEqual (true, eventReceiver.HasChangedEventBeenCalled, "Changed event has not been called.");
    Assert.AreEqual (null, eventReceiver.OldValue);
    Assert.AreEqual ("Test string", eventReceiver.NewValue);
  }

  [Test]
  public void CancelEvents ()
  {
    PropertyValue propertyValue = CreateIntPropertyValue ("test", 5);
    PropertyValueEventReceiver eventReceiver = new PropertyValueEventReceiver (propertyValue, true);

    propertyValue.Value = 5;

    Assert.AreEqual (5, propertyValue.Value, "Value");
    Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged");
    Assert.AreEqual (false, eventReceiver.HasChangingEventBeenCalled, "Changing event must not be called.");
    Assert.AreEqual (false, eventReceiver.HasChangedEventBeenCalled, "Changed event must not be called.");

    try
    {
      propertyValue.Value = 10;
      Assert.Fail ("EventReceiverCancelException should be raised.");
    }
    catch (EventReceiverCancelException)
    {
      Assert.AreEqual (5, propertyValue.Value, "Value");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue");
      Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged");
      Assert.AreEqual (true, eventReceiver.HasChangingEventBeenCalled, "Changing event has not been called.");
      Assert.AreEqual (false, eventReceiver.HasChangedEventBeenCalled, "Changed event must not be called.");
      Assert.AreEqual (5, eventReceiver.OldValue);
      Assert.AreEqual (10, eventReceiver.NewValue);
    }
  }

  [Test]
  public void CancelEventsWithNullValue ()
  {
    PropertyValue propertyValue = CreateStringPropertyValue ("test", null);
    PropertyValueEventReceiver eventReceiver = new PropertyValueEventReceiver (propertyValue, true);

    propertyValue.Value = null;

    Assert.AreEqual (null, propertyValue.Value, "Value");
    Assert.AreEqual (null, propertyValue.OriginalValue, "OriginalValue");
    Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged");
    Assert.AreEqual (false, eventReceiver.HasChangingEventBeenCalled, "Changing event must not be called.");
    Assert.AreEqual (false, eventReceiver.HasChangedEventBeenCalled, "Changed event must not be called.");

    try
    {
      propertyValue.Value = "Test string";
    }
    catch (EventReceiverCancelException)
    {
      Assert.AreEqual (null, propertyValue.Value, "Value");
      Assert.AreEqual (null, propertyValue.OriginalValue, "OriginalValue");
      Assert.AreEqual (false, propertyValue.HasChanged, "HasChanged");
      Assert.AreEqual (true, eventReceiver.HasChangingEventBeenCalled, "Changing event has not been called.");
      Assert.AreEqual (false, eventReceiver.HasChangedEventBeenCalled, "Changed event must not be called.");
      Assert.AreEqual (null, eventReceiver.OldValue);
      Assert.AreEqual ("Test string", eventReceiver.NewValue);
    }
  }

  [Test]
  [ExpectedException (typeof (ValueTooLongException))]
  public void MaxLengthCheck ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "string", 10);
    PropertyValue propertyValue = new PropertyValue (definition, "12345");
    propertyValue.Value = "12345678901";
  }

  [Test]
  [ExpectedException (typeof (ValueTooLongException))]
  public void MaxLengthCheckInConstructor ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "string", 10);
    PropertyValue propertyValue = new PropertyValue (definition, "12345678901");
  }
  
  [Test]
  [ExpectedException (typeof (InvalidTypeException))]
  public void TypeCheckInConstructor ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "string", new NaInt32 (10));
    PropertyValue propertyValue = new PropertyValue (definition, 123);    
  }

  [Test]
  [ExpectedException (typeof (InvalidTypeException))]
  public void TypeCheck ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "string", new NaInt32 (10));
    PropertyValue propertyValue = new PropertyValue (definition, "123");
    propertyValue.Value = 123;
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), "Property 'test' does not allow null values.")]
  public void SetNotNullableStringToNull ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "string", false, new NaInt32 (10));
    PropertyValue propertyValue = new PropertyValue (definition, string.Empty);

    propertyValue.Value = null;
  }

  private PropertyValue CreateIntPropertyValue (string name, int intValue)
  {
    return CreatePropertyValue (name, "int32", false, intValue);
  }

  private PropertyValue CreateStringPropertyValue (string name, string stringValue)
  {
    bool isNullable = (stringValue == null) ? true : false;
    return CreatePropertyValue (name, "string", isNullable, stringValue);
  }

  private PropertyDefinition CreateIntPropertyDefinition (string name)
  {
    return CreatePropertyDefinition (name, "int32", false);
  }

  private PropertyDefinition CreatePropertyDefinition (string name, string mappingType, bool isNullable)
  {
    NaInt32 maxLength = NaInt32.Null;
    if (mappingType == "string")
      maxLength = new NaInt32 (100);

    return new PropertyDefinition (name, name, mappingType, isNullable, maxLength);
  }

  private PropertyValue CreatePropertyValue (string name, string mappingType, bool isNullable, object value)
  {
    return new PropertyValue (CreatePropertyDefinition (name, mappingType, isNullable), value);
  }
}
}
