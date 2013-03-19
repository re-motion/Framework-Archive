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

using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes
{
  [Serializable]
  public class SerializableFetchEnabledPersistenceStrategyFake : IFetchEnabledPersistenceStrategy
  {
    public ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      throw new System.NotImplementedException();
    }

    public ILoadedObjectData LoadObjectData (ObjectID id)
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<ILoadedObjectData> LoadObjectData (IEnumerable<ObjectID> objectIDs)
    {
      throw new System.NotImplementedException();
    }

    public ILoadedObjectData ResolveObjectRelationData (RelationEndPointID relationEndPointID, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<ILoadedObjectData> ResolveCollectionRelationData (RelationEndPointID relationEndPointID, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<ILoadedObjectData> ExecuteCollectionQuery (IQuery query, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      throw new System.NotImplementedException();
    }

    public object ExecuteScalarQuery (IQuery query)
    {
      throw new System.NotImplementedException();
    }

    public void PersistData (IEnumerable<PersistableData> data)
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<LoadedObjectDataWithDataSourceData> ExecuteFetchQuery (IQuery query, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      throw new System.NotImplementedException();
    }
  }
}