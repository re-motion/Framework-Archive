using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
public class NullRelationEndPointDefinition : IRelationEndPointDefinition
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _classDefinition;

  // construction and disposing

  public NullRelationEndPointDefinition (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    _classDefinition = classDefinition;
  }

  // methods and properties

  #region INullableObject Members
  
  public bool IsNull
  {
    get { return true; }
  }

  #endregion

  #region IRelationEndPointDefinition Members

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

  #endregion
}
}
