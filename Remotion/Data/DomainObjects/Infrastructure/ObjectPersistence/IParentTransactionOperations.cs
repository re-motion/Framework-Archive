﻿using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides access to the parent transaction operations required by <see cref="SubPersistenceStrategy"/>.
  /// </summary>
  public interface IParentTransactionOperations : IDisposable
  {
    ObjectID CreateNewObjectID (ClassDefinition classDefinition);

    DomainObject GetObject (ObjectID objectID);
    DomainObject[] GetObjects (IEnumerable<ObjectID> objectIDs);
    DomainObject TryGetObject (ObjectID objectID);
    DomainObject[] TryGetObjects (IEnumerable<ObjectID> objectIDs);

    DomainObject ResolveRelatedObject (RelationEndPointID relationEndPointID);
    IEnumerable<DomainObject> GetRelatedObjects (RelationEndPointID relationEndPointID);

    QueryResult<DomainObject> ExecuteCollectionQuery (IQuery query);
    IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query);
    object ExecuteScalarQuery (IQuery query);

    DataContainer GetDataContainerWithoutLoading (ObjectID objectID);
    DataContainer GetDataContainerWithLazyLoad (ObjectID objectID, bool throwOnNotFound);
    IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID relationEndPointID);

    bool IsInvalid (ObjectID objectID);
    void MarkNotInvalid (ObjectID objectID);

    void RegisterDataContainer (DataContainer dataContainer);
    IDataManagementCommand CreateDeleteCommand (DomainObject deletedObject);
    void Discard (DataContainer dataContainer);
  }
}