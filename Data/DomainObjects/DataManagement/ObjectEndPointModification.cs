using System;

namespace Rubicon.Data.DomainObjects.DataManagement
{
  public class ObjectEndPointModification : RelationEndPointModification
  {
    private readonly ObjectEndPoint _affectedEndPoint;

    public ObjectEndPointModification (ObjectEndPoint affectedEndPoint, IEndPoint oldEndPoint, IEndPoint newEndPoint)
        : base (affectedEndPoint, oldEndPoint, newEndPoint)
    {
      _affectedEndPoint = affectedEndPoint;
    }

    public override void Begin ()
    {
      _affectedEndPoint.BeginRelationChange (OldEndPoint, NewEndPoint);
    }

    public override void Perform ()
    {
      _affectedEndPoint.PerformRelationChange (NewEndPoint);
    }

    public override void End ()
    {
      _affectedEndPoint.EndRelationChange();
    }
  }
}