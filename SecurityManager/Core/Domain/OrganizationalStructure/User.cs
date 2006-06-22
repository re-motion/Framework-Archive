using System;
using System.Collections.Generic;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.User")]
  public class User : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static User Find (ClientTransaction transaction, string userName)
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.User.FindUser");
      query.Parameters.Add ("@userName", userName);

      DomainObjectCollection users = transaction.QueryManager.GetCollection (query);
      if (users.Count == 0)
        return null;

      return (User) users[0];
    }

    public static new User GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (User) DomainObject.GetObject (id, clientTransaction);
    }

    public static new User GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (User) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    public static DomainObjectCollection FindByClientID (ObjectID clientID, ClientTransaction clientTransaction)
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.User.FindByClientID");

      query.Parameters.Add ("@clientID", clientID);

      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    // member fields

    // construction and disposing

    protected internal User (ClientTransaction clientTransaction)
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

    public List<Role> GetRolesForGroup (Group group)
    {
      ArgumentUtility.CheckNotNull ("group", group);

      List<Role> roles = new List<Role>();

      foreach (Role role in Roles)
      {
        if (role.Group.ID == group.ID)
          roles.Add (role);
      }

      return roles;
    }

    public override string DisplayName
    {
      get { return GetFormattedName (); }
    }

    private string GetFormattedName ()
    {
      string formattedName = LastName;

      if (!StringUtility.IsNullOrEmpty (FirstName))
        formattedName += " " + FirstName;

      if (!StringUtility.IsNullOrEmpty (Title))
        formattedName += ", " + Title;

      return formattedName;
    }
  }
}
