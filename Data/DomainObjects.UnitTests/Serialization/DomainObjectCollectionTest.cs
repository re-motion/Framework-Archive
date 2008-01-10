using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DomainObjectCollectionTest : ClientTransactionBaseTest
  {
    [Test]
    public void DomainObjectCollectionIsSerializable ()
    {
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));

      DomainObjectCollection deserializedCollection = Serializer.SerializeAndDeserialize (collection);
      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
    }

    [Test]
    public void DomainObjectCollectionIsFlattenedSerializable ()
    {
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.AreNotSame (collection, deserializedCollection);
      Assert.IsNotNull (deserializedCollection);
    }

    [Test]
    public void DomainObjectCollection_Contents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Order));
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));
      long version = (long) PrivateInvoke.GetNonPublicField (collection, "_version");

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
      Assert.AreEqual (typeof (Order), deserializedCollection.RequiredItemType);
      Assert.IsFalse (deserializedCollection.IsReadOnly);
      Assert.IsNull (PrivateInvoke.GetNonPublicField (deserializedCollection, "_changeDelegate"));
      Assert.AreEqual (version, PrivateInvoke.GetNonPublicField (collection, "_version"));
    }

    [Test]
    public void DomainObjectCollection_Events_Contents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Order));
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));
      long version = (long) PrivateInvoke.GetNonPublicField (collection, "_version");

      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
      Assert.AreEqual (typeof (Order), deserializedCollection.RequiredItemType);
      Assert.IsFalse (deserializedCollection.IsReadOnly);
      Assert.IsNull (PrivateInvoke.GetNonPublicField (deserializedCollection, "_changeDelegate"));
      Assert.AreEqual (version, PrivateInvoke.GetNonPublicField (collection, "_version"));

      Assert.IsFalse (eventReceiver.HasAddedEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasAddingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRemovedEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRemovingEventBeenCalled);

      deserializedCollection.Add (Order.NewObject());
      deserializedCollection.RemoveAt (0);

      Assert.IsTrue (eventReceiver.HasAddedEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRemovedEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRemovingEventBeenCalled);
    }

    [Test]
    public void DomainObjectCollection_ReadOnlyContents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Order));
      collection = new DomainObjectCollection (collection, true);

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.IsTrue (deserializedCollection.IsReadOnly);
    }

    private DomainObjectCollection SerializeAndDeserialize (DomainObjectCollection source)
    {
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo();
      source.SerializeIntoFlatStructure (serializationInfo);
      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (serializationInfo.GetData());
      DomainObjectCollection target = new DomainObjectCollection();
      target.DeserializeFromFlatStructure (deserializationInfo);
      return target;
    }
  }
}