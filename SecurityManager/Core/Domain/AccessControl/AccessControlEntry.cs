using System;
using System.Collections.Generic;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.AccessControl.AccessControlEntry")]
  public class AccessControlEntry : AccessControlObject
  {
    // types

    // static members and constants

    public const int UserPriority = 8;
    public const int AbstractRolePriority = 4;
    public const int GroupPriority = 2;
    public const int ClientPriority = 1;

    public static new AccessControlEntry GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (AccessControlEntry) DomainObject.GetObject (id, clientTransaction);
    }

    public static new AccessControlEntry GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (AccessControlEntry) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public AccessControlEntry (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected AccessControlEntry (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public ClientSelection Client
    {
      get { return (ClientSelection) DataContainer["Client"]; }
      set { DataContainer["Client"] = value; }
    }

    public GroupSelection Group
    {
      get { return (GroupSelection) DataContainer["Group"]; }
      set { DataContainer["Group"] = value; }
    }

    public UserSelection User
    {
      get { return (UserSelection) DataContainer["User"]; }
      set { DataContainer["User"] = value; }
    }

    public NaInt32 Priority
    {
      get { return (NaInt32) DataContainer["Priority"]; }
      set { DataContainer["Priority"] = value; }
    }

    public int ActualPriority
    {
      get
      {
        if (Priority.IsNull)
          return CalculatePriority ();

        return Priority.Value;
      }
    }

    public Group SpecificGroup
    {
      get { return (Group) GetRelatedObject ("SpecificGroup"); }
      set { SetRelatedObject ("SpecificGroup", value); }
    }

    public GroupType SpecificGroupType
    {
      get { return (GroupType) GetRelatedObject ("SpecificGroupType"); }
      set { SetRelatedObject ("SpecificGroupType", value); }
    }

    public Position SpecificPosition
    {
      get { return (Position) GetRelatedObject ("SpecificPosition"); }
      set { SetRelatedObject ("SpecificPosition", value); }
    }

    public User SpecificUser
    {
      get { return (User) GetRelatedObject ("SpecificUser"); }
      set { SetRelatedObject ("SpecificUser", value); }
    }

    public AbstractRoleDefinition SpecificAbstractRole
    {
      get { return (AbstractRoleDefinition) GetRelatedObject ("SpecificAbstractRole"); }
      set { SetRelatedObject ("SpecificAbstractRole", value); }
    }

    public AccessControlList AccessControlList
    {
      get { return (AccessControlList) GetRelatedObject ("AccessControlList"); }
      set { SetRelatedObject ("AccessControlList", value); }
    }

    public DomainObjectCollection Permissions
    {
      get { return new DomainObjectCollection (GetPermissions(), true); }
    }

    public bool MatchesToken (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      if (!MatchesAbstractRole (token))
        return false;

      if (!MatchesUserOrPosition (token))
        return false;

      return true;
    }

    public AccessTypeDefinition[] GetAllowedAccessTypes ()
    {
      List<AccessTypeDefinition> allowedAccessTypes = new List<AccessTypeDefinition>();

      foreach (Permission permission in Permissions)
      {
        if (permission.Allowed.IsTrue)
          allowedAccessTypes.Add (permission.AccessType);
      }

      return allowedAccessTypes.ToArray ();
    }

    public void AttachAccessType (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      if (FindPermission (accessType) != null)
        throw new ArgumentException (string.Format ("The access type '{0}' has already been attached to this access control entry.", accessType.Name), "accessType");

      Permission permission = new Permission (ClientTransaction);
      permission.AccessType = accessType;
      permission.Allowed = NaBoolean.Null;
      permission.Index = Permissions.Count;

      GetPermissions ().Add (permission);
    }

    public void AllowAccess (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      Permission permission = GetPermission (accessType);
      permission.Allowed = NaBoolean.True;
    }

    public void RemoveAccess (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      Permission permission = GetPermission (accessType);
      permission.Allowed = NaBoolean.Null;
    }

    private bool MatchesAbstractRole (SecurityToken token)
    {
      if (SpecificAbstractRole == null)
        return true;

      foreach (AbstractRoleDefinition role in token.AbstractRoles)
      {
        if (role.ID == SpecificAbstractRole.ID)
          return true;
      }

      return false;
    }

    private bool MatchesUserOrPosition (SecurityToken token)
    {
      switch (User)
      {
        case UserSelection.All:
          return true;

        case UserSelection.SpecificPosition:
          return token.ContainsRoleForPosition (SpecificPosition);

        default:
          return false;
      }
    }

    private Permission GetPermission (AccessTypeDefinition accessType)
    {
      Permission permission = FindPermission (accessType);
      if (permission == null)
        throw new ArgumentException (string.Format ("The access type '{0}' is not assigned to this access control entry.", accessType.Name), "accessType");

      return permission;
    }

    private Permission FindPermission (AccessTypeDefinition accessType)
    {
      foreach (Permission permission in Permissions)
      {
        if (permission.AccessType.ID == accessType.ID)
          return permission;
      }

      return null;
    }

    private int CalculatePriority ()
    {
      int priority = 0;

      if (User != UserSelection.All)
        priority += UserPriority;

      if (SpecificAbstractRole != null)
        priority += AbstractRolePriority;

      if (Group != GroupSelection.All)
        priority += GroupPriority;

      if (Client != ClientSelection.All)
        priority += ClientPriority;

      return priority;
    }

    private DomainObjectCollection GetPermissions ()
    {
      return GetRelatedObjects ("Permissions");
    }

    //TODO: Rewrite with test
    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      while (Permissions.Count > 0)
        Permissions[0].Delete ();
    }
  }
}
