using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.Security.Service.Domain.Globalization.OrganizationalStructure.Position")]
  public class Position : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new Position GetObject (ObjectID id)
    {
      return (Position) DomainObject.GetObject (id);
    }

    public static new Position GetObject (ObjectID id, bool includeDeleted)
    {
      return (Position) DomainObject.GetObject (id, includeDeleted);
    }

    public static new Position GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Position) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Position GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Position) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public Position ()
    {
    }

    public Position (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Position (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public DomainObjectCollection ConcretePositions
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("ConcretePositions"); }
      set { } // marks property ConcretePositions as modifiable
    }

    public DomainObjectCollection Roles
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Roles"); }
      set { } // marks property Roles as modifiable
    }

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
      set { SetRelatedObject ("Client", value); }
    }

  }
}
