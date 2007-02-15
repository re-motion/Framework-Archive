using System;
using System.Runtime.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
[Serializable]
public class NullRelationEndPointDefinition : IRelationEndPointDefinition, ISerializable, IObjectReference
{
  // types

  // static members and constants

  // member fields

  private RelationDefinition _relationDefinition;
  private ClassDefinition _classDefinition;

  // Note: _mappingRelationID is used only during the deserialization process. 
  // It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
  private string _mappingRelationID;

  // construction and disposing

  public NullRelationEndPointDefinition (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    _classDefinition = classDefinition;
  }

  protected NullRelationEndPointDefinition (SerializationInfo info, StreamingContext context)
  {
    bool ispartOfMappingConfiguration = info.GetBoolean ("IsPartOfMappingConfiguration");

    if (ispartOfMappingConfiguration)
    {
      // Note: If this object was part of MappingConfiguration.Current during the serialization process,
      // it is assumed that the deserialized object should be the instance from MappingConfiguration.Current again.
      // Therefore only the information needed in IObjectReference.GetRealObject is deserialized here.
      _mappingRelationID = info.GetString ("MappingRelationID");
    }
    else
    {
      _relationDefinition = (RelationDefinition) info.GetValue ("RelationDefinition", typeof (RelationDefinition));
      _classDefinition = (ClassDefinition) info.GetValue ("ClassDefinition", typeof (ClassDefinition));
    }
  }

  // methods and properties

  #region INullableObject Members
  
  public bool IsNull
  {
    get { return true; }
  }

  #endregion

  #region IRelationEndPointDefinition Members

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
    get { return null; }
  }

  public Type PropertyType
  {
    get { return null; }
  }

  public bool IsPropertyTypeResolved
  {
    get { return _classDefinition.IsClassTypeResolved; }
  }

  public string PropertyTypeName
  {
    get { return null; }
  }

  public bool IsMandatory
  {
    get { return false;}
  }

  public CardinalityType Cardinality
  {
    get { return CardinalityType.Many; }
  }

  public bool IsVirtual
  {
    get { return true; }
  }

  public bool CorrespondsTo (string classID, string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

    return (_classDefinition.ID == classID && propertyName == null);
  }
  
  public void SetRelationDefinition (RelationDefinition relationDefinition)
  {
    _relationDefinition = relationDefinition;
  }

  #endregion

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
      info.AddValue ("MappingRelationID", _relationDefinition.ID);
    }
    else
    {
      info.AddValue ("RelationDefinition", _relationDefinition);
      info.AddValue ("ClassDefinition", _classDefinition);
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
    if (_mappingRelationID != null)
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions.GetMandatory (_mappingRelationID);
      if (relationDefinition.EndPointDefinitions[0].IsNull)
        return relationDefinition.EndPointDefinitions[0];
      else
        return relationDefinition.EndPointDefinitions[1];
    }
    else
    {
      return this;
    }
  }

  #endregion
}
}
