using System;
using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Represents a top-level <see cref="ClientTransaction"/>, which does not have a parent transaction.
  /// </summary>
  [Serializable]
  public class RootClientTransaction : ClientTransaction
  {
    private readonly Dictionary<ObjectID, DomainObject> _enlistedObjects;

    /// <summary>
    /// Initializes a new instance of the <b>RootClientTransaction</b> class.
    /// </summary>
    public RootClientTransaction ()
      : base (new Dictionary<Enum, object>(), new ClientTransactionExtensionCollection ())
    {
      _enlistedObjects = new Dictionary<ObjectID, DomainObject>();
    }

    public override bool IsDiscarded
    {
      get { return false; }
    }

    public override ClientTransaction ParentTransaction
    {
      get { return null; }
    }

    public override ClientTransaction RootTransaction
    {
      get { return this; }
    }

    public override bool ReturnToParentTransaction ()
    {
      return false;
    }

    
    protected internal override void DoEnlistDomainObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      DomainObject alreadyEnlistedObject = GetEnlistedOrNull (domainObject.ID);
      if (alreadyEnlistedObject != null && alreadyEnlistedObject != domainObject)
      {
        string message = string.Format ("A domain object instance for object '{0}' already exists in this transaction.", domainObject.ID);
        throw new InvalidOperationException (message);
      }

      _enlistedObjects.Add (domainObject.ID, domainObject);
    }

    protected internal override bool IsEnlisted (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      return GetEnlistedOrNull (domainObject.ID) == domainObject;
    }

    private DomainObject GetEnlistedOrNull (ObjectID objectID)
    {
      DomainObject domainObject;
      _enlistedObjects.TryGetValue (objectID, out domainObject);
      return domainObject;
    }

    protected override void PersistData (DataContainerCollection changedDataContainers)
    {
      ArgumentUtility.CheckNotNull ("changedDataContainers", changedDataContainers);

      if (changedDataContainers.Count > 0)
      {
        using (PersistenceManager persistenceManager = new PersistenceManager())
        {
          persistenceManager.Save (changedDataContainers);
        }
      }
    }

    protected internal override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      ObjectID newObjectID;
      using (PersistenceManager persistenceManager = new PersistenceManager ())
      {
        newObjectID = persistenceManager.CreateNewObjectID (classDefinition);
      }
      return newObjectID;
    }

    protected override DataContainer LoadDataContainer (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      using (PersistenceManager persistenceManager = new PersistenceManager ())
      {
        DataContainer dataContainer = persistenceManager.LoadDataContainer (id);
        TransactionEventSink.ObjectLoading (dataContainer.ID);
        SetClientTransaction (dataContainer);

        DataManager.RegisterExistingDataContainer (dataContainer);
        return dataContainer;
      }
    }

    internal protected override DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      using (EnterSideEffectFreeScope ())
      {
        DomainObject domainObject = GetObject (relationEndPointID.ObjectID, false);

        using (PersistenceManager persistenceManager = new PersistenceManager ())
        {
          DataContainer relatedDataContainer = persistenceManager.LoadRelatedDataContainer (domainObject.GetDataContainer (), relationEndPointID);
          if (relatedDataContainer != null)
          {
            TransactionEventSink.ObjectLoading (relatedDataContainer.ID);
            SetClientTransaction (relatedDataContainer);
            DataManager.RegisterExistingDataContainer (relatedDataContainer);

            DomainObjectCollection loadedDomainObjects = new DomainObjectCollection (new DomainObject[] { relatedDataContainer.DomainObject }, true);
            OnLoaded (new ClientTransactionEventArgs (loadedDomainObjects));

            return relatedDataContainer.DomainObject;
          }
          else
          {
            DataManager.RelationEndPointMap.RegisterObjectEndPoint (relationEndPointID, null);
            return null;
          }
        }
      }
    }

    internal protected override DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      using (EnterSideEffectFreeScope ())
      {
        using (PersistenceManager persistenceManager = new PersistenceManager ())
        {
          DataContainerCollection relatedDataContainers = persistenceManager.LoadRelatedDataContainers (relationEndPointID);
          return MergeLoadedDomainObjects (relatedDataContainers, relationEndPointID);
        }
      }
    }
  }
}