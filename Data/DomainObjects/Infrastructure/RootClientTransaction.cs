using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.DataManagement;
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
    /// <summary>
    /// Initializes a new instance of the <b>RootClientTransaction</b> class.
    /// </summary>
    public RootClientTransaction ()
      : base (new Dictionary<Enum, object>(), new ClientTransactionExtensionCollection ())
    {
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