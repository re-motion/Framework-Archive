namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents a virtual <see cref="IRelationEndPoint"/> holding a single opposite <see cref="IObjectID{DomainObject}"/>, i.e. the non-foreign key side in a 
  /// 1:1 relation.
  /// </summary>
  public interface IVirtualObjectEndPoint : IVirtualEndPoint<DomainObject>, IObjectEndPoint
  {
    void MarkDataComplete (DomainObject item);
  }
}