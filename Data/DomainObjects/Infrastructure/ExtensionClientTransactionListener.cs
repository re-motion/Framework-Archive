/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// A <see cref="IClientTransactionListener"/> implementation that notifies <see cref="IClientTransactionExtension">IClientTransactionExtensions</see>
  /// about transaction events.
  /// </summary>
  /// <remarks>
  /// The <see cref="ClientTransaction"/> class uses this listener to implement its extension mechanism.
  /// </remarks>
  [Serializable]
  public class ExtensionClientTransactionListener : IClientTransactionListener
  {
    private readonly ClientTransactionExtensionCollection _extensions;
    private readonly ClientTransaction _clientTransaction;

    public ExtensionClientTransactionListener (ClientTransaction clientTransaction, ClientTransactionExtensionCollection extensions)
    {
      _clientTransaction = clientTransaction;
      _extensions = extensions;
    }

    public ClientTransactionExtensionCollection Extensions
    {
      get { return _extensions; }
    }

    public void SubTransactionCreating ()
    {
      // not handled by this listener
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      // not handled by this listener
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
      Extensions.NewObjectCreating (_clientTransaction, type);
    }

    public void ObjectLoading (ObjectID id)
    {
      Extensions.ObjectLoading (_clientTransaction, id);
    }

    public void ObjectInitializedFromDataContainer (ObjectID id, DomainObject instance)
    {
      // not handled by this listener
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      Extensions.ObjectsLoaded (_clientTransaction, domainObjects);
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      Extensions.ObjectDeleting (_clientTransaction, domainObject);
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      Extensions.ObjectDeleted (_clientTransaction, domainObject);
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      Extensions.PropertyValueReading (_clientTransaction, dataContainer, propertyValue, valueAccess);
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      Extensions.PropertyValueRead (_clientTransaction, dataContainer, propertyValue, value, valueAccess);
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (propertyValue.PropertyType != typeof (ObjectID))
        Extensions.PropertyValueChanging (_clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (propertyValue.PropertyType != typeof (ObjectID))
        Extensions.PropertyValueChanged (_clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      Extensions.RelationReading (_clientTransaction, domainObject, propertyName, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      Extensions.RelationRead (_clientTransaction, domainObject, propertyName, relatedObject, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      Extensions.RelationRead (_clientTransaction, domainObject, propertyName, relatedObjects, valueAccess);
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      Extensions.RelationChanging (_clientTransaction, domainObject, propertyName, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      Extensions.RelationChanged (_clientTransaction, domainObject, propertyName);
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      Extensions.FilterQueryResult (_clientTransaction, queryResult, query);
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      Extensions.Committing (_clientTransaction, domainObjects);
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      Extensions.Committed (_clientTransaction, domainObjects);
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      Extensions.RollingBack (_clientTransaction, domainObjects);
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      Extensions.RolledBack (_clientTransaction, domainObjects);
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapCopyingTo (RelationEndPointMap source)
    {
      // not handled by this listener
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      // not handled by this listener
    }

    public void DataManagerCopyingFrom (DataManager source)
    {
      // not handled by this listener
    }

    public void DataManagerCopyingTo (DataManager destination)
    {
      // not handled by this listener
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      // not handled by this listener
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      // not handled by this listener
    }

    public void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      // not handled by this listener
    }

    public void DataContainerMapCopyingTo (DataContainerMap destination)
    {
      // not handled by this listener
    }
  }
}
