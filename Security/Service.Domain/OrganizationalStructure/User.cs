using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.Security.Service.Domain.Globalization.OrganizationalStructure.User")]
  public class User : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new User GetObject (ObjectID id)
    {
      return (User) DomainObject.GetObject (id);
    }

    public static new User GetObject (ObjectID id, bool includeDeleted)
    {
      return (User) DomainObject.GetObject (id, includeDeleted);
    }

    public static new User GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (User) DomainObject.GetObject (id, clientTransaction);
    }

    public static new User GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (User) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public User ()
    {
    }

    public User (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected User (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public string Title
    {
      get { return (string) DataContainer["Title"]; }
      set { DataContainer["Title"] = value; }
    }

    public string FirstName
    {
      get { return (string) DataContainer["FirstName"]; }
      set { DataContainer["FirstName"] = value; }
    }

    public string LastName
    {
      get { return (string) DataContainer["LastName"]; }
      set { DataContainer["LastName"] = value; }
    }

    public string UserName
    {
      get { return (string) DataContainer["UserName"]; }
      set { DataContainer["UserName"] = value; }
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

    public Group Group
    {
      get { return (Group) GetRelatedObject ("Group"); }
      set { SetRelatedObject ("Group", value); }
    }

  }
}
