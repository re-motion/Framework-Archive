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
    private readonly object[] _baseMembers;
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
      
      object[] baseMembers = null;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseFields = FormatterServices.GetSerializableMembers (baseType);
        baseMembers = FormatterServices.GetObjectData (concreteObject, baseFields);
      }
      info.AddValue ("BaseType.Data", baseMembers);

      if (!serializeBaseMembers)
        info.AddValue ("DomainObject.ID", concreteObject.ID);
    }

    public SerializationHelper (SerializationInfo info, StreamingContext context)
    {
      string baseTypeName = info.GetString ("BaseType.AssemblyQualifiedName");
      _baseMembers = (object[]) info.GetValue ("BaseType.Data", typeof (object[]));
      _baseType = Type.GetType (baseTypeName);

      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType (_baseType);

      if (typeof (ISerializable).IsAssignableFrom (_baseType))
      {
        _realObject = factory.GetTypesafeConstructorInvoker<DomainObject> (concreteType).With (info, context);
        _id = (ObjectID) info.GetValue ("DomainObject.ID", typeof (ObjectID));
      }
      else
      {
        _realObject = (DomainObject) FormatterServices.GetSafeUninitializedObject (concreteType);
        factory.PrepareUnconstructedInstance (_realObject);
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
      if (_baseMembers != null)
      {
        MemberInfo[] baseFields = FormatterServices.GetSerializableMembers (_baseType);
        FormatterServices.PopulateObjectMembers (_realObject, baseFields, _baseMembers);
      }
      else if (!object.Equals (_realObject.ID, _id))
      {
        string message = string.Format (
            "The deserialization constructor on type {0} did not call DomainObject's base deserialization constructor.", _baseType.FullName);
        throw new SerializationException (message);
      }
    }
  }
}
