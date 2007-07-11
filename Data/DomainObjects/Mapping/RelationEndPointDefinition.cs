using System;
using System.Runtime.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
[Serializable]
public class RelationEndPointDefinition : IRelationEndPointDefinition, ISerializable, IObjectReference
{
  // types

  // static members and constants

  // member fields

  private RelationDefinition _relationDefinition;
  private ClassDefinition _classDefinition;
  private PropertyDefinition _propertyDefinition;
  private bool _isMandatory;

  // Note: _mappingClassID and _mappingPropertyName are used only during the deserialization process. 
  // They are set only in the deserialization constructor and are used in IObjectReference.GetRealObject.
  private string _mappingClassID;
  private string _mappingPropertyName;

  // construction and disposing

  public RelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory) 
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    
    PropertyDefinition propertyDefinition = classDefinition[propertyName];
    if (propertyDefinition == null)
      throw CreateMappingException ("Relation definition error for end point: Class '{0}' has no property '{1}'.", classDefinition.ID, propertyName);

    if (!propertyDefinition.IsObjectID)
    {
      throw CreateMappingException (
          "Relation definition error: Property '{0}' of class '{1}' is of type '{2}', but non-virtual properties must be of type '{3}'.",
          propertyDefinition.PropertyName, classDefinition.ID, propertyDefinition.PropertyType, typeof (ObjectID));
    }

    _classDefinition = classDefinition;    
    _isMandatory = isMandatory;
    _propertyDefinition = propertyDefinition;
  }

  protected RelationEndPointDefinition (SerializationInfo info, StreamingContext context)
  {
    bool ispartOfMappingConfiguration = info.GetBoolean ("IsPartOfMappingConfiguration");

    if (ispartOfMappingConfiguration)
    {
      // Note: If this object was part of MappingConfiguration.Current during the serialization process,
      // it is assumed that the deserialized object should be the instance from MappingConfiguration.Current again.
      // Therefore only the information needed in IObjectReference.GetRealObject is deserialized here.
      _mappingClassID = info.GetString ("MappingClassID");
      _mappingPropertyName = info.GetString ("MappingPropertyName");
    }
    else
    {
      _relationDefinition = (RelationDefinition) info.GetValue ("RelationDefinition", typeof (RelationDefinition));
      _classDefinition = (ClassDefinition) info.GetValue ("ClassDefinition", typeof (ClassDefinition));
      _propertyDefinition = (PropertyDefinition) info.GetValue ("PropertyDefinition", typeof (PropertyDefinition));
      _isMandatory = info.GetBoolean ("IsMandatory");
    }
  }

  // methods and properties

  #region INullObject Members
  
  public bool IsNull
  {
    get { return false; }
  }
  
  #endregion

  #region IRelationEndPointDefinition Members

  public bool CorrespondsTo (string classID, string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

    return (_classDefinition.ID == classID && PropertyName == propertyName);
  }
  
  public void SetRelationDefinition (RelationDefinition relationDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

    _relationDefinition = relationDefinition;
  }

  public RelationDefinition RelationDefinition
  {
    get { return _relationDefinition; }
  }

  public ClassDefinition ClassDefinition
  {
    get { return _classDefinition; }
  }

  public string PropertyName
  {
    get { return _propertyDefinition.PropertyName; }
  }

  public bool IsMandatory
  {
    get { return _isMandatory; }
  }

  public CardinalityType Cardinality
  {
    get { return CardinalityType.One; }
  }

  public Type PropertyType
  {
    get { return _propertyDefinition.PropertyType; }
  }

  public bool IsPropertyTypeResolved
  {
    get { return PropertyType != null; }
  }

  public string PropertyTypeName
  {
    get 
    {
      if (IsPropertyTypeResolved)
        return typeof(ObjectID).AssemblyQualifiedName; 
      else
        return TypeUtility.GetPartialAssemblyQualifiedName (typeof (ObjectID));
    }
  }

  public bool IsVirtual
  {
    get { return false; }
  }
  
  #endregion

  public PropertyDefinition PropertyDefinition
  {
    get { return _propertyDefinition; }
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }

  #region ISerializable Members

  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    GetObjectData (info, context);
  }

  protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    bool isPartOfMappingConfiguration = MappingConfiguration.Current.Contains (this);
    info.AddValue ("IsPartOfMappingConfiguration", isPartOfMappingConfiguration);

    if (isPartOfMappingConfiguration)
    {
      // Note: If this object is part of MappingConfiguration.Current during the serialization process,
      // it is assumed that the deserialized object should be the instance from MappingConfiguration.Current again.
      // Therefore only the information needed in IObjectReference.GetRealObject is serialized here.
      info.AddValue ("MappingClassID", _classDefinition.ID);
      info.AddValue ("MappingPropertyName", _propertyDefinition.PropertyName);
    }
    else
    {
      info.AddValue ("RelationDefinition", _relationDefinition);
      info.AddValue ("ClassDefinition", _classDefinition);
      info.AddValue ("PropertyDefinition", _propertyDefinition);
      info.AddValue ("IsMandatory", _isMandatory);
    }
  }

  #endregion

  #region IObjectReference Members

  object IObjectReference.GetRealObject (StreamingContext context)
  {
    // Note: A EndPointDefinition knows its ClassDefinition and a ClassDefinition implicitly knows 
    // its RelationEndPointDefinitions via its RelationDefinitions. For bi-directional relationships 
    // with two classes implementing IObjectReference.GetRealObject the order of calling this method is unpredictable.
    // Therefore the members _classDefinition and _propertyDefinition cannot be used here, because they could point to the wrong instances. 
    if (_mappingClassID != null && _mappingPropertyName != null)
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (_mappingClassID).GetMandatoryRelationEndPointDefinition (_mappingPropertyName);
    else
      return this;
  }

  #endregion
}
}
