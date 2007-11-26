using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
  public abstract class RelationEndPointModification
  {
    public readonly RelationEndPoint AffectedEndPoint;
    public readonly IEndPoint OldEndPoint;
    public readonly IEndPoint NewEndPoint;

    public RelationEndPointModification (RelationEndPoint affectedEndPoint, IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      ArgumentUtility.CheckNotNull ("affectedEndPoint", affectedEndPoint);
      ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
      ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

      AffectedEndPoint = affectedEndPoint;
      OldEndPoint = oldEndPoint;
      NewEndPoint = newEndPoint;
    }

    public abstract void Begin ();
    public abstract void Perform ();
    public abstract void End ();
  }
}