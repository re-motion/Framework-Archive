using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.Relations
{
public class NullRelationEndPoint : RelationEndPoint
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NullRelationEndPoint (IRelationEndPointDefinition definition) : base (definition)
  {
  }

  // methods and properties

  public override bool BeginRelationChange (RelationEndPoint oldEndPoint, RelationEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);
    return true;
  }

  public override void EndRelationChange ()
  {
  }

  public override void SetOppositeEndPoint (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);
  }

  public override DataContainer DataContainer
  {
    get { return null; }
  }

  public override DomainObject DomainObject
  {
    get { return null; }
  }

  public override ObjectID ObjectID
  {
    get { return null; }
  }

  public override RelationLinkID LinkID
  {
    get { return null; }
  }

  public override bool IsNull
  {
    get { return true; }
  }
}
}
