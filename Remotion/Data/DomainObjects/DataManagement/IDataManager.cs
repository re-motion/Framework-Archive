// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides an interface for classes managing the data inside a <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IDataManager
  {
    IEnumerable<ObjectID> InvalidObjectIDs { get; }
    IDataContainerMapReadOnlyView DataContainerMap { get; }
    IRelationEndPointMapReadOnlyView RelationEndPointMap { get; }
    DomainObjectStateCache DomainObjectStateCache { get; }

    bool IsInvalid (ObjectID id);
    DomainObject GetInvalidObjectReference (ObjectID id);
    void MarkObjectInvalid (DomainObject domainObject);
    void ClearInvalidFlag (ObjectID objectID);

    void Discard (DataContainer dataContainer);

    IEnumerable<Tuple<DomainObject, DataContainer>> GetLoadedData ();
    IEnumerable<Tuple<DomainObject, DataContainer, StateType>> GetLoadedDataByObjectState (params StateType[] domainObjectStates);
    IEnumerable<Tuple<DomainObject, DataContainer, StateType>> GetChangedDataByObjectState ();
    IEnumerable<DataContainer> GetChangedDataContainersForCommit ();
    IEnumerable<RelationEndPoint> GetChangedRelationEndPoints ();
    IEnumerable<RelationEndPoint> GetOppositeRelationEndPoints (DataContainer dataContainer);
    
    bool HasRelationChanged (DataContainer dataContainer);
    void CheckMandatoryRelations (DataContainer dataContainer);
    
    void RegisterDataContainer (DataContainer dataContainer);
    void MarkCollectionEndPointComplete (RelationEndPointID relationEndPointID);
    
    void Commit ();
    void Rollback ();
    
    DataContainer GetDataContainerWithLazyLoad (ObjectID objectID);
    DataContainer GetDataContainerWithoutLoading (ObjectID id);
    IDataManagementCommand CreateDeleteCommand (DomainObject deletedObject);
    UnloadCommand CreateUnloadCommand (params ObjectID[] objectIDs);
  }
}