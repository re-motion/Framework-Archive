using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DataContainerMapTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Rubicon.Data.DomainObjects.DataManagemenet.DataContainerMap' in Assembly "
        + "'Rubicon.Data.DomainObjects, Version=1.7.65.202, Culture=neutral, PublicKeyToken=ad97c3e83e217fcd' is not marked as serializable.")]
    [Ignore ("TODO: FS - after finishing flattened serializable implementation")]
    public void DataContainerMapIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (ClientTransactionMock.DataManager.DataContainerMap);
    }

    [Test]
    public void DataContainerMapIsFlattenedSerializable ()
    {
      DataContainerMap map = ClientTransactionMock.DataManager.DataContainerMap;
      DomainObjectSerializationInfo info = new DomainObjectSerializationInfo ();
      ((IFlattenedSerializable) map).SerializeIntoFlatStructure (info);

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (info.GetData ());
      DataContainerMap deserializedMap = DataContainerMap.DeserializeFromFlatStructure (deserializationInfo);

      Assert.IsNotNull (deserializedMap);
    }

    [Test]
    public void DataContainerMap_Content ()
    {
      DataContainerMap map = ClientTransactionMock.DataManager.DataContainerMap;
      Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (1, map.Count);
      
      DomainObjectSerializationInfo info = new DomainObjectSerializationInfo();
      ((IFlattenedSerializable) map).SerializeIntoFlatStructure (info);
      
      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (info.GetData ());
      DataContainerMap deserializedMap = DataContainerMap.DeserializeFromFlatStructure (deserializationInfo);

      Assert.AreEqual (ClientTransactionMock, PrivateInvoke.GetNonPublicField (deserializedMap, "_clientTransaction"));
      Assert.IsNotNull (PrivateInvoke.GetNonPublicField (deserializedMap, "_transactionEventSink"));
      Assert.AreEqual (1, deserializedMap.Count);

    }
  }
}