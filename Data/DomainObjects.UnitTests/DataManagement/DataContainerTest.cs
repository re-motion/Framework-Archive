using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataContainerTest : ClientTransactionBaseTest
  {
    private PropertyDefinition _nameDefinition;
    private PropertyValue _nameProperty;
    private DataContainer _newDataContainer;
    private DataContainer _existingDataContainer;

    public override void SetUp ()
    {
      base.SetUp ();

      Guid idValue = Guid.NewGuid ();

      _newDataContainer = DataContainer.CreateNew (new ObjectID ("Order", idValue));

      _existingDataContainer = DataContainer.CreateForExisting (new ObjectID ("Order", idValue), null);

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
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);

      Assert.AreSame (_nameProperty, eventReceiver.ChangedPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangedOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangedNewValue);
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
        Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
        Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);
        Assert.AreSame (null, eventReceiver.ChangedPropertyValue);
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
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);

      Assert.AreSame (_nameProperty, eventReceiver.ChangedPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangedOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangedNewValue);
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
        Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
        Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);
        Assert.AreSame (null, eventReceiver.ChangedPropertyValue);
      }
    }

    [Test]
    public void GetObjectID ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      ObjectID id = (ObjectID) dataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
      Assert.IsNotNull (id);
    }

    [Test]
    public void GetNullObjectID ()
    {
      ObjectID id = new ObjectID ("Official", 1);
      DataContainer container = DataContainer.CreateNew (id);

      PropertyDefinition reportsToDefinition = new PropertyDefinition (
          "ReportsTo", "ReportsTo", "string", true, true, new NaInt32 (100));

      container.PropertyValues.Add (new PropertyValue (reportsToDefinition, null));

      Assert.IsNull (container.GetValue ("ReportsTo"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "Property 'NonExistingPropertyName' does not exist.\r\nParameter name: propertyName")]
    public void GetObjectIDForNonExistingProperty ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
      container.GetValue ("NonExistingPropertyName");
    }

    [Test]
    public void ChangePropertyBackToOriginalValue ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();

      container["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 42;
      Assert.AreEqual (StateType.Changed, container.State);
      Assert.AreEqual (42, container.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));

      container["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 1;
      Assert.AreEqual (StateType.Unchanged, container.State);
      Assert.AreEqual (1, container.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));
    }

    [Test]
    public void SetValue ()
    {
      _existingDataContainer.PropertyValues.Add (_nameProperty);
      _existingDataContainer.SetValue ("Name", "Zaphod Beeblebrox");

      Assert.AreEqual ("Zaphod Beeblebrox", _existingDataContainer.GetValue ("Name"));
    }

    [Test]
    public void IsNull ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

      Assert.IsTrue (dataContainer.IsNull ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"));
      Assert.IsTrue (dataContainer.IsNull ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"));
      Assert.IsTrue (dataContainer.IsNull ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));

      Assert.IsFalse (dataContainer.IsNull ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"));
      Assert.IsFalse (dataContainer.IsNull ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"));
      Assert.IsFalse (dataContainer.IsNull ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));
    }

    [Test]
    public void IsNullOrEmpty ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

      Assert.IsTrue (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"));
      Assert.IsTrue (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"));
      Assert.IsFalse (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"));
      Assert.IsFalse (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"));
      Assert.IsFalse (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"));
      Assert.IsFalse (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"));
      Assert.IsFalse (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));

      dataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"] = Guid.Empty;
      dataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"] = string.Empty;
      dataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = new byte[0];

      Assert.IsTrue (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"));
      Assert.IsTrue (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"));
      Assert.IsTrue (dataContainer.IsNullOrEmpty ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));
    }

    [Test]
    public void GetBytes ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

      ResourceManager.IsEqualToImage1 ((byte[]) dataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));
      Assert.IsNull (dataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));
    }

    [Test]
    public void SetBytes ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

      dataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = new byte[0];
      ResourceManager.IsEmptyImage ((byte[]) dataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));

      dataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = null;
      Assert.IsNull (dataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));
    }
  }
}
