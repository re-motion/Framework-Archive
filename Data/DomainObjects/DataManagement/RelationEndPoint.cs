using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

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

  private IRelationEndPointDefinition _definition;
  private RelationEndPointID _id;

  // construction and disposing

  protected RelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition) 
      : this (domainObject.ID, definition)
  {
  }

  protected RelationEndPoint (DataContainer dataContainer, IRelationEndPointDefinition definition) 
      : this (dataContainer.ID, definition.PropertyName) 
  {
  }


  protected RelationEndPoint (DomainObject domainObject, string propertyName) 
      : this (domainObject.ID, propertyName)
  {
  }

  protected RelationEndPoint (DataContainer dataContainer, string propertyName)
      : this (dataContainer.ID, propertyName)
  {
  }

  protected RelationEndPoint (ObjectID objectID, IRelationEndPointDefinition definition) 
      : this (objectID, definition.PropertyName) 
  {
  }

  protected RelationEndPoint (ObjectID objectID, string propertyName) : this (new RelationEndPointID (objectID, propertyName))
  {
  }
  
  protected RelationEndPoint (RelationEndPointID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

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

  // methods and properties

  public bool BeginRelationChange (RelationEndPoint oldEndPoint)
  {
    return BeginRelationChange (oldEndPoint, RelationEndPoint.CreateNullRelationEndPoint (oldEndPoint.Definition));    
  }

  public virtual bool BeginRelationChange (RelationEndPoint oldEndPoint, RelationEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    return DomainObject.BeginRelationChange (
        PropertyName, oldEndPoint.DomainObject, newEndPoint.DomainObject);
  }

  public virtual void EndRelationChange ()
  {
    DomainObject.EndRelationChange (PropertyName);
  }

  public virtual DomainObject DomainObject
  {
    get 
    {
      // TODO: This property should return a deleted domainObject too!
      return ClientTransaction.Current.GetObject (ObjectID); 
    }
  }

  public virtual DataContainer DataContainer
  {
    get { return DomainObject.DataContainer; }
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
