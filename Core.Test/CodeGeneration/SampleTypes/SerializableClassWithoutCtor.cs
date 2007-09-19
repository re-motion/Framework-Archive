using System;
using System.Runtime.Serialization;

namespace Rubicon.Core.UnitTests.CodeGeneration.SampleTypes
{
  [Serializable]
  public class SerializableClassWithoutCtor : ISerializable
  {
    public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
    {
    }
  }
}