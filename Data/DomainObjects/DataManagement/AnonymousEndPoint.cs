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
  private AnonymousRelationEndPointDefinition _definition;
  private ClientTransaction _clientTransaction;
  private DomainObject _domainObject;

  // construction and disposing

  // This end point stores a DomainObject rather than an ObjectID in order to support AnonymousEndPoints storing discarded objects (which can
  // happen with unidirectional relations.)
  public AnonymousEndPoint (ClientTransaction clientTransaction, DomainObject domainObject, RelationDefinition relationDefinition)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    ArgumentUtility.CheckNotNull ("domainObjace", domainObject);
    ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

    _definition = GetAnonymousRelationEndPointDefinition (relationDefinition);
    _relationDefinition = relationDefinition;
    _clientTransaction = clientTransaction;
    _domainObject = domainObject;
  }

  protected AnonymousEndPoint (RelationDefinition relationDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);
    
    _definition = GetAnonymousRelationEndPointDefinition (relationDefinition);
    _relationDefinition = relationDefinition;
  }

  private AnonymousRelationEndPointDefinition GetAnonymousRelationEndPointDefinition (RelationDefinition relationDefinition)
  {
    foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
    {
      if (endPointDefinition.IsNull)
        return (AnonymousRelationEndPointDefinition) endPointDefinition;
    }

    throw new ArgumentException ("The provided relation definition must contain a AnonymousRelationEndPointDefinition.", "relationDefinition");
  }

  // methods and properties

  #region IEndPoint Members

  public virtual ClientTransaction ClientTransaction
  {
    get { return _clientTransaction; }
  }

  public virtual DomainObject GetDomainObject ()
  {
    return _domainObject;
  }

  public virtual DataContainer GetDataContainer ()
  {
    DomainObject domainObject = GetDomainObject ();
    return domainObject.GetDataContainer();
  }

  public virtual ObjectID ObjectID
  {
    get { return _domainObject.ID; }
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

  #region INullObject Members

  public virtual bool IsNull
  {
    get { return false; }
  }

  #endregion
}
}
