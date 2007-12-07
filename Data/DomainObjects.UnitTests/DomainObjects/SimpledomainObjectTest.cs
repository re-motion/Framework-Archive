using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class SimpleDomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewObject ()
    {
      ClassDerivedFromSimpleDomainObject instance = SimpleDomainObject.NewObject<ClassDerivedFromSimpleDomainObject>().With();
      Assert.IsNotNull (instance);
      Assert.AreEqual (0, instance.IntProperty);
      instance.IntProperty = 5;
      Assert.AreEqual (5, instance.IntProperty);
    }

    [Test]
    public void GetObject_WithoutDeleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = SimpleDomainObject.NewObject<ClassDerivedFromSimpleDomainObject> ().With ();
      instance.IntProperty = 7;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = SimpleDomainObject.GetObject<ClassDerivedFromSimpleDomainObject> (instance.ID);
        Assert.AreSame (instance, gottenInstance);
        Assert.AreEqual (7, gottenInstance.IntProperty);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedFalse_NonDeleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = SimpleDomainObject.NewObject<ClassDerivedFromSimpleDomainObject> ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = SimpleDomainObject.GetObject<ClassDerivedFromSimpleDomainObject> (instance.ID, false);
        Assert.AreSame (instance, gottenInstance);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException), ExpectedMessage = "Object '.*' is already deleted.", MatchType = MessageMatch.Regex)]
    public void GetObject_IncludeDeletedFalse_Deleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = SimpleDomainObject.NewObject<ClassDerivedFromSimpleDomainObject> ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instance.Delete();
        SimpleDomainObject.GetObject<ClassDerivedFromSimpleDomainObject> (instance.ID, false);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_NonDeleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = SimpleDomainObject.NewObject<ClassDerivedFromSimpleDomainObject> ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = SimpleDomainObject.GetObject<ClassDerivedFromSimpleDomainObject> (instance.ID, true);
        Assert.AreSame (instance, gottenInstance);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_Deleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = SimpleDomainObject.NewObject<ClassDerivedFromSimpleDomainObject> ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instance.Delete ();
        ClassDerivedFromSimpleDomainObject gottenInstance = SimpleDomainObject.GetObject<ClassDerivedFromSimpleDomainObject> (instance.ID, true);
        Assert.AreSame (instance, gottenInstance);
        Assert.AreEqual (StateType.Deleted, gottenInstance.State);
      }
    }

    [Test]
    public void Serializable ()
    {
      ClassDerivedFromSimpleDomainObject instance = SimpleDomainObject.NewObject<ClassDerivedFromSimpleDomainObject> ().With ();
      instance.IntProperty = 6;
      Tuple<ClientTransaction, ClassDerivedFromSimpleDomainObject> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, instance));
      ClassDerivedFromSimpleDomainObject deserializedInstance = deserializedData.B;
      using (deserializedData.A.EnterNonDiscardingScope ())
      {
        Assert.AreNotSame (instance, deserializedInstance);
        Assert.AreEqual (6, deserializedInstance.IntProperty);
      }
    }

    [DBTable]
    [Instantiable]
    [Serializable]
    public abstract class ClassDerivedFromSimpleDomainObjectImplementingISerializable : SimpleDomainObject, ISerializable
    {
      public ClassDerivedFromSimpleDomainObjectImplementingISerializable ()
      {
      }

      public ClassDerivedFromSimpleDomainObjectImplementingISerializable (SerializationInfo info, StreamingContext context)
          : base (info, context)
      {
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        BaseGetObjectData (info, context);
      }

      public abstract int IntProperty { get; set; }
    }

    [Test]
    public void Serializable_WithISerializable ()
    {
      ClassDerivedFromSimpleDomainObjectImplementingISerializable instance =
          SimpleDomainObject.NewObject<ClassDerivedFromSimpleDomainObjectImplementingISerializable> ().With ();
      instance.IntProperty = 5;
      Tuple<ClientTransaction, ClassDerivedFromSimpleDomainObjectImplementingISerializable> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, instance));
      ClassDerivedFromSimpleDomainObjectImplementingISerializable deserializedInstance = deserializedData.B;
      using (deserializedData.A.EnterNonDiscardingScope ())
      {
        Assert.AreNotSame (instance, deserializedInstance);
        Assert.AreEqual (5, deserializedInstance.IntProperty);
      }
    }

  }
}