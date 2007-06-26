using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Globalization;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Group")]
  [PermanentGuid ("AA1761A4-226C-4ebe-91F0-8FFF4974B175")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Group : OrganizationalStructureObject
  {
    // types

    public enum Methods
    {
      //Create
      Search
    }

    // TODO: Rewrite with test
    protected class GroupSecurityStrategy : ObjectSecurityStrategy
    {
      private Group _group;

      public GroupSecurityStrategy (Group group)
        : base (group)
      {
        ArgumentUtility.CheckNotNull ("group", group);

        _group = group;
      }

      public override bool HasAccess (ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes)
      {
        //TODO: if (!_group.IsDiscarded && _group.State == StateType.New)
        // Move ObjectSecurityAdapter into RPA and add IsDiscarded check.
        if (_group.IsDiscarded || _group.State == StateType.New)
          return true;

        return base.HasAccess (securityProvider, user, requiredAccessTypes);
      }
    }

    // static members and constants

    internal static Group NewObject (ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return NewObject<Group> ().With ();
      }
    }

    public static new Group GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return DomainObject.GetObject<Group> (id);
      }
    }

    public static DomainObjectCollection FindByTenantID (ObjectID tenantID, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Group.FindByTenantID");
      query.Parameters.Add ("@tenantID", tenantID);

      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    public static Group FindByUnqiueIdentifier (string uniqueIdentifier, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Group.FindByUnqiueIdentifier");
      query.Parameters.Add ("@uniqueIdentifier", uniqueIdentifier);

      DomainObjectCollection groups = clientTransaction.QueryManager.GetCollection (query);
      if (groups.Count == 0)
        return null;

      return (Group) groups[0];
    }

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static Group Create (ClientTransaction clientTransaction)
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateGroup (clientTransaction);
    //}

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    // member fields

    // construction and disposing

    protected Group ()
    {
      UniqueIdentifier = Guid.NewGuid ().ToString ();
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (MaximumLength = 10)]
    public abstract string ShortName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string UniqueIdentifier { get; set; }

    [Mandatory]
    public abstract Tenant Tenant { get; set; }

    [DBBidirectionalRelation ("Children")]
    public abstract Group Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<Group> Children { get; }

    [DBBidirectionalRelation ("Groups")]
    public abstract GroupType GroupType { get; set; }

    [DemandPropertyWritePermission (SecurityManagerAccessTypes.AssignRole)]
    [DBBidirectionalRelation ("Group")]
    public abstract ObjectList<Role> Roles { get; }

    // Must not be private because PermissionReflection would not work with derived classes.
    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("SpecificGroup")]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

    public override string DisplayName
    {
      get
      {
        if (StringUtility.IsNullOrEmpty (ShortName))
          return Name;
        else
          return string.Format ("{0} ({1})", ShortName, Name);
      }
    }

    // TODO: UnitTests
    public List<Group> GetPossibleParentGroups (ObjectID tenantID)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);

      List<Group> groups = new List<Group> ();

      foreach (Group group in FindByTenantID (tenantID, ClientTransaction))
      {
        if ((!Children.Contains (group.ID)) && (group.ID != this.ID))
          groups.Add (group);
      }
      return groups;
    }

    protected override string GetOwningTenant ()
    {
      return Tenant == null ? null : Tenant.UniqueIdentifier;
    }

    protected override string GetOwningGroup ()
    {
      return UniqueIdentifier;
    }
  }
}
