using System;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Development.UnitTesting;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    [Test]
    public void DataManagerIsSerializable ()
    {
      DataManager dataManager = new DataManager (ClientTransactionMock);
      DataManager dataManager2 = Serializer.SerializeAndDeserialize (dataManager);
      Assert.IsNotNull (dataManager2);
      Assert.AreNotSame (dataManager2, dataManager);
    }

    [Test]
    public void DataManager_Content ()
    {
      DataManager dataManager = ClientTransactionMock.DataManager;
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Dev.Null = order.OrderItems[0];

      Order discardedOrder = Order.NewObject();
      DataContainer discardedContainer = discardedOrder.InternalDataContainer;
      discardedOrder.Delete();

      Assert.AreNotEqual (0, dataManager.DataContainerMap.Count);
      Assert.AreNotEqual (0, dataManager.RelationEndPointMap.Count);
      Assert.AreEqual (1, dataManager.DiscardedObjectCount);
      Assert.IsTrue (dataManager.IsDiscarded (discardedContainer.ID));
      Assert.AreSame (discardedContainer, dataManager.GetDiscardedDataContainer(discardedContainer.ID));

      Tuple<ClientTransaction, DataManager, DataContainer> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, dataManager, discardedContainer));

      Assert.AreNotEqual (0, deserializedData.B.DataContainerMap.Count);
      Assert.AreNotEqual (0, deserializedData.B.RelationEndPointMap.Count);
      Assert.AreEqual (1, deserializedData.B.DiscardedObjectCount);
      Assert.IsTrue (deserializedData.B.IsDiscarded (discardedContainer.ID));
      Assert.AreSame (deserializedData.C, deserializedData.B.GetDiscardedDataContainer (discardedContainer.ID));

      Assert.AreSame (deserializedData.A, PrivateInvoke.GetNonPublicField (deserializedData.B, "_clientTransaction"));
      Assert.IsNotNull (PrivateInvoke.GetNonPublicField (deserializedData.B, "_transactionEventSink"));
    }
  }
}