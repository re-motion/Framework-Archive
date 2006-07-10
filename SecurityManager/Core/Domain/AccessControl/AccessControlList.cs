using System;
using System.Collections.Generic;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

using Rubicon.SecurityManager.Domain.Metadata;

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

    // construction and disposing

    public AccessControlList (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected AccessControlList (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

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

      List<AccessControlEntry> filteredAces = new List<AccessControlEntry> ();

      int highestPriority = sortedAces[0].ActualPriority;

      for (int i = 0; i < sortedAces.Length && sortedAces[i].ActualPriority == highestPriority; i++)
        filteredAces.Add (sortedAces[i]);

      return filteredAces.ToArray ();
    }

    public AccessTypeDefinition[] GetAccessTypes (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      AccessControlEntry[] aces = FilterAcesByPriority (FindMatchingEntries (token));

      List<AccessTypeDefinition> accessTypes = new List<AccessTypeDefinition> ();

      foreach (AccessControlEntry ace in aces)
      {
        AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes ();

        foreach (AccessTypeDefinition allowedAccessType in allowedAccessTypes)
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

      while (AccessControlEntries.Count > 0)
        AccessControlEntries[0].Delete ();

      while (StateCombinations.Count > 0)
        StateCombinations[0].Delete ();
    }

    public StateCombination CreateStateCombination ()
    {
      StateCombination stateCombination = new StateCombination (ClientTransaction);
      stateCombination.Class = Class;
      stateCombination.AccessControlList = this;

      return stateCombination;
    }

    public AccessControlEntry CreateAccessControlEntry ()
    {
      AccessControlEntry accessControlEntry = new AccessControlEntry (ClientTransaction);
      foreach (AccessTypeDefinition accessTypeDefinition in Class.AccessTypes)
      {
        Permission permission = new Permission (ClientTransaction);
        permission.AccessType = accessTypeDefinition;
        permission.AccessControlEntry = accessControlEntry;
      }
      accessControlEntry.AccessControlList = this;

      return accessControlEntry;
    }
  }
}
