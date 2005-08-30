using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
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

  [Test]
  public void SerializeNaByte ()
  {
    CheckSerialization (NaByte.Null);
    CheckSerialization (new NaByte (32));
    CheckSerialization (NaByte.Zero);
    CheckSerialization (NaByte.MinValue);
    CheckSerialization (NaByte.MaxValue);
  }

  [Test]
  public void SerializeNaDecimal ()
  {
    CheckSerialization (NaDecimal.Null);
    CheckSerialization (new NaDecimal (32));
    CheckSerialization (new NaDecimal (-32));
    CheckSerialization (new NaDecimal (new decimal (321.12345)));
    CheckSerialization (new NaDecimal (new decimal (-321.12345)));
    CheckSerialization (NaDecimal.Zero);
    CheckSerialization (NaDecimal.MinValue);
    CheckSerialization (NaDecimal.MaxValue);
  }

  [Test]
  public void SerializeNaGuid ()
  {
    CheckSerialization (NaGuid.Null);
    CheckSerialization (new NaGuid (Guid.NewGuid ()));
    CheckSerialization (NaGuid.Empty);
  }

  [Test]
  public void SerializeNaInt16()
  {
    CheckSerialization (NaInt16.Null);
    CheckSerialization (new NaInt16 (32));
    CheckSerialization (new NaInt16 (-32));
    CheckSerialization (NaInt16.Zero);
    CheckSerialization (NaInt16.MinValue);
    CheckSerialization (NaInt16.MaxValue);
  }

  [Test]
  public void SerializeNaInt64()
  {
    CheckSerialization (NaInt64.Null);
    CheckSerialization (new NaInt64 (32));
    CheckSerialization (new NaInt64 (-32));
    CheckSerialization (NaInt64.Zero);
    CheckSerialization (NaInt64.MinValue);
    CheckSerialization (NaInt64.MaxValue);
  }

  [Test]
  public void SerializeNaSingle ()
  {
    CheckSerialization (NaSingle.Null);
    CheckSerialization (new NaSingle (32));
    CheckSerialization (new NaSingle (-32));
    CheckSerialization (new NaSingle (321.12345F));
    CheckSerialization (new NaSingle (-321.12345F));
    CheckSerialization (NaSingle.Zero);
    CheckSerialization (NaSingle.MinValue);
    CheckSerialization (NaSingle.MaxValue);
  }

  [Test]
  public void SerializeNaDouble ()
  {
    CheckSerialization (NaDouble.Null);
    CheckSerialization (new NaDouble (32));
    CheckSerialization (new NaDouble (-32));
    CheckSerialization (new NaDouble (321.12345F));
    CheckSerialization (new NaDouble (-321.12345F));
    CheckSerialization (NaDouble.Zero);
    CheckSerialization (NaDouble.MinValue);
    CheckSerialization (NaDouble.MaxValue);
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