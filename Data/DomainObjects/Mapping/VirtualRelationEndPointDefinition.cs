using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Configuration.Mapping
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

  // construction and disposing

  public VirtualRelationEndPointDefinition (
      ClassDefinition classDefinition, 
      string propertyName, 
      bool isMandatory,    
      CardinalityType cardinality,
      Type propertyType) 
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNull ("propertyType", propertyType);

    if (!Enum.IsDefined (typeof (CardinalityType), cardinality))
      throw new ArgumentException (string.Format ("Invalid cardinality '{0}' provided.", cardinality), "cardinality");

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

    _classDefinition = classDefinition;    
    _cardinality = cardinality;
    _isMandatory = isMandatory;
    _propertyName = propertyName;
    _propertyType = propertyType;
  }

  // methods and properties

  public bool CorrespondsTo (string classID, string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

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

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }
}
}
