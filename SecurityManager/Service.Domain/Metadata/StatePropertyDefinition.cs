using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.Metadata
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

    public StatePropertyDefinition ()
    {
    }

    public StatePropertyDefinition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected StatePropertyDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public Guid MetadataItemID
    {
      get { return (Guid) DataContainer["MetadataItemID"]; }
      set { DataContainer["MetadataItemID"] = value; }
    }

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public Rubicon.Data.DomainObjects.DomainObjectCollection References
    {
      get { return (Rubicon.Data.DomainObjects.DomainObjectCollection) GetRelatedObjects ("References"); }
      set { } // marks property References as modifiable
    }

    public Rubicon.Data.DomainObjects.DomainObjectCollection DefinedStates
    {
      get { return (Rubicon.Data.DomainObjects.DomainObjectCollection) GetRelatedObjects ("DefinedStates"); }
      set { } // marks property DefinedStates as modifiable
    }

  }
}
