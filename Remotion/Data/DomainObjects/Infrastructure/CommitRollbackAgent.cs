// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Performs the <see cref="ClientTransaction.Commit"/> and <see cref="ClientTransaction.Rollback"/> operations by gathering the commit set from the 
  /// <see cref="IDataManager"/>, raising events via the <see cref="IClientTransactionEventSink"/>, and persisting the commit set via the 
  /// <see cref="IPersistenceStrategy"/>.
  /// </summary>
  [Serializable]
  public class CommitRollbackAgent : ICommitRollbackAgent
  {
    private readonly IClientTransactionEventSink _eventSink;
    private readonly IPersistenceStrategy _persistenceStrategy;
    private readonly IDataManager _dataManager;

    public CommitRollbackAgent (IClientTransactionEventSink eventSink, IPersistenceStrategy persistenceStrategy, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      _eventSink = eventSink;
      _persistenceStrategy = persistenceStrategy;
      _dataManager = dataManager;
    }

    public IClientTransactionEventSink EventSink
    {
      get { return _eventSink; }
    }

    public IPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public bool HasDataChanged ()
    {
      return _dataManager.GetNewChangedDeletedData ().Any ();
    }

    public void CommitData ()
    {
      var persistableDataItems = BeginCommit ();
      _eventSink.RaiseEvent ((tx, l) => l.TransactionCommitValidate (tx, new ReadOnlyCollection<PersistableData> (persistableDataItems)));

      _persistenceStrategy.PersistData (persistableDataItems);
      _dataManager.Commit ();

      var changedButNotDeletedDomainObjects = persistableDataItems
          .Where (item => item.DomainObjectState != StateType.Deleted)
          .Select (item => item.DomainObject)
          .ToList ()
          .AsReadOnly ();
      EndCommit (changedButNotDeletedDomainObjects);
    }

    public void RollbackData ()
    {
      var persistableDataItems = BeginRollback ();

      _dataManager.Rollback ();

      var changedButNotNewItems =
          persistableDataItems.Where (item => item.DomainObjectState != StateType.New)
              .Select (item => item.DomainObject)
              .ToList()
              .AsReadOnly();
      EndRollback (changedButNotNewItems);
    }

    private IList<PersistableData> BeginCommit ()
    {
      // TODO Doc: ES

      // Note regarding to Committing: 
      // Every object raises a Committing event even if another object's Committing event changes the first object's state back to original 
      // during its own Committing event. Because the event order of .NET is not deterministic, this behavior is desired to ensure consistency: 
      // Every object changed during a ClientTransaction raises a Committing event regardless of the Committing event order of specific objects.  

      // Note regarding to Committed: 
      // If an object is changed back to its original state during the Committing phase, no Committed event will be raised,
      // because in this case the object won't be committed to the underlying backend (e.g. database).

      var committingEventNotRaised = _dataManager.GetNewChangedDeletedData ().ToList ();
      var committingEventRaised = new HashSet<ObjectID>();

      while (true)
      {
        var eventArgReadOnlyCollection = ListAdapter.AdaptReadOnly (committingEventNotRaised, item => item.DomainObject);
        var committingEventRegistrar = new CommittingEventRegistrar();
        _eventSink.RaiseEvent ((tx, l) => l.TransactionCommitting (tx, eventArgReadOnlyCollection, committingEventRegistrar));

        committingEventRaised.UnionWith (committingEventNotRaised.Select (item => item.DomainObject.ID));
        committingEventRaised.ExceptWith (committingEventRegistrar.RegisteredObjects.Select (obj => obj.ID));

        var changedItems = _dataManager.GetNewChangedDeletedData ().ToList();
        committingEventNotRaised = changedItems.Where (item => !committingEventRaised.Contains (item.DomainObject.ID)).ToList ();
       
        if (!committingEventNotRaised.Any ())
          return changedItems;
      }
    }

    private void EndCommit (ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      _eventSink.RaiseEvent ((tx, l) => l.TransactionCommitted (tx, changedDomainObjects));
    }

    private IList<PersistableData> BeginRollback ()
    {
      // TODO Doc: ES

      // Note regarding to RollingBack: 
      // Every object raises a RollingBack event even if another object's RollingBack event changes the first object's state back to original 
      // during its own RollingBack event. Because the event order of .NET is not deterministic, this behavior is desired to ensure consistency: 
      // Every object changed during a ClientTransaction raises a RollingBack event regardless of the RollingBack event order of specific objects.  

      // Note regarding to RolledBack: 
      // If an object is changed back to its original state during the RollingBack phase, no RolledBack event will be raised,
      // because the object actually has never been changed from a ClientTransaction's perspective.

      var rollingBackEventNotRaised = _dataManager.GetNewChangedDeletedData ().ToList ();
      var rollingBackEventRaised = new HashSet<ObjectID> ();

      while (true)
      {
        var eventArgReadOnlyCollection = ListAdapter.AdaptReadOnly (rollingBackEventNotRaised, item => item.DomainObject);
        _eventSink.RaiseEvent ((tx, l) => l.TransactionRollingBack (tx, eventArgReadOnlyCollection));

        rollingBackEventRaised.UnionWith (rollingBackEventNotRaised.Select (item => item.DomainObject.ID));

        var changedItems = _dataManager.GetNewChangedDeletedData ().ToList ();
        rollingBackEventNotRaised = changedItems.Where (item => !rollingBackEventRaised.Contains (item.DomainObject.ID)).ToList ();

        if (!rollingBackEventNotRaised.Any ())
          return changedItems;
      }
    }

    private void EndRollback (ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      _eventSink.RaiseEvent ((tx, l) => l.TransactionRolledBack (tx, changedDomainObjects));
    }
  }
}