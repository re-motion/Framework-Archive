using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
// Note: No properties and methods of this class are inheritance-aware!
public class RelationDefinition
{
  // types

  // static members and constants

  // member fields

  private string _id;
  private IRelationEndPointDefinition[] _endPointDefinitions = new IRelationEndPointDefinition [2];

  // construction and disposing

  public RelationDefinition (
      string id, 
      IRelationEndPointDefinition endPointDefinition1, 
      IRelationEndPointDefinition endPointDefinition2)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("id", id);
    ArgumentUtility.CheckNotNull ("endPointDefinition1", endPointDefinition1);
    ArgumentUtility.CheckNotNull ("endPointDefinition2", endPointDefinition2);

    CheckEndPointDefinitions (id, endPointDefinition1, endPointDefinition2);
    
    _id = id;
    _endPointDefinitions[0] = endPointDefinition1;
    _endPointDefinitions[1] = endPointDefinition2;
  }

  private void CheckEndPointDefinitions (
      string id, 
      IRelationEndPointDefinition endPointDefinition1, 
      IRelationEndPointDefinition endPointDefinition2)
  {
    if (endPointDefinition1.IsVirtual && endPointDefinition2.IsVirtual)
      throw CreateMappingException ("Relation '{0}' cannot have two virtual end points.", id);

    if (!endPointDefinition1.IsVirtual && !endPointDefinition2.IsVirtual)
      throw CreateMappingException ("Relation '{0}' cannot have two non-virtual end points.", id);
  }

  // methods and properties

  public IRelationEndPointDefinition GetMandatoryOppositeRelationEndPointDefinition (IRelationEndPointDefinition endPointDefinition)
  {
    ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);

    IRelationEndPointDefinition oppositeEndPointDefinition = GetOppositeEndPointDefinition (endPointDefinition);
    if (oppositeEndPointDefinition == null)
    {
      throw CreateMappingException (
          "Relation '{0}' has no association with class '{1}' and property '{2}'.",
          ID, endPointDefinition.ClassDefinition.ID, endPointDefinition.PropertyName);
    }

    return oppositeEndPointDefinition;
  }

  public string ID
  {
    get { return _id; }
  }

  public IRelationEndPointDefinition[] EndPointDefinitions
  {
    get { return _endPointDefinitions; }
  }
  
  public IRelationEndPointDefinition GetEndPointDefinition (string classID, string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

    if (_endPointDefinitions[0].CorrespondsTo (classID, propertyName))
      return _endPointDefinitions[0];

    if (_endPointDefinitions[1].CorrespondsTo (classID, propertyName)) 
      return _endPointDefinitions[1];

    return null;
  }

  public IRelationEndPointDefinition GetOppositeEndPointDefinition (IRelationEndPointDefinition endPointDefinition)
  {
    ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);
    return GetOppositeEndPointDefinition (endPointDefinition.ClassDefinition.ID, endPointDefinition.PropertyName);
  }

  public IRelationEndPointDefinition GetOppositeEndPointDefinition (string classID, string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    if (_endPointDefinitions[0].CorrespondsTo (classID, propertyName)) 
      return _endPointDefinitions[1];

    if (_endPointDefinitions[1].CorrespondsTo (classID, propertyName)) 
      return _endPointDefinitions[0];
    
    return null;
  }

  public ClassDefinition GetOppositeClassDefinition (IRelationEndPointDefinition endPointDefinition)
  {
    ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);
    return GetOppositeClassDefinition (endPointDefinition.ClassDefinition.ID, endPointDefinition.PropertyName);
  }

  public ClassDefinition GetOppositeClassDefinition (string classID, string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    IRelationEndPointDefinition oppositeEndPointDefinition = GetOppositeEndPointDefinition (classID, propertyName);
    if (oppositeEndPointDefinition == null)
      return null;

    return oppositeEndPointDefinition.ClassDefinition;
  }

  public bool IsEndPoint (IRelationEndPointDefinition endPointDefinition)
  {
    ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);
    return IsEndPoint (endPointDefinition.ClassDefinition.ID, endPointDefinition.PropertyName);
  }

  public bool IsEndPoint (string classID, string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    foreach (IRelationEndPointDefinition endPointDefinition in _endPointDefinitions)
    {
      if (endPointDefinition.CorrespondsTo (classID, propertyName))
        return true;
    }

    return false;
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }
}
}
