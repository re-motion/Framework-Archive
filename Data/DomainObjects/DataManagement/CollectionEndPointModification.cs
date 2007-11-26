using System;

namespace Rubicon.Data.DomainObjects.DataManagement
{
  public class CollectionEndPointModification : RelationEndPointModification
  {
    public readonly CollectionEndPointChangeAgent ChangeAgent;

    private readonly CollectionEndPoint _affectedEndPoint;

    public CollectionEndPointModification (CollectionEndPoint affectedEndPoint, CollectionEndPointChangeAgent changeAgent)
      : base (affectedEndPoint, changeAgent.OldEndPoint, changeAgent.NewEndPoint)
    {
      _affectedEndPoint = affectedEndPoint;
      ChangeAgent = changeAgent;
    }

    public override void Begin ()
    {
      ChangeAgent.BeginRelationChange();
      _affectedEndPoint.BeginRelationChange (OldEndPoint, NewEndPoint);
    }

    public override void Perform ()
    {
      _affectedEndPoint.PerformRelationChange();
    }

    public override void End ()
    {
      ChangeAgent.EndRelationChange();
      _affectedEndPoint.EndRelationChange();
    }
  }
}