using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Globalization;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.User")]
  [PermanentGuid ("759DA370-E2C4-4221-B878-BE378C916042")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class User : OrganizationalStructureObject
  {
    public enum Methods
    {
      //Create
      Search
    }

    private static readonly string s_currentKey = typeof (User).AssemblyQualifiedName + "_Current";

    public static User Current
    {
      get { return (User) CallContext.GetData (s_currentKey); }
      set { CallContext.SetData (s_currentKey, value); }
    }

    internal static User NewObject (ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return NewObject<User> ().With ();
      }
    }

    public static new User GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return DomainObject.GetObject<User> (id);
      }
    }

    public static User FindByUserName (string userName, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.User.FindByUserName");
      query.Parameters.Add ("@userName", userName);

      DomainObjectCollection users = clientTransaction.QueryManager.GetCollection (query);
      if (users.Count == 0)
        return null;

      return (User) users[0];
    }

    public static DomainObjectCollection FindByTenantID (ObjectID tenantID, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.User.FindByTenantID");
      query.Parameters.Add ("@tenantID", tenantID);

      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static User Create (ClientTransaction clientTransaction)
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateUser (clientTransaction);
    //}

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    protected User ()
    {
    }

    [StringProperty (MaximumLength = 100)]
    public abstract string Title { get; set; }

    [StringProperty (MaximumLength = 100)]
    public abstract string FirstName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string LastName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string UserName { get; set; }

    [DemandPropertyWritePermission (SecurityManagerAccessTypes.AssignRole)]
    [DBBidirectionalRelation ("User")]
    public abstract ObjectList<Role> Roles { get; }

    [Mandatory]
    public abstract Tenant Tenant { get; set; }

    [Mandatory]
    public abstract Group OwningGroup { get; set; }

    // Must not be private because PermissionReflection would not work with derived classes.
    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("SpecificUser")]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

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

    protected override string GetOwner ()
    {
      return UserName;
    }

    protected override string GetOwningTenant ()
    {
      return Tenant == null ? null : Tenant.UniqueIdentifier;
    }

    protected override string GetOwningGroup ()
    {
      return OwningGroup == null ? null : OwningGroup.UniqueIdentifier;
    }
  }
}
