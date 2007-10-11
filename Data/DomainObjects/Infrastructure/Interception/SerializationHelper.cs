using System;
using System.Runtime.Serialization;
using System.Reflection;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  [Serializable]
  public class SerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    private readonly DomainObject _realObject;
    private readonly IObjectReference _nestedObjectReference;
    private readonly object[] _baseMemberValues;
    private readonly Type _baseType;
    private readonly ObjectID _id;

    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (SerializationInfo info, StreamingContext context, DomainObject concreteObject,
        bool serializeBaseMembers)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("concreteObject", concreteObject);

      info.SetType (typeof (SerializationHelper));

      Type baseType = concreteObject.GetPublicDomainObjectType();
      info.AddValue ("BaseType.AssemblyQualifiedName", baseType.AssemblyQualifiedName);
      
      object[] baseMemberValues = null;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (baseType);
        baseMemberValues = FormatterServices.GetObjectData (concreteObject, baseMembers);
      }
      info.AddValue ("BaseType.Data", baseMemberValues);

      if (!serializeBaseMembers)
        info.AddValue ("DomainObject.ID", concreteObject.ID);
    }

    public SerializationHelper (SerializationInfo info, StreamingContext context)
    {
      string baseTypeName = info.GetString ("BaseType.AssemblyQualifiedName");
      _baseMemberValues = (object[]) info.GetValue ("BaseType.Data", typeof (object[]));
      _baseType = Type.GetType (baseTypeName);

      // TODO: This doesn't hold true if the MixinConfiguration changes between serialization and deserialization. This is likely not supported,
      // but should throw a nicer exception than this assertion error.
      Assertion.IsTrue (_baseMemberValues == null || DomainObjectMixinCodeGenerationBridge.GetConcreteType (_baseType) == _baseType,
          "If a DomainObject has mixins, we certainly haven't serialized its base members.");

      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteDomainObjectType = factory.GetConcreteDomainObjectType (_baseType);

      // Usually, instantiate a deserialized object using GetSafeUninitializedObject.
      // However, _baseMemberValues being null means that the object itself manages its member deserialization via ISerializable. In such a case, we
      // need to use the deserialization constructor to instantiate the object.
      if (_baseMemberValues != null)
      {
        _realObject = (DomainObject) FormatterServices.GetSafeUninitializedObject (concreteDomainObjectType);
        factory.PrepareUnconstructedInstance (_realObject);
      }
      else
      {
        _nestedObjectReference = DomainObjectMixinCodeGenerationBridge.BeginDeserialization (concreteDomainObjectType, info, context);
        _realObject = (DomainObject) _nestedObjectReference.GetRealObject (context);
        _id = (ObjectID) info.GetValue ("DomainObject.ID", typeof (ObjectID));
      }
    }

    public object GetRealObject (StreamingContext context)
    {
      return _realObject;
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException ("This method should never be called.");
    }

    public void OnDeserialization (object sender)
    {
      if (_baseMemberValues != null)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (_baseType);
        FormatterServices.PopulateObjectMembers (_realObject, baseMembers, _baseMemberValues);
      }
      else
      {
        Assertion.IsNotNull (_nestedObjectReference);
        DomainObjectMixinCodeGenerationBridge.FinishDeserialization (_nestedObjectReference);

        if (!object.Equals (_realObject.ID, _id))
        {
          string message = string.Format (
              "The deserialization constructor on type {0} did not call DomainObject's base deserialization constructor.", _baseType.FullName);
          throw new SerializationException (message);
        }
      }

      IDeserializationCallback objectAsDeserializationCallback = _realObject as IDeserializationCallback;
      if (objectAsDeserializationCallback != null)
        objectAsDeserializationCallback.OnDeserialization (sender);
    }
  }
}
