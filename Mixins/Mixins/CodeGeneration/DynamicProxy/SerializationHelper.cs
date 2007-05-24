using System;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Mixins;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration.DynamicProxy
{
  [Serializable]
  public class SerializationHelper : IObjectReference, ISerializable
  {
    private static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter s_formatter = new BinaryFormatter (); // HACK: this is used to circumvent serialization bug

    public static void GetObjectDataForGeneratedTypes (SerializationInfo info, StreamingContext context, object concreteObject,
        BaseClassDefinition configuration, object[] extensions, bool serializeBaseMembers)
    {
      info.SetType (typeof (SerializationHelper));

      // TODO: serialize context instead of configuration
      // Remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any ordering

      // info.AddValue ("__configuration", configuration); => doesn't work, CLR bug, serialize into byte array

      using (System.IO.MemoryStream stream = new MemoryStream ())
      {
        s_formatter.Serialize (stream, configuration);
        info.AddValue ("__configurationBytes", stream.ToArray ());
      }

      SerializeArray(extensions, "__extension", info);

      object[] baseMemberValues;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (concreteObject.GetType().BaseType);
        baseMemberValues = FormatterServices.GetObjectData (concreteObject, baseMembers);
      }
      else
      {
        baseMemberValues = null;
      }
      SerializeArray (baseMemberValues, "__baseMemberValue", info);
    }

    // to work around CLR bug
    private static void SerializeArray(object[] array, string id, SerializationInfo info)
    {
      if (array != null)
      {
        info.AddValue (id + "Count", array.Length);
        for (int i = 0; i < array.Length; ++i)
        {
          info.AddValue (id + "_" + i, array[i]);
        }
      }
      else
      {
        info.AddValue (id + "Count", -1);
      }
    }

    private object _deserializedObject;

    public SerializationHelper (SerializationInfo info, StreamingContext context)
    {
      // BaseClassDefinition configuration = (BaseClassDefinition) info.GetValue ("__configuration", typeof (BaseClassDefinition));

      BaseClassDefinition configuration;
      byte[] configurationBytes = (byte[]) info.GetValue ("__configurationBytes", typeof (byte[]));

      using (System.IO.MemoryStream stream = new MemoryStream (configurationBytes))
      {
        configuration = (BaseClassDefinition) s_formatter.Deserialize (stream);
      }

      Type concreteType = ConcreteTypeBuilder.Current.GetConcreteType (configuration);

      object[] extensions = DeserializeArray<object> ("__extension", info);
      object[] baseMemberValues = DeserializeArray<object> ("__baseMemberValue", info);

      if (baseMemberValues != null)
      {
        _deserializedObject = FormatterServices.GetSafeUninitializedObject (concreteType);
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers(concreteType.BaseType);
        FormatterServices.PopulateObjectMembers (_deserializedObject, baseMembers, baseMemberValues);
      }
      else
      {
        Assertion.Assert (typeof (ISerializable).IsAssignableFrom(concreteType));
        _deserializedObject = Activator.CreateInstance (concreteType, new object[] {info, context});
      }

      Assertion.Assert (extensions != null);
      ConcreteTypeBuilder.Current.Scope.InitializeInstance (_deserializedObject, extensions);
    }

    private T[] DeserializeArray<T> (string id, SerializationInfo info)
    {
      int length = info.GetInt32 (id + "Count");
      if (length == -1)
      {
        return null;
      }
      else
      {
        T[] array = new T[length];
        for (int i = 0; i < length; ++i)
        {
          array[i] = (T) info.GetValue (id + "_" + i, typeof (T));
        }
        return array;
      }
    }

    public object GetRealObject (StreamingContext context)
    {
      return _deserializedObject;
    }
    
    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException("This should never be called.");
    }

    public static void InitializeDeserializedMixin (object mixinInstance)
    {
      // ConcreteTypeBuilder.Current.Scope.InitializeMixinInstance (mixinInstance);
    }
  }
}
