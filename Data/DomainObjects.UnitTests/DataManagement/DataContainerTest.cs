using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

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
      ReflectionBasedClassDefinition orderClass =
          new ReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false);

      _newDataContainer = DataContainer.CreateNew (new ObjectID ("Order", idValue));
      _existingDataContainer = DataContainer.CreateForExisting (new ObjectID ("Order", idValue), null);

      ClientTransactionMock.SetClientTransaction (_existingDataContainer);
      ClientTransactionMock.SetClientTransaction (_newDataContainer);

      _nameDefinition = new ReflectionBasedPropertyDefinition (orderClass, "Name", "Name", typeof (string), 100);
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

      PropertyDefinition reportsToDefinition = new ReflectionBasedPropertyDefinition (
          (ReflectionBasedClassDefinition) container.ClassDefinition, "ReportsTo", "ReportsTo", typeof (string), true, 100);

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
      Assert.AreEqual (StateType.Changed, container.State);
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

    private void CheckIfDataContainersAreEqual (DataContainer expected, DataContainer actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);
      ArgumentUtility.CheckNotNull ("actual", actual);

      Assert.AreNotSame (expected, actual);
      
      Assert.AreEqual (expected.ID, actual.ID);
      Assert.AreSame (expected.ClassDefinition, actual.ClassDefinition);
      Assert.AreSame (expected.ClientTransaction, actual.ClientTransaction);
      Assert.AreSame (expected.DomainObject, actual.DomainObject);
      Assert.AreSame (expected.DomainObjectType, actual.DomainObjectType);
      Assert.AreEqual (expected.IsDiscarded, actual.IsDiscarded);
      Assert.AreEqual (expected.PropertyValues.Count, actual.PropertyValues.Count);

      for (int i = 0; i < actual.PropertyValues.Count; ++i)
      {
        Assert.AreSame (expected.PropertyValues[i].Definition, actual.PropertyValues[i].Definition);
        Assert.AreEqual (expected.PropertyValues[i].HasChanged, actual.PropertyValues[i].HasChanged);
        Assert.AreEqual (expected.PropertyValues[i].IsDiscarded, actual.PropertyValues[i].IsDiscarded);
        Assert.AreEqual (expected.PropertyValues[i].OriginalValue, actual.PropertyValues[i].OriginalValue);
        Assert.AreEqual (expected.PropertyValues[i].Value, actual.PropertyValues[i].Value);
      }
      
      Assert.AreEqual (expected.State, actual.State);
      Assert.AreSame (expected.Timestamp, actual.Timestamp);
    }

    [Test]
    public void CloneLoadedUnchanged ()
    {
      DataContainer original = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      Assert.IsNotNull (original);
      Assert.AreEqual (DomainObjectIDs.Order1, original.ID);
      Assert.IsNotNull (original.ClassDefinition);
      Assert.AreSame (ClientTransactionMock, original.ClientTransaction);
      Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), original.DomainObject);
      Assert.AreSame (typeof (Order), original.DomainObjectType);
      Assert.IsFalse (original.IsDiscarded);
      Assert.AreEqual (4, original.PropertyValues.Count);
      Assert.IsNotNull (original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Definition);
      Assert.IsFalse (original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].HasChanged);
      Assert.IsFalse (original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].IsDiscarded);
      Assert.AreEqual (1, original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue);
      Assert.AreEqual (1, original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Value);
      Assert.AreEqual (StateType.Unchanged, original.State);
      Assert.IsNotNull (original.Timestamp);

      DataContainer clone = original.Clone ();

      Assert.IsNotNull (clone);
      CheckIfDataContainersAreEqual (original, clone);
    }

    [Test]
    public void CloneLoadedChanged ()
    {
      DataContainer original = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Value = 75;

      Assert.IsNotNull (original);
      Assert.AreEqual (DomainObjectIDs.Order1, original.ID);
      Assert.IsNotNull (original.ClassDefinition);
      Assert.AreSame (ClientTransactionMock, original.ClientTransaction);
      Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), original.DomainObject);
      Assert.AreSame (typeof (Order), original.DomainObjectType);
      Assert.IsFalse (original.IsDiscarded);
      Assert.AreEqual (4, original.PropertyValues.Count);
      Assert.IsNotNull (original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Definition);
      Assert.IsTrue (original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].HasChanged);
      Assert.IsFalse (original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].IsDiscarded);
      Assert.AreEqual (1, original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue);
      Assert.AreEqual (75, original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Value);
      Assert.AreEqual (StateType.Changed, original.State);
      Assert.IsNotNull (original.Timestamp);

      DataContainer clone = original.Clone ();

      Assert.IsNotNull (clone);
      CheckIfDataContainersAreEqual (original, clone);
    }

    [Test]
    public void CloneNew ()
    {
      Order order = Order.NewObject ();
      DataContainer original = order.InternalDataContainer;

      Assert.IsNotNull (original);
      Assert.AreEqual (order.ID, original.ID);
      Assert.IsNotNull (original.ClassDefinition);
      Assert.AreSame (ClientTransactionMock, original.ClientTransaction);
      Assert.AreSame (order, original.DomainObject);
      Assert.AreSame (typeof (Order), original.DomainObjectType);
      Assert.IsFalse (original.IsDiscarded);
      Assert.AreEqual (4, original.PropertyValues.Count);
      Assert.IsNotNull (original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Definition);
      Assert.IsFalse (original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].HasChanged);
      Assert.IsFalse (original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].IsDiscarded);
      Assert.AreEqual (0, original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue);
      Assert.AreEqual (0, original.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Value);
      Assert.AreEqual (StateType.New, original.State);
      Assert.IsNull (original.Timestamp);

      DataContainer clone = original.Clone ();

      Assert.IsNotNull (clone);
      CheckIfDataContainersAreEqual (original, clone);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException), ExpectedMessage = "Object 'Order.*' is already discarded.", MatchType = MessageMatch.Regex)]
    public void CloneDeleted ()
    {
      Order order = Order.NewObject ();
      DataContainer original = order.InternalDataContainer;
      order.Delete ();

      Assert.IsTrue (original.IsDiscarded);

      original.Clone ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A DataContainer cannot be discarded while it doesn't have an "
        + "associated DomainObject.")]
    public void DiscardWithoutDomainObjectThrows ()
    {
      DataContainer dataContainerWithoutDomainObject = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionMock.SetClientTransaction (dataContainerWithoutDomainObject);
      PrivateInvoke.InvokeNonPublicMethod (dataContainerWithoutDomainObject, "Delete");
      Assert.Fail ("Expected exception");
    }

    [Test]
    public void GetIDEvenPossibleWhenDiscarded ()
    {
      Order order = Order.NewObject ();
      DataContainer dataContainer = order.InternalDataContainer;
      order.Delete ();
      Assert.IsTrue (dataContainer.IsDiscarded);
      Assert.AreEqual (order.ID, dataContainer.ID);
    }

    [Test]
    public void GetDomainObjectEvenPossibleWhenDiscarded ()
    {
      Order order = Order.NewObject ();
      DataContainer dataContainer = order.InternalDataContainer;
      order.Delete ();
      Assert.IsTrue (dataContainer.IsDiscarded);
      Assert.AreSame (order, dataContainer.DomainObject);
    }
  }
}
