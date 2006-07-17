using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class StatePropertyDefinition : MetadataObject
  {
    // types

    // static members and constants

    public static new StatePropertyDefinition GetObject (ObjectID id)
    {
      return (StatePropertyDefinition) DomainObject.GetObject (id);
    }

    public static new StatePropertyDefinition GetObject (ObjectID id, bool includeDeleted)
    {
      return (StatePropertyDefinition) DomainObject.GetObject (id, includeDeleted);
    }

    public static new StatePropertyDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (StatePropertyDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new StatePropertyDefinition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (StatePropertyDefinition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public StatePropertyDefinition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public StatePropertyDefinition (ClientTransaction clientTransaction, Guid metadataItemID, string name)
      : base (clientTransaction)
    {
      DataContainer["MetadataItemID"] = metadataItemID;
      DataContainer["Name"] = name;
    }

    protected StatePropertyDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public DomainObjectCollection References
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("References"); }
      set { } // marks property References as modifiable
    }

    public DomainObjectCollection DefinedStates
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("DefinedStates"); }
      set { } // marks property DefinedStates as modifiable
    }

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

    public StateDefinition this[string stateName]
    {
      get { return GetState (stateName); }
    }

    public void AddState (string stateName, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("stateName", stateName);

      StateDefinition newStateDefinition = new StateDefinition (ClientTransaction);
      newStateDefinition.Name = stateName;
      newStateDefinition.Value = value;

      AddState (newStateDefinition);
    }

    public void AddState (StateDefinition newState)
    {
      ArgumentUtility.CheckNotNull ("newState", newState);
      DefinedStates.Add (newState);
    }
  }
}
