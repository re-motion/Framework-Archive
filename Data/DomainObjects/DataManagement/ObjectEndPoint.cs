using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.Relations
{
public class ObjectEndPoint : RelationEndPoint, INullable
{
  // types

  // static members and constants

  // member fields

  private DataContainer _dataContainer;

  // construction and disposing

  public ObjectEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition) 
    : this (domainObject.DataContainer, definition)
  {
  }

  public ObjectEndPoint (DataContainer dataContainer, IRelationEndPointDefinition definition) 
  {
    ArgumentUtility.CheckNotNull ("definition", definition);

    Initialize (dataContainer, definition.PropertyName);
  }


  public ObjectEndPoint (DomainObject domainObject, string propertyName) 
      : this (domainObject.DataContainer, propertyName)
  {
  }

  public ObjectEndPoint (DataContainer dataContainer, string propertyName)
  {
    Initialize (dataContainer, propertyName);
  }

  protected ObjectEndPoint ()
  {
  }

  private void Initialize (DataContainer dataContainer, string propertyName)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    base.Initialize (dataContainer.ClassDefinition.GetMandatoryRelationEndPointDefinition (propertyName));
    _dataContainer = dataContainer;
  }

  // methods and properties

  public virtual void SetOppositeEndPoint (ObjectEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

    if (!IsVirtual)
      _dataContainer.PropertyValues[PropertyName].SetRelationValue (relationEndPoint.ObjectID);
  }

  public virtual RelationLinkID CreateRelationLinkID ()
  {
    return new RelationLinkID (_dataContainer.ID, PropertyName);
  }

  public virtual bool BeginRelationChange (ObjectEndPoint oldRelatedEndPoint)
  {
    return BeginRelationChange (oldRelatedEndPoint, new NullObjectEndPoint (oldRelatedEndPoint.Definition));    
  }

  public override bool BeginRelationChange (ObjectEndPoint oldRelatedEndPoint, ObjectEndPoint newRelatedEndPoint)
  {
    return DomainObject.BeginRelationChange (
        PropertyName, oldRelatedEndPoint.DomainObject, newRelatedEndPoint.DomainObject);
  }

  public override void EndRelationChange ()
  {
    DomainObject.EndRelationChange (PropertyName);
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

  #region INullable Members

  public virtual bool IsNull
  {
    get { return false; }
  }

  #endregion
}
}
