using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
public class VirtualRelationEndPointDefinition : IRelationEndPointDefinition
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _classDefinition;
  private bool _isMandatory;
  private CardinalityType _cardinality;
  private string _propertyName;
  private Type _propertyType;
  private string _sortExpression;

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
    ArgumentUtility.CheckValidEnumValue (cardinality, "cardinality");
    ArgumentUtility.CheckNotNull ("propertyType", propertyType);

    CheckParameters (classDefinition, propertyName, isMandatory, cardinality, propertyType, sortExpression);

    _classDefinition = classDefinition;    
    _cardinality = cardinality;
    _isMandatory = isMandatory;
    _propertyName = propertyName;
    _propertyType = propertyType;
    _sortExpression = sortExpression;
  }

  private void CheckParameters (
      ClassDefinition classDefinition, 
      string propertyName, 
      bool isMandatory,    
      CardinalityType cardinality,
      Type propertyType,
      string sortExpression)
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
}
}
