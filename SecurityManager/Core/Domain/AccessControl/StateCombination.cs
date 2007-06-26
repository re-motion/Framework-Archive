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
  public abstract class StateCombination : AccessControlObject
  {
    // types

    // static members and constants

     public static StateCombination NewObject (ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return NewObject<StateCombination> ().With ();
      }
    }

    // member fields

    private ObjectList<StateUsage> _stateUsagesToBeDeleted;

    // construction and disposing

    protected StateCombination ()
    {
      Initialize();
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
      StateUsages.Added += StateUsages_Added;
    }

    private void StateUsages_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      if (Class != null)
        Class.Touch ();
    }

    public abstract int Index { get; set; }

    [DBBidirectionalRelation ("StateCombinations")]
    [DBColumn ("SecurableClassID")]
    [Mandatory]
    public abstract SecurableClassDefinition Class { get; set; }

    [DBBidirectionalRelation ("StateCombination")]
    public abstract ObjectList<StateUsage> StateUsages { get; }

    [DBBidirectionalRelation ("StateCombinations")]
    [Mandatory]
    public abstract AccessControlList AccessControlList { get; set; }

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
      StateUsage usage = StateUsage.NewObject (ClientTransaction);
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

      _stateUsagesToBeDeleted = StateUsages.Clone ();
    }
    
    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      foreach (StateUsage stateUsage in _stateUsagesToBeDeleted)
        stateUsage.Delete ();
      _stateUsagesToBeDeleted = null;
    }
  }
}
