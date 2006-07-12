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
  public class StateCombination : AccessControlObject
  {
    // types

    // static members and constants

    public static new StateCombination GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (StateCombination) DomainObject.GetObject (id, clientTransaction);
    }

    public static new StateCombination GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (StateCombination) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public StateCombination (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected StateCombination (DataContainer dataContainer)
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

    public DomainObjectCollection StateUsages
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("StateUsages"); }
      set { } // marks property StateUsages as modifiable
    }

    public AccessControlList AccessControlList
    {
      get { return (AccessControlList) GetRelatedObject ("AccessControlList"); }
      set { SetRelatedObject ("AccessControlList", value); }
    }

    public bool MatchesStates (List<StateDefinition> states)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("states", states);

      if (StateUsages.Count == 0 && states.Count > 0)
        return false;

      foreach (StateUsage usage in StateUsages)
      {
        if (!states.Contains (usage.StateDefinition))
          return false;
      }

      return true;
    }

    public void AttachState (StateDefinition state)
    {
      StateUsage usage = new StateUsage (ClientTransaction);
      usage.StateDefinition = state;
      StateUsages.Add (usage);
    }

    public List<StateDefinition> GetStates ()
    {
      List<StateDefinition> states = new List<StateDefinition> ();

      foreach (StateUsage stateUsage in StateUsages)
        states.Add (stateUsage.StateDefinition);

      return states;
    }

    //TODO: Rewrite with test
    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      while (StateUsages.Count > 0)
        StateUsages[0].Delete ();
    }
  }
}
