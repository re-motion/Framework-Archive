using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class AnonymousEndPoint : IEndPoint
{
  // types

  // static members and constants

  // member fields

  private RelationDefinition _relationDefinition;
  private NullRelationEndPointDefinition _definition;
  private ClientTransaction _clientTransaction;
  private ObjectID _objectID;

  // construction and disposing

  public AnonymousEndPoint (ClientTransaction clientTransaction, DomainObject domainObject, RelationDefinition relationDefinition)
      : this (clientTransaction, domainObject.ID, relationDefinition)
  {
  }

  public AnonymousEndPoint (ClientTransaction clientTransaction, DataContainer dataContainer, RelationDefinition relationDefinition) 
      : this (clientTransaction, dataContainer.ID, relationDefinition)
  {
  }

  public AnonymousEndPoint (ClientTransaction clientTransaction, ObjectID objectID, RelationDefinition relationDefinition)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    ArgumentUtility.CheckNotNull ("objectID", objectID);
    ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

    _definition = GetNullRelationEndPointDefinition (relationDefinition);
    _relationDefinition = relationDefinition;
    _clientTransaction = clientTransaction;
    _objectID = objectID;
  }

  private NullRelationEndPointDefinition GetNullRelationEndPointDefinition (RelationDefinition relationDefinition)
  {
    foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
    {
      if (endPointDefinition.IsNull)
        return (NullRelationEndPointDefinition) endPointDefinition;
    }

    throw new ArgumentException ("The provided relation definition must contain a NullRelationEndPointDefinition.", "relationDefinition");
  }

  // methods and properties

  #region IEndPoint Members

  public ClientTransaction ClientTransaction
  {
    get { return _clientTransaction; }
  }

  public DomainObject GetDomainObject ()
  {
    return _clientTransaction.GetObject (ObjectID, true); 
  }

  public DataContainer GetDataContainer ()
  {
    DomainObject domainObject = GetDomainObject ();
    return domainObject.DataContainer;
  }

  public ObjectID ObjectID
  {
    get { return _objectID; }
  }

  public ClassDefinition ClassDefinition
  {
    get { return _definition.ClassDefinition; }
  }

  public RelationDefinition RelationDefinition
  {
    get { return _relationDefinition; }
  }

  public IRelationEndPointDefinition Definition
  {
    get { return _definition; }
  }

  #endregion

  #region INullableObject Members

  public bool IsNull
  {
    get { return false; }
  }

  #endregion
}
}
