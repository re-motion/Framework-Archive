using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  public class FlattenedSerializableStub : IFlattenedSerializable
  {
    public readonly string Data1;
    public readonly int Data2;
    public FlattenedSerializableStub Data3;

    public FlattenedSerializableStub (string data1, int data2)
    {
      Data1 = data1;
      Data2 = data2;
    }

    protected FlattenedSerializableStub (FlattenedDeserializationInfo info)
    {
      Data1 = info.GetValue<string> ();
      Data2 = info.GetValue<int> ();
      Data3 = info.GetValueForHandle<FlattenedSerializableStub> ();
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddValue (Data1);
      info.AddValue (Data2);
      info.AddHandle (Data3);
    }
  }
}