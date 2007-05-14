using System;
using System.Collections.Generic;
using System.ComponentModel;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Globalization;
using Rubicon.Security;
using Rubicon.Utilities;
using Rubicon.Security.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.User")]
  [PermanentGuid ("759DA370-E2C4-4221-B878-BE378C916042")]
  public class User : OrganizationalStructureObject, ISecurableObject, IDomainObjectSecurityContextFactory
  {
    // types

    //public enum Methods
    //{
    //  Create
    //}

    // static members and constants

    public static User FindByUserName (ClientTransaction transaction, string userName)
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.User.FindByUserName");
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

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static User Create (ClientTransaction clientTransaction)
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateUser (clientTransaction);
    //}

    // member fields

    private DomainObjectSecurityStrategy _securityStrategy;

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

    [DemandPropertyWritePermission (SecurityManagerAccessTypes.AssignRole)]
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

    // Must not be private because PermissionReflection would not work with derived classes.
    [EditorBrowsable (EditorBrowsableState.Never)]
    protected DomainObjectCollection AccessControlEntries
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlEntries"); }
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

    //TODO: UnitTests
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

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      if (_securityStrategy == null)
        _securityStrategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, this);

      return _securityStrategy;
    }

    SecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      string owner = UserName;
      string ownerGroup = Group == null ? null : Group.UniqueIdentifier;
      string ownerClient = Client == null ? null : Client.UniqueIdentifier;

      return new SecurityContext (GetType (), owner, ownerGroup, ownerClient, GetStates(), GetAbstractRoles ());
    }

    protected virtual IDictionary<string, Enum> GetStates ()
    {
      return new Dictionary<string, Enum> ();
    }

    protected virtual IList<Enum> GetAbstractRoles ()
    {
      return new List<Enum> ();
    }

    bool IDomainObjectSecurityContextFactory.IsDiscarded
    {
      get { return IsDiscarded; }
    }

    bool IDomainObjectSecurityContextFactory.IsNew
    {
      get { return State == StateType.New; }
    }

    bool IDomainObjectSecurityContextFactory.IsDeleted
    {
      get { return State == StateType.Deleted; }
    }
  }
}
