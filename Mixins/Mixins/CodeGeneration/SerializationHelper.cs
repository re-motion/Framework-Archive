using System;
using System.Runtime.Serialization;
using System.Reflection;
using Mixins.Definitions;

namespace Mixins.CodeGeneration
{
  public class SerializationHelper /*: IObjectReference, ISerializable*/
  {
    public static void GetObjectDataHelper (SerializationInfo info, StreamingContext context, object concreteObject,
        BaseClassDefinition configuration, object[] extensions, bool serializeBaseMembers)
    {
      info.SetType (typeof (SerializationHelper));

      info.AddValue("__configuration", configuration);
      
      for (int i = 0; i < extensions.Length; ++i)
      {
        info.AddValue ("__extenstion_" + i, extensions[i]);
      }

      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (concreteObject.GetType ().BaseType);
        object[] baseMemberValues = FormatterServices.GetObjectData(concreteObject, baseMembers);
        info.AddValue ("__baseMembers", baseMemberValues);
      }
    }

/*    private object concreteObject;

    public SerializationHelper (SerializationInfo info, StreamingContext context)
		{
      BaseClassDefinition configuration = (BaseClassDefinition) info.GetValue ("__configuration", typeof (BaseClassDefinition));
		}

    public object GetRealObject (StreamingContext context)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new Exception ("The method or operation is not implemented.");
    }*/
  }
}
