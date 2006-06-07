using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.Security.Service.Domain.Globalization.OrganizationalStructure.GroupType")]
  public class GroupType : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new GroupType GetObject (ObjectID id)
    {
      return (GroupType) DomainObject.GetObject (id);
    }

    public static new GroupType GetObject (ObjectID id, bool includeDeleted)
    {
      return (GroupType) DomainObject.GetObject (id, includeDeleted);
    }

    public static new GroupType GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (GroupType) DomainObject.GetObject (id, clientTransaction);
    }

    public static new GroupType GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (GroupType) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public GroupType (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected GroupType (DataContainer dataContainer)
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

    public DomainObjectCollection Groups
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Groups"); }
      set { } // marks property Groups as modifiable
    }

    public DomainObjectCollection ConcretePositions
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("ConcretePositions"); }
      set { } // marks property ConcretePositions as modifiable
    }

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
      set { SetRelatedObject ("Client", value); }
    }

  }
}
