using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
public class RelationEndPointDefinition : IRelationEndPointDefinition
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _classDefinition;
  private PropertyDefinition _propertyDefinition;
  private bool _isMandatory;

  // construction and disposing

  public RelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory) 
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    
    PropertyDefinition propertyDefinition = classDefinition[propertyName];
    if (propertyDefinition == null)
    {
      throw CreateMappingException (
          "Relation definition error for end point: Class '{0}' has no property '{1}'.",
          classDefinition.ID, propertyName);
    }

    if (propertyDefinition.PropertyType != typeof (ObjectID))
    {
      throw CreateMappingException (
          "Relation definition error: Property '{0}' of class '{1}' is of type '{2}',"
          + " but non-virtual properties must be of type 'Rubicon.Data.DomainObjects.ObjectID'.",
          propertyDefinition.PropertyName, classDefinition.ID, propertyDefinition.PropertyType);
    }

    _classDefinition = classDefinition;    
    _isMandatory = isMandatory;
    _propertyDefinition = propertyDefinition;
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
}
}
