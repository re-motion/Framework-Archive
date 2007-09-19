using System;
using System.Runtime.Serialization;

namespace Rubicon.Core.UnitTests.CodeGeneration.SampleTypes
{
  [Serializable]
  public class SerializableClassWithPrivateGetObjectData : ISerializable
  {
    public SerializableClassWithPrivateGetObjectData ()
    {
    }

    protected SerializableClassWithPrivateGetObjectData (SerializationInfo info, StreamingContext context)
    {
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
    }
  }
}