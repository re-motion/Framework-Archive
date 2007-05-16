using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mixins.CodeGeneration.DynamicProxy
{
  [Serializable]
  public class BaseCallProxySerializationHelper : IObjectReference, ISerializable
  {
    private object _deserializedObject;

    public static void GetObjectDataForBaseCallProxy (SerializationInfo info, StreamingContext context, object baseCallProxy, int depth,
        object target)
    {
      info.SetType (typeof (BaseCallProxySerializationHelper));
      info.AddValue ("__depth", baseCallProxy.GetType().GetField ("__depth").GetValue (baseCallProxy));
      info.AddValue ("__this", baseCallProxy.GetType ().GetField ("__this").GetValue (baseCallProxy));
    }

    public BaseCallProxySerializationHelper(SerializationInfo info, StreamingContext context)
    {
      _deserializedObject = null;
    }

    public object GetRealObject (StreamingContext context)
    {
      throw new NotImplementedException();
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException("This method should never be called.");
    }
  }
}
