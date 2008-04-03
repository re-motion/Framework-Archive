using System;
using System.Runtime.Serialization;

namespace Remotion.Core.UnitTests.CodeGeneration.SampleTypes
{
  [Serializable]
  public class SerializableClassWithoutCtor : ISerializable
  {
    public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
    {
    }
  }
}