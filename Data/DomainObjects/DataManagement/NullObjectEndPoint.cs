using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class NullObjectEndPoint : ObjectEndPoint
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NullObjectEndPoint (IRelationEndPointDefinition definition) : base (definition)
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

  public override void CheckMandatory ()
  {
    throw new InvalidOperationException ("CheckMandatory cannot be called on a NullObjectEndPoint.");    
  }

  public override void Commit ()
  {
    throw new InvalidOperationException ("Commit cannot be called on a NullObjectEndPoint.");    
  }

  public override void Rollback ()
  {
    throw new InvalidOperationException ("Rollback cannot be called on a NullObjectEndPoint.");
  }

  public override bool HasChanged
  {
    get { return false; }
  }

  public override DataContainer GetDataContainer ()
  {
    return null;
  }

  public override DomainObject GetDomainObject ()
  {
    return null;
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

  #region ICloneable Members

  public override object Clone ()
  {
    return new NullObjectEndPoint (this.Definition);
  }

  #endregion
}
}
