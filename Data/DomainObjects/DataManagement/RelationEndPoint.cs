using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
[Serializable]
public abstract class RelationEndPoint : IEndPoint
{
  // types

  // static members and constants

  public static RelationEndPoint CreateNullRelationEndPoint (IRelationEndPointDefinition definition)
  {
    if (definition.Cardinality == CardinalityType.One)
      return new NullObjectEndPoint (definition);
    else
      return new NullCollectionEndPoint (definition);
  }

  // member fields

  private ClientTransaction _clientTransaction;
  private IRelationEndPointDefinition _definition;
  private RelationEndPointID _id;

  // construction and disposing

  protected RelationEndPoint (
      ClientTransaction clientTransaction,
      DomainObject domainObject, 
      IRelationEndPointDefinition definition) 
      : this (clientTransaction, domainObject.ID, definition)
  {
  }

  protected RelationEndPoint (
      ClientTransaction clientTransaction,
      DataContainer dataContainer, 
      IRelationEndPointDefinition definition) 
      : this (clientTransaction, dataContainer.ID, definition.PropertyName) 
  {
  }

  protected RelationEndPoint (
      ClientTransaction clientTransaction,
      DomainObject domainObject, 
      string propertyName) 
      : this (clientTransaction, domainObject.ID, propertyName)
  {
  }

  protected RelationEndPoint (
      ClientTransaction clientTransaction,
      DataContainer dataContainer, 
      string propertyName)
      : this (clientTransaction, dataContainer.ID, propertyName)
  {
  }

  protected RelationEndPoint (
      ClientTransaction clientTransaction,
      ObjectID objectID, 
      IRelationEndPointDefinition definition) 
      : this (clientTransaction, objectID, definition.PropertyName) 
  {
  }

  protected RelationEndPoint (
      ClientTransaction clientTransaction,
      ObjectID objectID, 
      string propertyName) 
      : this (clientTransaction, new RelationEndPointID (objectID, propertyName))
  {
  }
  
  protected RelationEndPoint (ClientTransaction clientTransaction, RelationEndPointID id)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    ArgumentUtility.CheckNotNull ("id", id);

    _clientTransaction = clientTransaction;
    _id = id;
    _definition = _id.Definition;
  }

  protected RelationEndPoint (IRelationEndPointDefinition definition)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);
    _definition = definition;
  }

  // abstract methods and properties

  public abstract bool HasChanged { get; } 
  public abstract void Commit ();
  public abstract void Rollback ();
  public abstract void CheckMandatory ();
  public abstract void PerformRelationChange ();
  public abstract void PerformDelete ();

  // methods and properties

  public void NotifyClientTransactionOfBeginRelationChange (IEndPoint oldEndPoint)
  {
    NotifyClientTransactionOfBeginRelationChange (oldEndPoint, RelationEndPoint.CreateNullRelationEndPoint (oldEndPoint.Definition));
  }

  public virtual void NotifyClientTransactionOfBeginRelationChange (IEndPoint oldEndPoint, IEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    _clientTransaction.RelationChanging (
        GetDomainObject (), 
        _definition.PropertyName, 
        oldEndPoint.GetDomainObject (), 
        newEndPoint.GetDomainObject ());
  }

  public virtual void NotifyClientTransactionOfEndRelationChange ()
  {
    _clientTransaction.RelationChanged (GetDomainObject (), _definition.PropertyName);
  }

  public void BeginRelationChange (IEndPoint oldEndPoint)
  {
    BeginRelationChange (oldEndPoint, RelationEndPoint.CreateNullRelationEndPoint (oldEndPoint.Definition));    
  }

  public virtual void BeginRelationChange (IEndPoint oldEndPoint, IEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    DomainObject domainObject = GetDomainObject ();

    domainObject.BeginRelationChange (
        PropertyName, oldEndPoint.GetDomainObject (), newEndPoint.GetDomainObject ());
  }

  public virtual void EndRelationChange ()
  {
    DomainObject domainObject = GetDomainObject (); 
    domainObject.EndRelationChange (PropertyName);
  }

  public virtual DomainObject GetDomainObject ()
  {
    return _clientTransaction.GetObject (ObjectID, true); 
  }

  public virtual DataContainer GetDataContainer ()
  {
    DomainObject domainObject = GetDomainObject ();
    return domainObject.GetDataContainer();
  }

  public virtual ObjectID ObjectID
  {
    get { return _id.ObjectID; }
  }

  public IRelationEndPointDefinition Definition 
  {
    get { return _definition; }
  }

  public string PropertyName
  {
    get { return _definition.PropertyName; }
  }

  public IRelationEndPointDefinition OppositeEndPointDefinition
  {
    get { return _definition.ClassDefinition.GetMandatoryOppositeEndPointDefinition (PropertyName); }
  }

  public RelationDefinition RelationDefinition
  {
    get { return _definition.ClassDefinition.GetMandatoryRelationDefinition (PropertyName); }
  }

  public bool IsVirtual
  {
    get { return _definition.IsVirtual; }
  }

  public virtual RelationEndPointID ID
  {
    get { return _id; }
  }

  public ClientTransaction ClientTransaction
  {
    get { return _clientTransaction; }
  }

  protected MandatoryRelationNotSetException CreateMandatoryRelationNotSetException (
      DomainObject domainObject,
      string propertyName,
      string formatString, 
      params object[] args)
  {
    return new MandatoryRelationNotSetException (domainObject, propertyName, string.Format (formatString, args));
  }

  #region INullObject Members

  public virtual bool IsNull
  {
    get { return false; }
  }

  #endregion
}
}
