using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public abstract class RelationEndPoint : INullable
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

  public bool BeginRelationChange (RelationEndPoint oldEndPoint)
  {
    return BeginRelationChange (oldEndPoint, RelationEndPoint.CreateNullRelationEndPoint (oldEndPoint.Definition));    
  }

  public virtual bool BeginRelationChange (RelationEndPoint oldEndPoint, RelationEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    DomainObject domainObject = GetDomainObject ();
    return domainObject.BeginRelationChange (
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
    return domainObject.DataContainer;
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

  public ClassDefinition ClassDefinition
  {
    get { return _definition.ClassDefinition; }
  }

  public IRelationEndPointDefinition OppositeEndPointDefinition
  {
    get { return ClassDefinition.GetOppositeEndPointDefinition (PropertyName); }
  }

  public RelationDefinition RelationDefinition
  {
    get { return ClassDefinition.GetRelationDefinition (PropertyName); }
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
      string formatString, 
      params object[] args)
  {
    return new MandatoryRelationNotSetException (string.Format (formatString, args));
  }

  #region INullable Members

  public virtual bool IsNull
  {
    get { return false; }
  }

  #endregion
}
}
