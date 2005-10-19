using System;
using System.Runtime.Serialization;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
// Note: No properties and methods of this class are inheritance-aware!
[Serializable]
public class RelationDefinition : ISerializable, IObjectReference
{
  // types

  // static members and constants

  // member fields

  private string _id;
  private IRelationEndPointDefinition[] _endPointDefinitions = new IRelationEndPointDefinition [2];

  // Note: _isPartOfMappingConfiguration is used only during the deserialization process. 
  // It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
  private bool _isPartOfMappingConfiguration;
  
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

    try
    {
      endPointDefinition1.SetRelationDefinition (this);
      endPointDefinition2.SetRelationDefinition (this);
    }
    catch (Exception)
    {
      endPointDefinition1.SetRelationDefinition (null);
      endPointDefinition2.SetRelationDefinition (null);
      throw;
    }

    _id = id;
    _endPointDefinitions[0] = endPointDefinition1;
    _endPointDefinitions[1] = endPointDefinition2;
  }

  protected RelationDefinition (SerializationInfo info, StreamingContext context)
  {
    _id = info.GetString ("ID");
    _isPartOfMappingConfiguration = info.GetBoolean ("IsPartOfMappingConfiguration");

    if (!_isPartOfMappingConfiguration)
      _endPointDefinitions = (IRelationEndPointDefinition[]) info.GetValue ("EndPointDefinitions", typeof (IRelationEndPointDefinition[]));
  }

  private void CheckEndPointDefinitions (
      string id, 
      IRelationEndPointDefinition endPointDefinition1, 
      IRelationEndPointDefinition endPointDefinition2)
  {
    if (endPointDefinition1.IsNull && endPointDefinition2.IsNull)
      throw CreateMappingException ("Relation '{0}' cannot have two null end points.", id);

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

    foreach (IRelationEndPointDefinition endPointDefinition in _endPointDefinitions)
    {
      if (endPointDefinition.CorrespondsTo (classID, propertyName))
        return true;
    }

    return false;
  }

  public bool Contains (IRelationEndPointDefinition endPointDefinition)
  {
    ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);

    if (object.ReferenceEquals (endPointDefinition, _endPointDefinitions[0]))
      return true;

    return object.ReferenceEquals (endPointDefinition, _endPointDefinitions[1]);
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
    info.AddValue ("ID", _id);

    bool isPartOfMappingConfiguration = MappingConfiguration.Current.Contains (this);
    info.AddValue ("IsPartOfMappingConfiguration", isPartOfMappingConfiguration);

    if (!isPartOfMappingConfiguration)
      info.AddValue ("EndPointDefinitions", _endPointDefinitions);
  }

  #endregion

  #region IObjectReference Members

  object IObjectReference.GetRealObject (StreamingContext context)
  {
    if (_isPartOfMappingConfiguration)
      return MappingConfiguration.Current.RelationDefinitions.GetMandatory (_id);
    else
      return this;
  }

  #endregion
}
}
