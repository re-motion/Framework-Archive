using System;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Mixins;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.Utilities;
using Rubicon.Utilities;
using Mixins.Context;

namespace Mixins.CodeGeneration.DynamicProxy
{
  [Serializable]
  public class MixinSerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (SerializationInfo info, StreamingContext context, object mixin, MixinDefinition configuration,
        bool serializeBaseMembers)
    {
      info.SetType (typeof (MixinSerializationHelper));

      ClassContext targetClassContext = configuration.BaseClass.ConfigurationContext;
      info.AddValue ("__configuration.BaseClass.ConfigurationContext", targetClassContext);
      info.AddValue ("__configuration.MixinIndex", configuration.MixinIndex);

      object[] baseMemberValues;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (mixin.GetType ().BaseType);
        baseMemberValues = FormatterServices.GetObjectData (mixin, baseMembers);
      }
      else
        baseMemberValues = null;

      info.AddValue ("__baseMemberValues", baseMemberValues);
    }

    private MixinDefinition _mixinDefinition;
    private object[] _baseMemberValues;
    private object _deserializedObject;

    public MixinSerializationHelper (SerializationInfo info, StreamingContext context)
    {
      ClassContext targetClassContext = (ClassContext) info.GetValue ("__configuration.BaseClass.ConfigurationContext", typeof (ClassContext));
      BaseClassDefinition targetClassDefinition = BaseClassDefinitionCache.Current.GetBaseClassDefinition (targetClassContext);

      int mixinIndex = info.GetInt32 ("__configuration.MixinIndex");
      _mixinDefinition = targetClassDefinition.Mixins[mixinIndex];

      Type concreteType = ConcreteTypeBuilder.Current.GetConcreteMixinType (_mixinDefinition);
      _baseMemberValues = (object[]) info.GetValue ("__baseMemberValues", typeof (object[]));

      // Usually, instantiate a deserialized object using GetSafeUninitializedObject.
      // However, _baseMemberValues being null means that the object itself manages its member deserialization via ISerializable. In such a case, we
      // need to use the deserialization constructor to instantiate the object.
      if (_baseMemberValues != null)
        _deserializedObject = FormatterServices.GetSafeUninitializedObject (concreteType);
      else
      {
        Assertion.Assert (typeof (ISerializable).IsAssignableFrom (concreteType));
        _deserializedObject = Activator.CreateInstance (concreteType, new object[] { info, context });
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
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (_deserializedObject.GetType ().BaseType);
        FormatterServices.PopulateObjectMembers (_deserializedObject, baseMembers, _baseMemberValues);
      }
    }
  }
}
