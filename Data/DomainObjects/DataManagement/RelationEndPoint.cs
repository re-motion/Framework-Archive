using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.Relations
{
public class RelationEndPoint : INullable
{
  // types

  // static members and constants

  // member fields

  private DataContainer _dataContainer;
  private IRelationEndPointDefinition _definition;
  private RelationLinkID _linkID;

  // construction and disposing
  
  public RelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition) 
      : this (domainObject.DataContainer, definition)
  {
  }

  public RelationEndPoint (DataContainer dataContainer, IRelationEndPointDefinition definition) 
      : this (dataContainer, definition.PropertyName) 
  {
  }


  public RelationEndPoint (DomainObject domainObject, string propertyName) 
      : this (domainObject.DataContainer, propertyName)
  {
  }

  public RelationEndPoint (DataContainer dataContainer, string propertyName)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    _definition = dataContainer.ClassDefinition.GetMandatoryRelationEndPointDefinition (propertyName);
    _dataContainer = dataContainer;
    _linkID = new RelationLinkID (dataContainer.ID, propertyName);
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
      _dataContainer.PropertyValues[PropertyName].SetRelationValue (endPoint.ObjectID);
  }

  public IRelationEndPointDefinition Definition 
  {
    get { return _definition; }
  }

  public virtual DataContainer DataContainer
  {
    get { return _dataContainer; }
  }

  public virtual DomainObject DomainObject
  {
    get { return _dataContainer.DomainObject; }
  }

  public virtual ObjectID ObjectID
  {
    get { return _dataContainer.ID; }
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

  public virtual RelationLinkID LinkID
  {
    get { return _linkID; }
  }

  #region INullable Members

  public virtual bool IsNull
  {
    get { return false; }
  }

  #endregion
}
}
