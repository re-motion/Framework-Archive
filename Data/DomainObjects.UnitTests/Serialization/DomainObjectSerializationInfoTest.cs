using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Infrastructure;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DomainObjectSerializationInfoTest
  {
    [Test]
    public void Values ()
    {
      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo();
      serializationInfo.AddValue (1);
      serializationInfo.AddValue (new DateTime (2007, 1, 2));
      serializationInfo.AddValue ("Foo");
      serializationInfo.AddValue<object> (null);
      serializationInfo.AddValue<int?> (null);
      object[] data = serializationInfo.GetData();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      Assert.AreEqual (1, deserializationInfo.GetValue<int> ());
      Assert.AreEqual (new DateTime (2007, 1, 2), deserializationInfo.GetValue<DateTime> ());
      Assert.AreEqual ("Foo", deserializationInfo.GetValue<string> ());
      Assert.AreEqual (null, deserializationInfo.GetValue<int?> ());
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "The serialization stream contains an object of type System.Int32 at "
        + "position 0, but an object of type System.String was expected.")]
    public void InvalidDeserializedType ()
    {
      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      serializationInfo.AddValue (1);
      object[] data = serializationInfo.GetData ();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      deserializationInfo.GetValue<string> ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "The serialization stream contains a null value at "
        + "position 0, but an object of type System.Int32 was expected.")]
    public void InvalidDeserializedType_WithNullAndValueType ()
    {
      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      serializationInfo.AddValue<object> (null);
      object[] data = serializationInfo.GetData ();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      deserializationInfo.GetValue<int> ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "There is no more data in the serialization stream at position 0.")]
    public void InvalidNumberOfDeserializedItems ()
    {
      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      object[] data = serializationInfo.GetData ();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      deserializationInfo.GetValue<string> ();
    }

    [Test]
    public void Arrays ()
    {
      object[] array1 = new object[] { "Foo", 1, 3.0 };
      DateTime[] array2 = new DateTime[] { DateTime.MinValue, DateTime.MaxValue };

      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      serializationInfo.AddArray (array1);
      serializationInfo.AddArray (array2);
      object[] data = serializationInfo.GetData ();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      Assert.That (deserializationInfo.GetArray<object> (), Is.EqualTo (array1));
      Assert.That (deserializationInfo.GetArray<DateTime> (), Is.EqualTo (array2));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "The serialization stream contains an object of type System.String at "
        + "position 1, but an object of type System.Int32 was expected.")]
    public void InvalidArrayType ()
    {
      object[] array1 = new object[] { "Foo", 1, 3.0 };

      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      serializationInfo.AddArray (array1);
      object[] data = serializationInfo.GetData ();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      deserializationInfo.GetArray<int> ();
    }

    [Test]
    public void Handles ()
    {
      DateTime dt1 = DateTime.MinValue;
      DateTime dt2 = DateTime.MaxValue;

      string s1 = "Foo";
      string s2 = "Fox";

      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();

      serializationInfo.AddHandle (dt1);
      serializationInfo.AddHandle (dt2);
      serializationInfo.AddHandle (dt1);
      serializationInfo.AddHandle (dt1);
      serializationInfo.AddHandle (s1);
      serializationInfo.AddHandle (s2);
      serializationInfo.AddHandle (s1);
      serializationInfo.AddHandle (s1);
      serializationInfo.AddHandle (s2);

      object[] data = serializationInfo.GetData();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);

      Assert.AreEqual (dt1, deserializationInfo.GetValueForHandle<DateTime> ());
      Assert.AreEqual (dt2, deserializationInfo.GetValueForHandle<DateTime> ());
      Assert.AreEqual (dt1, deserializationInfo.GetValueForHandle<DateTime> ());
      Assert.AreEqual (dt1, deserializationInfo.GetValueForHandle<DateTime> ());
      Assert.AreEqual (s1, deserializationInfo.GetValueForHandle<string> ());
      Assert.AreEqual (s2, deserializationInfo.GetValueForHandle<string> ());
      Assert.AreEqual (s1, deserializationInfo.GetValueForHandle<string> ());
      Assert.AreEqual (s1, deserializationInfo.GetValueForHandle<string> ());
      Assert.AreEqual (s2, deserializationInfo.GetValueForHandle<string> ());
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "The serialization stream contains an object of type System.DateTime at "
        + "position 3, but an object of type System.String was expected.")]
    public void HandlesWithInvalidType ()
    {
      DateTime dt1 = DateTime.MinValue;

      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      serializationInfo.AddHandle (dt1);
      serializationInfo.AddHandle (dt1);

      object[] data = serializationInfo.GetData ();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      deserializationInfo.GetValueForHandle<DateTime> ();
      deserializationInfo.GetValueForHandle<string> ();
    }

    [Test]
    public void FlattenedSerializables ()
    {
      FlattenedSerializableStub stub = new FlattenedSerializableStub ("begone, foul fiend", 123);
      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      serializationInfo.AddValue (stub);
      object[] data = serializationInfo.GetData();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      FlattenedSerializableStub deserializedStub =
          deserializationInfo.GetValue<FlattenedSerializableStub> (FlattenedSerializableStub.DeserializeFromFlatStructure);

      Assert.AreEqual ("begone, foul fiend", deserializedStub.Data1);
      Assert.AreEqual (123, deserializedStub.Data2);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This method does not support deserialization of IFlattenedSerializable "
        + "implementations. Use the overload taking a deserializer instead.")]
    public void FlattenedSerializablesWithWrongDeserializationMethod ()
    {
      FlattenedSerializableStub stub = new FlattenedSerializableStub ("begone, foul fiend", 123);
      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      serializationInfo.AddValue (stub);
      object[] data = serializationInfo.GetData ();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      deserializationInfo.GetValue<FlattenedSerializableStub> ();
    }

    [Test]
    public void FlattenedSerializableHandles ()
    {
      FlattenedSerializableStub stub = new FlattenedSerializableStub ("begone, foul fiend", 123);
      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      serializationInfo.AddHandle (stub);
      serializationInfo.AddHandle (stub);
      serializationInfo.AddHandle (stub);
      object[] data = serializationInfo.GetData ();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      FlattenedSerializableStub deserializedStub1 =
          deserializationInfo.GetValueForHandle<FlattenedSerializableStub> (FlattenedSerializableStub.DeserializeFromFlatStructure);
      FlattenedSerializableStub deserializedStub2 =
          deserializationInfo.GetValueForHandle<FlattenedSerializableStub> (FlattenedSerializableStub.DeserializeFromFlatStructure);
      FlattenedSerializableStub deserializedStub3 =
          deserializationInfo.GetValueForHandle<FlattenedSerializableStub> (FlattenedSerializableStub.DeserializeFromFlatStructure);

      Assert.AreSame (deserializedStub1, deserializedStub2);
      Assert.AreSame (deserializedStub2, deserializedStub3);
      Assert.AreEqual ("begone, foul fiend", deserializedStub1.Data1);
      Assert.AreEqual (123, deserializedStub1.Data2);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This method does not support deserialization of IFlattenedSerializable "
        + "implementations. Use the overload taking a deserializer instead.")]
    public void FlattenedSerializableHandlesWithWrongDeserializationMethod ()
    {
      FlattenedSerializableStub stub = new FlattenedSerializableStub ("begone, foul fiend", 123);
      DomainObjectSerializationInfo serializationInfo = new DomainObjectSerializationInfo ();
      serializationInfo.AddHandle (stub);
      object[] data = serializationInfo.GetData ();

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (data);
      deserializationInfo.GetValueForHandle<FlattenedSerializableStub> ();
    }
  }
}