﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides access to the parent transaction for <see cref="SubPersistenceStrategy"/>.
  /// </summary>
  public interface IParentTransactionContext
  {
    IObjectID<DomainObject> CreateNewObjectID (ClassDefinition classDefinition);

    DomainObject GetObject (IObjectID<DomainObject> objectID);
    DomainObject[] GetObjects (IEnumerable<IObjectID<DomainObject>> objectIDs);
    DomainObject TryGetObject (IObjectID<DomainObject> objectID);
    DomainObject[] TryGetObjects (IEnumerable<IObjectID<DomainObject>> objectIDs);

    DomainObject ResolveRelatedObject (RelationEndPointID relationEndPointID);
    IEnumerable<DomainObject> ResolveRelatedObjects (RelationEndPointID relationEndPointID);

    QueryResult<DomainObject> ExecuteCollectionQuery (IQuery query);
    IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query);
    object ExecuteScalarQuery (IQuery query);

    DataContainer GetDataContainerWithoutLoading (IObjectID<DomainObject> objectID);
    DataContainer GetDataContainerWithLazyLoad (IObjectID<DomainObject> objectID, bool throwOnNotFound);
    IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID relationEndPointID);

    bool IsInvalid (IObjectID<DomainObject> objectID);

    IUnlockedParentTransactionContext UnlockParentTransaction ();
  }
}