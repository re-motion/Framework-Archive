using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPoint : INullable
{
  // types

  // static members and constants

  // member fields

  private ObjectID _objectID;
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

  public RelationEndPoint (ObjectID objectID, string propertyName)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetByClassID (objectID.ClassID);
    _definition = classDefinition.GetMandatoryRelationEndPointDefinition (propertyName);
    _objectID = objectID;
    _id = new RelationEndPointID (objectID, propertyName);
  }

  protected RelationEndPoint (IRelationEndPointDefinition definition)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);
    _definition = definition;
  }

  // methods and properties

  public bool BeginRelationChange (RelationEndPoint oldEndPoint)
  {
    return BeginRelationChange (oldEndPoint, new NullRelationEndPoint (oldEndPoint.Definition));    
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

  public virtual void SetOppositeEndPoint (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);

    if (!IsVirtual)
      DataContainer.PropertyValues[PropertyName].SetRelationValue (endPoint.ObjectID);
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
      return ClientTransaction.Current.GetObject (_objectID); 
    }
  }

  public virtual DataContainer DataContainer
  {
    get { return DomainObject.DataContainer; }
  }

  public virtual ObjectID ObjectID
  {
    get { return _objectID; }
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

  #region INullable Members

  public virtual bool IsNull
  {
    get { return false; }
  }

  #endregion
}
}
