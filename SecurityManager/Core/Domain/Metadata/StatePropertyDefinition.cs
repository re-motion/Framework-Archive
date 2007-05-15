using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  public abstract class StatePropertyDefinition : MetadataObject
  {
    // types

    // static members and constants

    public static StatePropertyDefinition NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<StatePropertyDefinition> ().With ();
      }
    }

    public static StatePropertyDefinition NewObject (ClientTransaction clientTransaction, Guid metadataItemID, string name)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<StatePropertyDefinition> ().With (metadataItemID, name);
      }
    }

    public static new StatePropertyDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (StatePropertyDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    protected StatePropertyDefinition ()
    {
    }

    protected StatePropertyDefinition (Guid metadataItemID, string name)
    {
      MetadataItemID = metadataItemID;
      Name = name;
    }

    // methods and properties

    //TODO: Rename to StatePropertyReferences
    [DBBidirectionalRelation ("StateProperty")]
    public abstract ObjectList<StatePropertyReference> References { get; }

    [DBBidirectionalRelation ("StateProperty", SortExpression = "[Index] ASC")]
    [Mandatory]
    public abstract ObjectList<StateDefinition> DefinedStates { get; }

    public StateDefinition GetState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      foreach (StateDefinition state in DefinedStates)
      {
        if (state.Name == name)
          return state;
      }

      throw new ArgumentException (string.Format ("The state '{0}' is not defined for the property '{1}'.", name, Name), "name");
    }

    public bool ContainsState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      foreach (StateDefinition state in DefinedStates)
      {
        if (state.Name == name)
          return true;
      }

      return false;
    }

    public StateDefinition GetState (int stateValue)
    {
      foreach (StateDefinition state in DefinedStates)
      {
        if (state.Value == stateValue)
          return state;
      }

      throw new ArgumentException (string.Format ("A state with the value {0} is not defined for the property '{1}'.", stateValue, Name), "stateValue");
    }

    public bool ContainsState (int stateValue)
    {
      foreach (StateDefinition state in DefinedStates)
      {
        if (state.Value == stateValue)
          return true;
      }

      return false;
    }

    [StorageClassNone]
    public StateDefinition this[string stateName]
    {
      get { return GetState (stateName); }
    }

    public void AddState (string stateName, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("stateName", stateName);

      StateDefinition newStateDefinition = StateDefinition.NewObject (ClientTransaction);
      newStateDefinition.Name = stateName;
      newStateDefinition.Value = value;
      newStateDefinition.Index = value;

      AddState (newStateDefinition);
    }

    public void AddState (StateDefinition newState)
    {
      ArgumentUtility.CheckNotNull ("newState", newState);
      DefinedStates.Add (newState);
    }
  }
}
