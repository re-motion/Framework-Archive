using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public class AccessControlList : AccessControlObject
  {
    // types

    // static members and constants

    public static new AccessControlList GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (AccessControlList) DomainObject.GetObject (id, clientTransaction);
    }

    public static new AccessControlList GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (AccessControlList) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    private DomainObjectCollection _accessControlEntriesToBeDeleted;
    private DomainObjectCollection _stateCombinations;

    // construction and disposing

    public AccessControlList (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
      Touch ();
      Initialize ();
    }

    protected AccessControlList (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    //TODO: Add test for initialize during on load
    protected override void OnLoaded ()
    {
      base.OnLoaded ();
      Initialize ();
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
      Touch ();
      if (Class != null)
        Class.Touch ();
    }

    private void AccessControlEntries_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      AccessControlEntry ace = (AccessControlEntry) args.DomainObject;
      DomainObjectCollection accessControlEntries = AccessControlEntries;
      if (accessControlEntries.Count == 1)
        ace.Index = 0;
      else
        ace.Index = ((AccessControlEntry) accessControlEntries[accessControlEntries.Count - 2]).Index + 1;
      Touch ();
    }

    public DateTime ChangedAt
    {
      get { return (DateTime) DataContainer["ChangedAt"]; }
    }

    public void Touch ()
    {
      DataContainer["ChangedAt"] = DateTime.Now;
    }

    public int Index
    {
      get { return (int) DataContainer["Index"]; }
      set { DataContainer["Index"] = value; }
    }

    public SecurableClassDefinition Class
    {
      get { return (SecurableClassDefinition) GetRelatedObject ("Class"); }
      set { SetRelatedObject ("Class", value); }
    }

    public DomainObjectCollection StateCombinations
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("StateCombinations"); }
      set { } // marks property StateCombinations as modifiable
    }

    public DomainObjectCollection AccessControlEntries
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlEntries"); }
      set { } // marks property AccessControlEntries as modifiable
    }

    public AccessControlEntry[] FindMatchingEntries (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      List<AccessControlEntry> entries = new List<AccessControlEntry> ();

      foreach (AccessControlEntry entry in AccessControlEntries)
      {
        if (entry.MatchesToken (token))
          entries.Add (entry);
      }

      return entries.ToArray ();
    }

    public AccessControlEntry[] FilterAcesByPriority (AccessControlEntry[] aces)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("aces", aces);

      if (aces.Length == 0)
        return aces;

      AccessControlEntry[] sortedAces = (AccessControlEntry[]) aces.Clone ();

      Array.Sort (sortedAces, new AccessControlEntryPriorityComparer ());
      Array.Reverse (sortedAces);

      int highestPriority = sortedAces[0].ActualPriority;

      return Array.FindAll<AccessControlEntry> (
          sortedAces, 
          delegate (AccessControlEntry current) { return current.ActualPriority == highestPriority; });
    }

    public AccessTypeDefinition[] GetAccessTypes (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      List<AccessTypeDefinition> accessTypes = new List<AccessTypeDefinition> ();

      AccessControlEntry[] aces = FilterAcesByPriority (FindMatchingEntries (token));
      foreach (AccessControlEntry ace in aces)
      {
        foreach (AccessTypeDefinition allowedAccessType in ace.GetAllowedAccessTypes ())
        {
          if (!accessTypes.Contains (allowedAccessType))
            accessTypes.Add (allowedAccessType);
        }
      }

      return accessTypes.ToArray ();
    }

    //TODO: Rewrite with test

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _accessControlEntriesToBeDeleted = AccessControlEntries.Clone ();
      _stateCombinations = StateCombinations.Clone ();
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      foreach (AccessControlEntry accessControlEntry in _accessControlEntriesToBeDeleted)
        accessControlEntry.Delete ();
      _accessControlEntriesToBeDeleted = null;

      foreach (StateCombination stateCombination in _stateCombinations)
        stateCombination.Delete ();
      _stateCombinations = null;
    }

    public StateCombination CreateStateCombination ()
    {
      if (Class == null)
        throw new InvalidOperationException ("Cannot create StateCombination if no SecurableClassDefinition is assigned to this AccessControlList.");
      
      StateCombination stateCombination = new StateCombination (ClientTransaction);
      stateCombination.Class = Class;
      stateCombination.AccessControlList = this;

      return stateCombination;
    }

    public AccessControlEntry CreateAccessControlEntry ()
    {
      if (Class == null)
        throw new InvalidOperationException ("Cannot create AccessControlEntry if no SecurableClassDefinition is assigned to this AccessControlList.");

      AccessControlEntry accessControlEntry = new AccessControlEntry (ClientTransaction);
      foreach (AccessTypeDefinition accessTypeDefinition in Class.AccessTypes)
        accessControlEntry.AttachAccessType (accessTypeDefinition);
      accessControlEntry.AccessControlList = this;

      return accessControlEntry;
    }
  }
}
