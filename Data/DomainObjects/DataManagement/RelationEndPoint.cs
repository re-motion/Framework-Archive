using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.Relations
{
public abstract class RelationEndPoint
{
  // types

  // static members and constants

  // member fields

  private IRelationEndPointDefinition _definition;

  // construction and disposing

  protected RelationEndPoint (IRelationEndPointDefinition definition)
  {
    Initialize (definition);
  }

  protected RelationEndPoint ()
  {
  }

  // abstract methods and properties

  public abstract bool BeginRelationChange (ObjectEndPoint oldRelatedEndPoint, ObjectEndPoint newRelatedEndPoint);
  public abstract void EndRelationChange ();

  // methods and properties

  protected void Initialize (IRelationEndPointDefinition definition)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);

    _definition = definition;
  }

  public IRelationEndPointDefinition Definition 
  {
    get { return _definition; }
  }

  public virtual RelationDefinition RelationDefinition
  {
    get 
    {
      CheckDefinition ();
      return _definition.ClassDefinition.GetRelationDefinition (PropertyName); 
    }
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
}
}
