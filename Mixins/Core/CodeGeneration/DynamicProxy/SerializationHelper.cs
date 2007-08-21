using System;
using System.Reflection;
using System.Runtime.Serialization;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  [Serializable]
  public class SerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (SerializationInfo info, StreamingContext context, object concreteObject,
        TargetClassDefinition configuration, object[] extensions, bool serializeBaseMembers)
    {
      info.SetType (typeof (SerializationHelper));

      info.AddValue ("__configuration.ConfigurationContext", configuration.ConfigurationContext);
      info.AddValue ("__extensions", extensions);

      object[] baseMemberValues;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (configuration.Type);
        baseMemberValues = FormatterServices.GetObjectData (concreteObject, baseMembers);
      }
      else
        baseMemberValues = null;

      info.AddValue ("__baseMemberValues", baseMemberValues);
    }

    private readonly IMixinTarget _deserializedObject;
    private readonly object[] _extensions;
    private readonly object[] _baseMemberValues;
    private readonly TargetClassDefinition _targetClassDefinition;

    public SerializationHelper (SerializationInfo info, StreamingContext context)
        : this (null, info, context)
    {
    }

    public SerializationHelper (Type concreteType, SerializationInfo info, StreamingContext context)
    {
      ClassContext configurationContext = (ClassContext) info.GetValue ("__configuration.ConfigurationContext", typeof (ClassContext));
      _targetClassDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (configurationContext);

      if (concreteType == null)
        concreteType = ConcreteTypeBuilder.Current.GetConcreteType (_targetClassDefinition);
      else
        ArgumentUtility.CheckTypeIsAssignableFrom ("concreteType", concreteType, _targetClassDefinition.Type);

      _extensions = (object[]) info.GetValue ("__extensions", typeof (object[]));
      Assertion.IsNotNull (_extensions);

      _baseMemberValues = (object[]) info.GetValue ("__baseMemberValues", typeof (object[]));

      // Usually, instantiate a deserialized object using GetSafeUninitializedObject.
      // However, _baseMemberValues being null means that the object itself manages its member deserialization via ISerializable. In such a case, we
      // need to use the deserialization constructor to instantiate the object.
      if (_baseMemberValues != null)
        _deserializedObject = (IMixinTarget) FormatterServices.GetSafeUninitializedObject (concreteType);
      else
      {
        Assertion.IsTrue (typeof (ISerializable).IsAssignableFrom (concreteType));
        _deserializedObject = (IMixinTarget) Activator.CreateInstance (concreteType, new object[] {info, context});
      }
    }

    public object GetRealObject (StreamingContext context)
    {
      return _deserializedObject;
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException ("This should never be called.");
    }

    // Here, we can rely on everything being deserialized as needed.
    public void OnDeserialization (object sender)
    {
      if (_baseMemberValues != null)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (_targetClassDefinition.Type);
        FormatterServices.PopulateObjectMembers (_deserializedObject, baseMembers, _baseMemberValues);
      }

      ConcreteTypeBuilder.Current.Scope.InitializeDeserializedMixinTarget (_deserializedObject, _extensions);
    }
  }
}
