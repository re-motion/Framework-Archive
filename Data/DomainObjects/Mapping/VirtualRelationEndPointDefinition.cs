using System;
using System.Runtime.Serialization;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
[Serializable]
public class VirtualRelationEndPointDefinition : IRelationEndPointDefinition, ISerializable, IObjectReference
{
  // types

  // static members and constants

  // member fields

  private RelationDefinition _relationDefinition;
  private ClassDefinition _classDefinition;
  private bool _isMandatory;
  private CardinalityType _cardinality;
  private string _propertyName;
  private Type _propertyType;
  private string _propertyTypeName;
  private string _sortExpression;

  // Note: _mappingClassID is used only during the deserialization process. 
  // It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
  private string _mappingClassID;

  // construction and disposing

  public VirtualRelationEndPointDefinition (
      ClassDefinition classDefinition, 
      string propertyName, 
      bool isMandatory,    
      CardinalityType cardinality,
      Type propertyType) 
      : this (classDefinition, propertyName, isMandatory, cardinality, propertyType, null)
  {
  }

  public VirtualRelationEndPointDefinition (
      ClassDefinition classDefinition, 
      string propertyName, 
      bool isMandatory,    
      CardinalityType cardinality,
      Type propertyType,
      string sortExpression) 
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckValidEnumValue ("cardinality", cardinality);
    ArgumentUtility.CheckNotNull ("propertyType", propertyType);

