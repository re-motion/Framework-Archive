using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

using Rubicon.Data.NullableValueTypes;

namespace Rubicon.Test.Data.NullableValueTypesTest
{

[TestFixture]
public class SerializationTests
{
  [Test]
  public void SerializeNaInt32()
  {
    CheckSerialization (NaInt32.Null);
    CheckSerialization (new NaInt32 (32));
    CheckSerialization (new NaInt32 (-32));
    CheckSerialization (NaInt32.Zero);
    CheckSerialization (NaInt32.MinValue);
    CheckSerialization (NaInt32.MaxValue);
  }

  private object SerializeAndDeserialize (object obj)
  {
    MemoryStream stream = new MemoryStream ();
    BinaryFormatter formatter = new BinaryFormatter ();
    formatter.Serialize (stream, obj);
    stream.Position = 0;
    return formatter.Deserialize (stream);
  }

  private void CheckSerialization (object obj)
  {
    object result = SerializeAndDeserialize (obj);
    Assertion.AssertEquals ("Serialization and deserialization changed the value of the argument.", obj, result);
  }

}


}