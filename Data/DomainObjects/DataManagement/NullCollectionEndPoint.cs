using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class NullCollectionEndPoint : CollectionEndPoint
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NullCollectionEndPoint (IRelationEndPointDefinition definition) : base (definition)
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

  public override void CheckMandatory()
  {
  }

  public override void Commit ()
  {
    throw new InvalidOperationException ("Commit cannot be called on a NullCollectionEndPoint.");    
  }

  public override void Rollback ()
  {
    throw new InvalidOperationException ("Rollback cannot be called on a NullCollectionEndPoint.");
  }

  public override bool HasChanged
  {
    get { return false; }
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

  public override RelationEndPointID ID
  {
    get { return null; }
  }

  public override bool IsNull
  {
    get { return true; }
  }
}
}