    Initialize (classDefinition, propertyName, isMandatory, cardinality, propertyType, null, sortExpression);
  }

  public VirtualRelationEndPointDefinition (
      ClassDefinition classDefinition, 
      string propertyName, 
      bool isMandatory,    
      CardinalityType cardinality,
      string propertyTypeName,
      string sortExpression) 
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckValidEnumValue ("cardinality", cardinality);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyTypeName", propertyTypeName);

    Initialize (classDefinition, propertyName, isMandatory, cardinality, null, propertyTypeName, sortExpression);
  }

  private void Initialize (
      ClassDefinition classDefinition, 
      string propertyName, 
      bool isMandatory,    
      CardinalityType cardinality,
      Type propertyType,
      string propertyTypeName,
      string sortExpression)
  {
    if (classDefinition.IsClassTypeResolved && propertyTypeName != null)
      propertyType = Type.GetType (propertyTypeName, true);

    if (propertyType != null)
    {
      CheckPropertyType (classDefinition, propertyName, cardinality, propertyType);
      propertyTypeName = propertyType.AssemblyQualifiedName;
    }

    CheckSortExpression (classDefinition, propertyName, cardinality, sortExpression);

    _classDefinition = classDefinition;    
    _cardinality = cardinality;
    _isMandatory = isMandatory;
    _propertyName = propertyName;
    _propertyType = propertyType;
    _propertyTypeName = propertyTypeName;
    _sortExpression = sortExpression;
  }

  protected VirtualRelationEndPointDefinition (SerializationInfo info, StreamingContext context)
  {
    _propertyName = info.GetString ("PropertyName");
    bool ispartOfMappingConfiguration = info.GetBoolean ("IsPartOfMappingConfiguration");

    if (ispartOfMappingConfiguration)
    {
      // Note: If this object was part of MappingConfiguration.Current during the serialization process,
      // it is assumed that the deserialized object should be the instance from MappingConfiguration.Current again.
      // Therefore only the information needed in IObjectReference.GetRealObject is deserialized here.
      _mappingClassID = info.GetString ("MappingClassID");
    }
    else
    {
      _relationDefinition = (RelationDefinition) info.GetValue ("RelationDefinition", typeof (RelationDefinition));
      _classDefinition = (ClassDefinition) info.GetValue ("ClassDefinition", typeof (ClassDefinition));
      _isMandatory = info.GetBoolean ("IsMandatory");
      _cardinality = (CardinalityType) info.GetValue ("Cardinality", typeof (CardinalityType));
      _propertyType = (Type) info.GetValue ("PropertyType", typeof (Type));
      _sortExpression = info.GetString ("SortExpression");
    }
  }

  private void CheckPropertyType (
      ClassDefinition classDefinition, 
      string propertyName, 
      CardinalityType cardinality,
      Type propertyType)
  {
    if (propertyType != typeof (DomainObjectCollection)
        && !propertyType.IsSubclassOf (typeof (DomainObjectCollection))
        && !propertyType.IsSubclassOf (typeof (DomainObject)))
    {
      throw CreateMappingException ("Relation definition error: Virtual property '{0}' of class '{1}' is of type"
          + "'{2}', but must be derived from 'Rubicon.Data.DomainObjects.DomainObject' or "
          + " 'Rubicon.Data.DomainObjects.DomainObjectCollection' or must be"
          + " 'Rubicon.Data.DomainObjects.DomainObjectCollection'.",
          propertyName, classDefinition.ID, propertyType);
    }

    if (cardinality == CardinalityType.One && !propertyType.IsSubclassOf (typeof (DomainObject)))
    {
      throw CreateMappingException ("The property type of a virtual end point of a one-to-one relation"
          + " must be derived from 'Rubicon.Data.DomainObjects.DomainObject'.");
    }

    if (cardinality == CardinalityType.Many 
        && propertyType != typeof (DomainObjectCollection)
        && !propertyType.IsSubclassOf (typeof (DomainObjectCollection)))
    {
      throw CreateMappingException ("The property type of a virtual end point of a one-to-many relation"
          + " must be or be derived from 'Rubicon.Data.DomainObjects.DomainObjectCollection'.");
    }
  }

  private void CheckSortExpression (
      ClassDefinition classDefinition, 
      string propertyName, 
      CardinalityType cardinality,
      string sortExpression)
  {
    if (cardinality == CardinalityType.One && sortExpression != null)
    {
      throw CreateMappingException (
          "Property '{0}' of class '{1}' must not specify a SortExpression, because cardinality is equal to 'one'.",
          propertyName, classDefinition.ID);
    }

  }

  // methods and properties

  #region INullableObject Members
  
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

  public bool IsMandatory
  {
    get { return _isMandatory; }
  }

  public CardinalityType Cardinality
  {
    get { return _cardinality; }
  }

  public string PropertyName 
  {
    get { return _propertyName; } 
  }

  public Type PropertyType 
  { 
    get { return _propertyType; } 
  }

  public bool IsPropertyTypeResolved
  {
    get { return _propertyType != null; }
  }

  public string PropertyTypeName
  {
    get { return _propertyTypeName; }
  }

  public bool IsVirtual
  {
    get { return true; }
  }
  
  #endregion

  public string SortExpression 
  {
    get { return _sortExpression; }
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
    info.AddValue ("PropertyName", _propertyName);

    bool isPartOfMappingConfiguration = MappingConfiguration.Current.Contains (this);
    info.AddValue ("IsPartOfMappingConfiguration", isPartOfMappingConfiguration);

    if (isPartOfMappingConfiguration)
    {
      // Note: If this object is part of MappingConfiguration.Current during the serialization process,
      // it is assumed that the deserialized object should be the instance from MappingConfiguration.Current again.
      // Therefore only the information needed in IObjectReference.GetRealObject is serialized here.
      info.AddValue ("MappingClassID", _classDefinition.ID);
    }
    else
    {
      info.AddValue ("RelationDefinition", _relationDefinition);
      info.AddValue ("ClassDefinition", _classDefinition);
      info.AddValue ("IsMandatory", _isMandatory);
      info.AddValue ("Cardinality", _cardinality);
      info.AddValue ("PropertyType", _propertyType);
      info.AddValue ("SortExpression", _sortExpression);
    }
  }

  #endregion

  #region IObjectReference Members

  object IObjectReference.GetRealObject (StreamingContext context)
  {
    // Note: A EndPointDefinition knows its ClassDefinition and a ClassDefinition implicitly knows 
    // its RelationEndPointDefinitions via its RelationDefinitions. For bi-directional relationships 
    // with two classes implementing IObjectReference.GetRealObject the order of calling this method is unpredictable.
    // Therefore the members _classDefinition and _relationDefinition cannot be used here, because they could point to the wrong instance. 
    if (_mappingClassID != null)
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (_mappingClassID).GetMandatoryRelationEndPointDefinition (_propertyName);
    else
      return this;
  }

  #endregion
}
}
