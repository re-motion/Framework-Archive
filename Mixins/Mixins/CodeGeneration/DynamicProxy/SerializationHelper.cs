using System;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Mixins;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Rubicon.Utilities;
using Mixins.Context;

namespace Mixins.CodeGeneration.DynamicProxy
{
  [Serializable]
  public class SerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (SerializationInfo info, StreamingContext context, object concreteObject,
        BaseClassDefinition configuration, object[] extensions, bool serializeBaseMembers)
    {
      info.SetType (typeof (SerializationHelper));

      info.AddValue ("__configuration.ConfigurationContext", configuration.ConfigurationContext);
      info.AddValue ("__extensions", extensions);

      object[] baseMemberValues;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (concreteObject.GetType().BaseType);
        baseMemberValues = FormatterServices.GetObjectData (concreteObject, baseMembers);
      }
      else
        baseMemberValues = null;

      info.AddValue ("__baseMemberValues", baseMemberValues);
    }

    private IMixinTarget _deserializedObject;
    private object[] _extensions;
    private object[] _baseMemberValues;
    private BaseClassDefinition _baseClassDefinition;

    public SerializationHelper (SerializationInfo info, StreamingContext context)
    {
      ClassContext configurationContext = (ClassContext) info.GetValue ("__configuration.ConfigurationContext", typeof (ClassContext));
      _baseClassDefinition = BaseClassDefinitionCache.Current.GetBaseClassDefinition (configurationContext);

      Type concreteType = ConcreteTypeBuilder.Current.GetConcreteType (_baseClassDefinition);

      _extensions = (object[]) info.GetValue ("__extensions", typeof (object[]));
      Assertion.Assert (_extensions != null);

      _baseMemberValues = (object[]) info.GetValue ("__baseMemberValues", typeof (object[]));

      // Usually, instantiate a deserialized object using GetSafeUninitializedObject.
      // However, _baseMemberValues being null means that the object itself manages its member deserialization via ISerializable. In such a case, we
      // need to use the deserialization constructor to instantiate the object.
      if (_baseMemberValues != null)
        _deserializedObject = (IMixinTarget) FormatterServices.GetSafeUninitializedObject (concreteType);
      else
      {
        Assertion.Assert (typeof (ISerializable).IsAssignableFrom (concreteType));
        _deserializedObject = (IMixinTarget) Activator.CreateInstance (concreteType, new object[] {info, context});
      }
    }

    public object GetRealObject (StreamingContext context)
    {
      return _deserializedObject;
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException("This should never be called.");
    }

    // Here, we can rely on everything being deserialized as needed.
    public void OnDeserialization (object sender)
    {
      if (_baseMemberValues != null)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (_deserializedObject.GetType ().BaseType);
        FormatterServices.PopulateObjectMembers (_deserializedObject, baseMembers, _baseMemberValues);
      }

      ConcreteTypeBuilder.Current.Scope.InitializeInstance (_deserializedObject, _extensions);
      for (int i = 0; i < _extensions.Length; ++i)
        ConcreteTypeBuilder.Current.Scope.InitializeMixinInstance (_baseClassDefinition.Mixins[i], _extensions[i], _deserializedObject);
    }
  }
}
