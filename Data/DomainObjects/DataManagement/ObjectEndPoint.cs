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

  // construction and disposing

  public RelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition) 
    : this (domainObject.DataContainer, definition)
  {
  }

  public RelationEndPoint (DataContainer dataContainer, IRelationEndPointDefinition definition) 
  {
    ArgumentUtility.CheckNotNull ("definition", definition);

    Initialize (dataContainer, definition.PropertyName);
  }


  public RelationEndPoint (DomainObject domainObject, string propertyName) 
      : this (domainObject.DataContainer, propertyName)
  {
  }

  public RelationEndPoint (DataContainer dataContainer, string propertyName)
  {
    Initialize (dataContainer, propertyName);
  }

  protected RelationEndPoint ()
  {
  }

  private void Initialize (DataContainer dataContainer, string propertyName)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    _definition = dataContainer.ClassDefinition.GetMandatoryRelationEndPointDefinition (propertyName);
    _dataContainer = dataContainer;
  }

  // methods and properties

  public virtual void SetOppositeEndPoint (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

    if (!IsVirtual)
      _dataContainer.PropertyValues[PropertyName].SetRelationValue (relationEndPoint.ObjectID);
  }

  public virtual RelationLinkID CreateRelationLinkID ()
  {
    return new RelationLinkID (_dataContainer.ID, PropertyName);
  }

  public virtual bool BeginRelationChange (RelationEndPoint oldRelatedEndPoint)
  {
    return BeginRelationChange (oldRelatedEndPoint, new NullRelationEndPoint (oldRelatedEndPoint.Definition));    
  }

  public virtual bool BeginRelationChange (RelationEndPoint oldRelatedEndPoint, RelationEndPoint newRelatedEndPoint)
  {
    return DomainObject.BeginRelationChange (
        PropertyName, oldRelatedEndPoint.DomainObject, newRelatedEndPoint.DomainObject);
  }

  public virtual void EndRelationChange ()
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

  public virtual RelationDefinition RelationDefinition
  {
    get 
    {
      CheckDefinition ();
      return _definition.ClassDefinition.GetRelationDefinition (PropertyName); 
    }
  }

  public virtual IRelationEndPointDefinition Definition
  {
    get 
    {
      CheckDefinition ();
      return _definition; 
    }
  }

  protected void SetDefinition (IRelationEndPointDefinition definition)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);

    _definition = definition;
  }

  public virtual IRelationEndPointDefinition OppositeEndPointDefinition
  {
    get 
    {
      CheckDefinition ();
      return _definition.ClassDefinition.GetOppositeEndPointDefinition (PropertyName); 
    }
  }

  public virtual string PropertyName
  {
    get 
    { 
      CheckDefinition ();
      return _definition.PropertyName; 
    }
  }

  public virtual bool IsVirtual
  {
    get 
    {
      CheckDefinition ();
      return _definition.IsVirtual; 
    }
  }

  private void CheckDefinition ()
  {
    if (_definition == null)
      throw new InvalidOperationException ("Initialize must be called first.");
  }

  #region INullable Members

  public virtual bool IsNull
  {
    get { return false; }
  }

  #endregion
}
}
