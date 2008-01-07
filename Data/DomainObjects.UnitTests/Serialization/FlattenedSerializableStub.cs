using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  public class FlattenedSerializableStub : IFlattenedSerializable
  {
    public static FlattenedSerializableStub DeserializeFromFlatStructure (DomainObjectDeserializationInfo info)
    {
      return new FlattenedSerializableStub (info.GetValue<string>(), info.GetValue<int>());
    }

    public readonly string Data1;
    public readonly int Data2;

    public FlattenedSerializableStub (string data1, int data2)
    {
      Data1 = data1;
      Data2 = data2;
    }

    public void SerializeIntoFlatStructure (DomainObjectSerializationInfo info)
    {
      info.AddValue (Data1);
      info.AddValue (Data2);
    }
  }
}