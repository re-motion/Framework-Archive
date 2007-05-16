using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class AccessControlList : AccessControlObject
  {
    // types

    // static members and constants

    public static AccessControlList NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return NewObject<AccessControlList>().With();
      }
    }

    public new static AccessControlList GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (AccessControlList) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    private DomainObjectCollection _accessControlEntriesToBeDeleted;
    private DomainObjectCollection _stateCombinations;

    // construction and disposing

    protected AccessControlList ()
    {
      Touch();
      Initialize();
    }

    // methods and properties

    //TODO: Add test for initialize during on load
    protected override void OnLoaded ()
    {
      base.OnLoaded();
      Initialize();
    }

    private void Initialize ()
    {
      StateCombinations.Added += new DomainObjectCollectionChangeEventHandler (StateCombinations_Added);
      AccessControlEntries.Added += new DomainObjectCollectionChangeEventHandler (AccessControlEntries_Added);
    }

    private void StateCombinations_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      StateCombination stateCombination = (StateCombination) args.DomainObject;
      DomainObjectCollection stateCombinations = StateCombinations;
      if (stateCombinations.Count == 1)
        stateCombination.Index = 0;
      else
        stateCombination.Index = ((StateCombination) stateCombinations[stateCombinations.Count - 2]).Index + 1;
      Touch();
      if (Class != null)
        Class.Touch();
    }

    private void AccessControlEntries_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      AccessControlEntry ace = (AccessControlEntry) args.DomainObject;
      DomainObjectCollection accessControlEntries = AccessControlEntries;
      if (accessControlEntries.Count == 1)
        ace.Index = 0;
      else
        ace.Index = ((AccessControlEntry) accessControlEntries[accessControlEntries.Count - 2]).Index + 1;
      Touch();
    }

    public virtual DateTime ChangedAt
    {
      get { return CurrentProperty<DateTime>().GetValue(); }
      private set { SetPropertyValue ("Rubicon.SecurityManager.Domain.AccessControl.AccessControlList.ChangedAt", value); }
    }

    public void Touch ()
    {
      ChangedAt = DateTime.Now;
    }

    public abstract int Index { get; set; }

    [DBBidirectionalRelation ("AccessControlLists")]
    [DBColumn ("SecurableClassID")]
    [Mandatory]
    public abstract SecurableClassDefinition Class { get; set; }

    [DBBidirectionalRelation ("AccessControlList", SortExpression = "[Index] ASC")]
    [Mandatory]
    public abstract ObjectList<StateCombination> StateCombinations { get; }

    [DBBidirectionalRelation ("AccessControlList", SortExpression = "[Index] ASC")]
    public abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

    public AccessControlEntry[] FindMatchingEntries (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      List<AccessControlEntry> entries = new List<AccessControlEntry>();

      foreach (AccessControlEntry entry in AccessControlEntries)
      {
        if (entry.MatchesToken (token))
          entries.Add (entry);
      }

      return entries.ToArray();
    }

    public AccessControlEntry[] FilterAcesByPriority (AccessControlEntry[] aces)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("aces", aces);

      if (aces.Length == 0)
        return aces;

      AccessControlEntry[] sortedAces = (AccessControlEntry[]) aces.Clone();

      Array.Sort (sortedAces, new AccessControlEntryPriorityComparer());
      Array.Reverse (sortedAces);

      int highestPriority = sortedAces[0].ActualPriority;

      return Array.FindAll<AccessControlEntry> (
          sortedAces,
          delegate (AccessControlEntry current) { return current.ActualPriority == highestPriority; });
    }

    public AccessTypeDefinition[] GetAccessTypes (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      List<AccessTypeDefinition> accessTypes = new List<AccessTypeDefinition>();

      AccessControlEntry[] aces = FilterAcesByPriority (FindMatchingEntries (token));
      foreach (AccessControlEntry ace in aces)
      {
        foreach (AccessTypeDefinition allowedAccessType in ace.GetAllowedAccessTypes())
        {
          if (!accessTypes.Contains (allowedAccessType))
            accessTypes.Add (allowedAccessType);
        }
      }

      return accessTypes.ToArray();
    }

    //TODO: Rewrite with test

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _accessControlEntriesToBeDeleted = AccessControlEntries.Clone();
      _stateCombinations = StateCombinations.Clone();
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      foreach (AccessControlEntry accessControlEntry in _accessControlEntriesToBeDeleted)
        accessControlEntry.Delete();
      _accessControlEntriesToBeDeleted = null;

      foreach (StateCombination stateCombination in _stateCombinations)
        stateCombination.Delete();
      _stateCombinations = null;
    }

    public StateCombination CreateStateCombination ()
    {
      if (Class == null)
        throw new InvalidOperationException ("Cannot create StateCombination if no SecurableClassDefinition is assigned to this AccessControlList.");

      StateCombination stateCombination = StateCombination.NewObject (ClientTransaction);
      stateCombination.Class = Class;
      stateCombination.AccessControlList = this;

      return stateCombination;
    }

    public AccessControlEntry CreateAccessControlEntry ()
    {
      if (Class == null)
        throw new InvalidOperationException ("Cannot create AccessControlEntry if no SecurableClassDefinition is assigned to this AccessControlList.");

      AccessControlEntry accessControlEntry = AccessControlEntry.NewObject (ClientTransaction);
      foreach (AccessTypeDefinition accessTypeDefinition in Class.AccessTypes)
        accessControlEntry.AttachAccessType (accessTypeDefinition);
      accessControlEntry.AccessControlList = this;

      return accessControlEntry;
    }
  }
}