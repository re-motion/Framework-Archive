using System;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class DataContainerTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private PropertyDefinition _nameDefinition;
  private PropertyValue _nameProperty;
  private DataContainer _newDataContainer;
  private DataContainer _existingDataContainer;

  // construction and disposing

  public DataContainerTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _newDataContainer = DataContainer.CreateNew (new ObjectID ("Order", 5));

    _existingDataContainer = DataContainer.CreateForExisting (new ObjectID ("Order", 5), null);

    _nameDefinition = new PropertyDefinition ("Name", "Name", "string", new NaInt32 (100));
    _nameProperty = new PropertyValue (_nameDefinition, "Arthur Dent");
  }

  [Test]
  public void NewDataContainerStates ()
  {
    _newDataContainer.PropertyValues.Add (_nameProperty);

    Assert.AreEqual (StateType.New, _newDataContainer.State);
    Assert.AreEqual ("Arthur Dent", _newDataContainer["Name"]);

    _newDataContainer["Name"] = "Zaphod Beeblebrox";

    Assert.AreEqual (StateType.New, _newDataContainer.State);
    Assert.AreEqual ("Zaphod Beeblebrox", _newDataContainer["Name"]);
  }

  [Test]
  public void ExistingDataContainerStates ()
  {
    _existingDataContainer.PropertyValues.Add (_nameProperty);

    Assert.AreEqual (StateType.Unchanged, _existingDataContainer.State);
    Assert.AreEqual ("Arthur Dent", _existingDataContainer["Name"]);

    _existingDataContainer["Name"] = "Zaphod Beeblebrox";

    Assert.AreEqual (StateType.Changed, _existingDataContainer.State);
    Assert.AreEqual ("Zaphod Beeblebrox", _existingDataContainer["Name"]);
  }

  [Test]
  public void NewDataContainerEvents ()
  {
    _newDataContainer.PropertyValues.Add (_nameProperty);

    PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (
        _newDataContainer, false);

    _newDataContainer["Name"] = "Zaphod Beeblebrox";

    Assert.AreEqual (StateType.New, _newDataContainer.State);
    Assert.AreEqual ("Zaphod Beeblebrox", _newDataContainer["Name"]);
    Assert.AreSame (_nameProperty, eventReceiver.ChangingPropertyValue);
    Assert.AreSame (_nameProperty, eventReceiver.ChangedPropertyValue);
    Assert.AreEqual ("Arthur Dent", eventReceiver.OldValue);
    Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.NewValue);
  }

  [Test]
  public void NewDataContainerCancelEvents ()
  {
    _newDataContainer.PropertyValues.Add (_nameProperty);

    PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (
        _newDataContainer, true);

    try
    {
      _newDataContainer["Name"] = "Zaphod Beeblebrox";
      Assert.Fail ("EventReceiverCancelException should be raised.");
    }
    catch (EventReceiverCancelException)
    {
      Assert.AreEqual (StateType.New, _newDataContainer.State);
      Assert.AreEqual ("Arthur Dent", _newDataContainer["Name"]);
      Assert.AreSame (_nameProperty, eventReceiver.ChangingPropertyValue);
      Assert.AreSame (null, eventReceiver.ChangedPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.OldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.NewValue);
    }
  }

  [Test]
  public void ExistingDataContainerEvents ()
  {
    _existingDataContainer.PropertyValues.Add (_nameProperty);

    PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (
        _existingDataContainer, false);

    _existingDataContainer["Name"] = "Zaphod Beeblebrox";

    Assert.AreEqual (StateType.Changed, _existingDataContainer.State);
    Assert.AreEqual ("Zaphod Beeblebrox", _existingDataContainer["Name"]);
    Assert.AreSame (_nameProperty, eventReceiver.ChangingPropertyValue);
    Assert.AreSame (_nameProperty, eventReceiver.ChangedPropertyValue);
    Assert.AreEqual ("Arthur Dent", eventReceiver.OldValue);
    Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.NewValue);
  }

  [Test]
  public void ExistingDataContainerCancelEvents ()
  {
    _existingDataContainer.PropertyValues.Add (_nameProperty);

    PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (
        _existingDataContainer, true);

    try
    {
      _existingDataContainer["Name"] = "Zaphod Beeblebrox";
      Assert.Fail ("EventReceiverCancelException should be raised.");
    }
    catch (EventReceiverCancelException)
    {
      Assert.AreEqual (StateType.Unchanged, _existingDataContainer.State);
      Assert.AreEqual ("Arthur Dent", _existingDataContainer["Name"]);
      Assert.AreSame (_nameProperty, eventReceiver.ChangingPropertyValue);
      Assert.AreSame (null, eventReceiver.ChangedPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.OldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.NewValue);
    }
  }

  [Test]
  public void GetObjectID ()
  {
    DataContainer dataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
    ObjectID id = dataContainer.GetObjectID ("Customer");
    Assert.IsNotNull (id);
  }

  [Test]
  public void GetNullObjectID ()
  {
    ObjectID id = new ObjectID ("Official", 1);
    DataContainer container = DataContainer.CreateNew (id);

    PropertyDefinition reportsToDefinition = new PropertyDefinition (
        "ReportsTo", "ReportsTo", "string", true, new NaInt32 (100));

    container.PropertyValues.Add (new PropertyValue (reportsToDefinition, null));

    Assert.IsNull (container.GetObjectID ("ReportsTo"));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "Property 'NonExistingPropertyName' does not exist.\r\nParameter name: propertyName")]
  public void GetObjectIDForNonExistingProperty ()
  {
    DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
    container.GetObjectID ("NonExistingPropertyName");
  }

  [Test]
  [ExpectedException (typeof (InvalidCastException), 
      "Property 'OrderNumber' is of type 'System.Int32', but must be 'Rubicon.Data.DomainObjects.ObjectID'.")]
  public void GetObjectIDForPropertyWithWrongType ()
  {
    DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
    container.GetObjectID ("OrderNumber");
  }

  [Test]
  public void ChangePropertyBackToOriginalValue ()
  {
    DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
    
    container["OrderNumber"] = 42;
    Assert.AreEqual (StateType.Changed, container.State);
    Assert.AreEqual (42, container.GetInt32 ("OrderNumber"));

    container["OrderNumber"] = 1;
    Assert.AreEqual (StateType.Unchanged, container.State);
    Assert.AreEqual (1, container.GetInt32 ("OrderNumber"));
  }

  [Test]
  public void SetValue ()
  {
    _existingDataContainer.PropertyValues.Add (_nameProperty);
    _existingDataContainer.SetValue ("Name", "Zaphod Beeblebrox");

    Assert.AreEqual ("Zaphod Beeblebrox", _existingDataContainer.GetString ("Name"));
  }

  [Test]
  public void IsNull ()
  {
    DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

    Assert.IsTrue (dataContainer.IsNull ("StringWithNullValueProperty"));
    Assert.IsTrue (dataContainer.IsNull ("NaInt32WithNullValueProperty"));
    Assert.IsFalse (dataContainer.IsNull ("NaDateProperty"));
    Assert.IsFalse (dataContainer.IsNull ("Int32Property"));
  }

  [Test]
  public void IsNullOrEmpty ()
  {
    DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

    Assert.IsTrue (dataContainer.IsNullOrEmpty ("StringWithNullValueProperty"));
    Assert.IsTrue (dataContainer.IsNullOrEmpty ("NaInt32WithNullValueProperty"));
    Assert.IsFalse (dataContainer.IsNullOrEmpty ("NaDateProperty"));
    Assert.IsFalse (dataContainer.IsNullOrEmpty ("Int32Property"));
    Assert.IsFalse (dataContainer.IsNullOrEmpty ("GuidProperty"));
    Assert.IsFalse (dataContainer.IsNullOrEmpty ("StringProperty"));

    dataContainer["GuidProperty"] = Guid.Empty;
    dataContainer["StringProperty"] = string.Empty;

    Assert.IsTrue (dataContainer.IsNullOrEmpty ("GuidProperty"));
    Assert.IsTrue (dataContainer.IsNullOrEmpty ("StringProperty"));    
  }
}
}
