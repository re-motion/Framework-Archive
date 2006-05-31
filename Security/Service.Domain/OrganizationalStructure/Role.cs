using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.Security.Service.Domain.Globalization.OrganizationalStructure.Role")]
  public class Role : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new Role GetObject (ObjectID id)
    {
      return (Role) DomainObject.GetObject (id);
    }

    public static new Role GetObject (ObjectID id, bool includeDeleted)
    {
      return (Role) DomainObject.GetObject (id, includeDeleted);
    }

    public static new Role GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Role) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Role GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Role) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public Role ()
    {
    }

    public Role (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Role (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public Group Group
    {
      get { return (Group) GetRelatedObject ("Group"); }
      set { SetRelatedObject ("Group", value); }
    }

    public Position Position
    {
      get { return (Position) GetRelatedObject ("Position"); }
      set { SetRelatedObject ("Position", value); }
    }

    public User User
    {
      get { return (User) GetRelatedObject ("User"); }
      set { SetRelatedObject ("User", value); }
    }

  }
}
