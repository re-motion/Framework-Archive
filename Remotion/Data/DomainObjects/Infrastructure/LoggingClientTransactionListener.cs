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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;
using Remotion.Text;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// A listener implementation logging all transaction events.
  /// </summary>
  [Serializable]
  public class LoggingClientTransactionListener : IClientTransactionListener
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (LoggingClientTransactionListener));

    public void TransactionInitializing (ClientTransaction clientTransaction)
    {
      if (s_log.IsDebugEnabled)
        s_log.Debug ("TransactionInitializing");
    }

    public void TransactionDiscarding (ClientTransaction clientTransaction)
    {
      if (s_log.IsDebugEnabled)
        s_log.Debug ("TransactionDiscarding");
    }

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      if (s_log.IsDebugEnabled)
        s_log.Debug ("SubTransactionCreating");
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      if (s_log.IsDebugEnabled)
        s_log.Debug ("SubTransactionCreated");
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type, DomainObject instance)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("NewObjectCreating: {0}", type.FullName);
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("ObjectsLoading: {0}", GetObjectIDString (objectIDs));
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("ObjectsUnloaded: {0}", GetDomainObjectsString (unloadedDomainObjects));
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("ObjectsLoaded: {0}", GetDomainObjectsString (domainObjects));
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("ObjectsUnloading: {0}", GetDomainObjectsString (unloadedDomainObjects));
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("ObjectDeleting: {0}", GetDomainObjectString (domainObject));
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("ObjectDeleted: {0}", GetDomainObjectString (domainObject));
    }

    public void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("PropertyValueReading: {0} ({1}, {2})", propertyValue.Name, valueAccess, dataContainer.ID);
    }

    public void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("PropertyValueRead: {0}=={1} ({2}, {3})", propertyValue.Name, value ?? "<null>", valueAccess, dataContainer.ID);
    }

    public void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("PropertyValueChanging: {0} {1}->{2} ({3})", propertyValue.Name, oldValue ?? "<null>", newValue ?? "<null>", dataContainer.ID);
    }

    public void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("PropertyValueChanged: {0} {1}->{2} ({3})", propertyValue.Name, oldValue ?? "<null>", newValue ?? "<null>", dataContainer.ID);
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("RelationReading: {0} ({1}, {2})", propertyName, valueAccess, GetDomainObjectString (domainObject));
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("RelationRead: {0}=={1} ({2}, {3})", propertyName, GetDomainObjectString (relatedObject), valueAccess, GetDomainObjectString (domainObject));
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
      {
        var domainObjectsString = relatedObjects.IsDataAvailable ? GetDomainObjectsString (relatedObjects) : "<data not loaded>";
        s_log.DebugFormat ("RelationRead: {0} ({1}, {2}): {3}", propertyName, valueAccess, domainObject.ID, domainObjectsString);
      }
    }

    public void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "RelationChanging: {0}: {1}->{2} /{3}",
            propertyName,
            GetDomainObjectString (oldRelatedObject),
            GetDomainObjectString (newRelatedObject),
            GetDomainObjectString (domainObject));
      }
    }

    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("RelationChanged: {0} ({1})", propertyName, GetDomainObjectString (domainObject));
    }

    public QueryResult<T> FilterQueryResult<T> (QueryResult<T> queryResult) where T : DomainObject
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "FilterQueryResult: {0}: {1} ({2})",
            queryResult.Query.ID,
            GetDomainObjectsString (queryResult.AsEnumerable().Cast<DomainObject>()),
            queryResult.Query.Statement);
      }
      return queryResult;
    }

    public void TransactionCommitting (ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("TransactionCommitting: {0}", GetDomainObjectsString (domainObjects));
    }

    public void TransactionCommitted (ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("TransactionCommitted: {0}", GetDomainObjectsString (domainObjects));
    }

    public void TransactionRollingBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("TransactionRollingBack: {0}", GetDomainObjectsString (domainObjects));
    }

    public void TransactionRolledBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("TransactionRolledBack: {0}", GetDomainObjectsString (domainObjects));
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("RelationEndPointMapRegistering: {0}", endPoint.ID);
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("RelationEndPointMapUnregistering: {0}", endPointID);
    }

    public void RelationEndPointUnloading (RelationEndPoint endPoint)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("RelationEndPointUnloading: {0}", endPoint.ID);
    }

    public void DataManagerMarkingObjectInvalid (ObjectID id)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("DataManagerMarkingObjectInvalid: {0}", id);
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("DataContainerMapRegistering: {0}", container.ID);
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("DataContainerMapUnregistering: {0}", container.ID);
    }

    private string GetObjectIDString (IEnumerable<ObjectID> objectIDs)
    {
      return SeparatedStringBuilder.Build (", ", objectIDs);
    }

    private string GetDomainObjectsString (IEnumerable<DomainObject> domainObjects)
    {
      return SeparatedStringBuilder.Build (", ", domainObjects.Select (obj => GetDomainObjectString (obj)));
    }

    private string GetDomainObjectString (DomainObject domainObject)
    {
      return domainObject != null ? domainObject.ID.ToString () : "<null>";
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
