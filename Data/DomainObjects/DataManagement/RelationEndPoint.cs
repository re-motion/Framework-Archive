using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPoint : INullable
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

  public RelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition) 
      : this (domainObject.ID, definition)
  {
  }

  public RelationEndPoint (DataContainer dataContainer, IRelationEndPointDefinition definition) 
      : this (dataContainer.ID, definition.PropertyName) 
  {
  }


  public RelationEndPoint (DomainObject domainObject, string propertyName) 
      : this (domainObject.ID, propertyName)
  {
  }

  public RelationEndPoint (DataContainer dataContainer, string propertyName)
      : this (dataContainer.ID, propertyName)
  {
  }

  public RelationEndPoint (ObjectID objectID, IRelationEndPointDefinition definition) 
      : this (objectID, definition.PropertyName) 
  {
  }

  public RelationEndPoint (ObjectID objectID, string propertyName) : this (new RelationEndPointID (objectID, propertyName))
  {
  }
  
  public RelationEndPoint (RelationEndPointID id)
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

  public IRelationEndPointDefinition Definition 
  {
    get { return _definition; }
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

  public RelationDefinition RelationDefinition
  {
    get { return _definition.ClassDefinition.GetRelationDefinition (PropertyName); }
  }

  public IRelationEndPointDefinition OppositeEndPointDefinition
  {
    get { return _definition.ClassDefinition.GetOppositeEndPointDefinition (PropertyName); }
  }

  public string PropertyName
  {
    get { return _definition.PropertyName; }
  }

  public bool IsVirtual
  {
    get { return _definition.IsVirtual; }
  }

  public virtual RelationEndPointID ID
  {
    get { return _id; }
  }


  public virtual bool HasChanged 
  {
    get 
    { 
      // TODO: Make this property abstract
      throw new InvalidOperationException ("HasChanged must be overridden in a derived class.");
    }
  }

  public virtual void Commit ()
  {
    // TODO: Make this method abstract
    throw new InvalidOperationException ("Commit must be overridden in a derived class.");
  }

  public virtual void Rollback ()
  {
    // TODO: Make this method abstract
    throw new InvalidOperationException ("Rollback must be overridden in a derived class.");
  }

  public virtual void CheckMandatory ()
  {
    // TODO: Make this method abstract
    throw new InvalidOperationException ("CheckMandatory must be overridden in a derived class.");
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
