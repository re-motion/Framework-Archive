using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Defines an interface for classes storing the data for a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public interface ICollectionEndPointDataKeeper : IFlattenedSerializable
  {
    IDomainObjectCollectionData CollectionData { get; }
    ReadOnlyCollectionDataDecorator OriginalCollectionData { get; }
    IObjectEndPoint[] OppositeEndPoints { get; }
    IObjectEndPoint[] OriginalOppositeEndPoints { get; }
    DomainObject[] OriginalItemsWithoutEndPoints { get; }
    IComparer<DomainObject> SortExpressionBasedComparer { get; }
    RelationEndPointID EndPointID { get; }

    bool ContainsOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint);

    void RegisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint);
    void UnregisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint);

    void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject);

    bool HasDataChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy);

    void SortCurrentAndOriginalData ();
    void Commit ();
  }
}