using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.Relations
{
public class NullRelationEndPoint : ObjectEndPoint
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NullRelationEndPoint (IRelationEndPointDefinition definition)
  {
    base.Initialize (definition);
  }

  // methods and properties

  public override bool IsNull
  {
    get { return true; }
  }

  public override RelationLinkID CreateRelationLinkID()
  {
    return null;
  }

  public override DataContainer DataContainer
  {
    get { return null; }
  }

  public override DomainObject DomainObject
  {
    get { return null; }
  }

  public override IRelationEndPointDefinition OppositeEndPointDefinition
  {
    get { return null; }
  }

  public override RelationDefinition RelationDefinition
  {
    get { return null; }
  }

  public override void SetOppositeEndPoint (ObjectEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
  }

  public override bool BeginRelationChange(ObjectEndPoint oldRelationEndPoint, ObjectEndPoint newRelationEndPoint)
  {
    return true;
  }

  public override void EndRelationChange()
  {
  }

  public override ObjectID ObjectID
  {
    get { return null; }
  }
}
}
