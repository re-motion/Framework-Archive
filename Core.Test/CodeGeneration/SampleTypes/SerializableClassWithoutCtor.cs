using System;
using System.Runtime.Serialization;

namespace Remotion.UnitTests.CodeGeneration.SampleTypes
{
  [Serializable]
  public class SerializableClassWithoutCtor : ISerializable
  {
    public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
    {
    }
  }
}