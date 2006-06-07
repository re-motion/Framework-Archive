using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.Security.Service.Domain.Globalization.OrganizationalStructure.Group")]
  public class Group : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new Group GetObject (ObjectID id)
    {
      return (Group) DomainObject.GetObject (id);
    }

    public static new Group GetObject (ObjectID id, bool includeDeleted)
    {
      return (Group) DomainObject.GetObject (id, includeDeleted);
    }

    public static new Group GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Group) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Group GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Group) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    protected internal Group (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Group (DataContainer dataContainer)
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

    public string ShortName
    {
      get { return (string) DataContainer["ShortName"]; }
      set { DataContainer["ShortName"] = value; }
    }

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
      set { SetRelatedObject ("Client", value); }
    }

    public Group Parent
    {
      get { return (Group) GetRelatedObject ("Parent"); }
      set { SetRelatedObject ("Parent", value); }
    }

    public DomainObjectCollection Children
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Children"); }
      set { } // marks property Children as modifiable
    }

    public GroupType GroupType
    {
      get { return (GroupType) GetRelatedObject ("GroupType"); }
      set { SetRelatedObject ("GroupType", value); }
    }

    public DomainObjectCollection Roles
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Roles"); }
      set { } // marks property Roles as modifiable
    }

  }
}
