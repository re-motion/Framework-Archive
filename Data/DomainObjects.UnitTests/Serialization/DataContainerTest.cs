using System;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DataContainerTest : ClientTransactionBaseTest
  {
    [Test]
    public void DataContainerIsSerializable ()
    {
      ObjectID objectID = new ObjectID ("Customer", Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);

      DataContainer deserializedDataContainer = Serializer.SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
    }

    [Test]
    public void DataContainer_ContentsTest ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);

      Computer computer = employee.Computer;
      computer.SerialNumber = "abc";

      DataContainer dataContainer = computer.InternalDataContainer;
      Tuple<ClientTransaction, DataContainer> deserializedObjects =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, dataContainer));

      ClientTransaction deserializedTransaction = deserializedObjects.A;
      DataContainer deserializedDataContainer = deserializedObjects.B;
      DomainObject deserializedDomainObject;
      using (deserializedTransaction.EnterNonDiscardingScope ())
      {
        deserializedDomainObject = Computer.GetObject (computer.ID);
      }

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
      Assert.AreSame (deserializedTransaction, deserializedDataContainer.ClientTransaction);
      Assert.AreEqual (dataContainer.Timestamp, deserializedDataContainer.Timestamp);
      Assert.AreSame (deserializedDomainObject, deserializedDataContainer.DomainObject);
      Assert.AreEqual (StateType.Changed, deserializedDataContainer.State);
      Assert.AreEqual ("abc", deserializedDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "SerialNumber")].Value);
      Assert.AreEqual (employee.ID, deserializedDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "Employee")].Value);
    }

    [Test]
    public void DataContainer_MarkAsChanged_ContentsTest ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);

      Computer computer = employee.Computer;
      computer.MarkAsChanged ();

      DataContainer dataContainer = computer.InternalDataContainer;
      DataContainer deserializedDataContainer = Serializer.SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
      Assert.AreEqual (StateType.Changed, deserializedDataContainer.State);
    }

    [Test]
    public void DataContainer_WithoutProperties_ContentsTest ()
    {
      ObjectID objectID = new ObjectID (typeof (ClassWithoutProperties), Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);
      DataContainer deserializedDataContainer = Serializer.SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
      Assert.IsEmpty (deserializedDataContainer.PropertyValues);
    }

    [Test]
    public void DataContainer_Discarded_ContentsTest ()
    {
      Computer computer = Computer.NewObject ();
      DataContainer dataContainer = computer.InternalDataContainer;
      computer.Delete ();
      Assert.IsTrue (dataContainer.IsDiscarded);

      DataContainer deserializedDataContainer = Serializer.SerializeAndDeserialize (dataContainer);
      Assert.IsTrue (deserializedDataContainer.IsDiscarded);
      Assert.AreEqual (StateType.Discarded, deserializedDataContainer.State);
    }

    [Test]
    public void DataContainer_EventHandlers_ContentsTest ()
    {
      Computer computer = Computer.NewObject ();

      DataContainer dataContainer = computer.InternalDataContainer;
      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (dataContainer, false);

      Tuple<PropertyValueContainerEventReceiver, DataContainer> deserializedObjects =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (eventReceiver, dataContainer));

      PropertyValueContainerEventReceiver deserializedEventReceiver = deserializedObjects.A;
      DataContainer deserializedDataContainer = deserializedObjects.B;

      Assert.IsNull (deserializedEventReceiver.ChangingNewValue);
      Assert.IsNull (deserializedEventReceiver.ChangedNewValue);
      deserializedDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "SerialNumber")].Value = "1234";
      Assert.IsNotNull (deserializedEventReceiver.ChangingNewValue);
      Assert.IsNotNull (deserializedEventReceiver.ChangedNewValue);
    }
  }
}