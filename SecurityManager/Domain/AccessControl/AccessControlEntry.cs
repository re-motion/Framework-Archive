using System;

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
  public class AccessControlEntry : AccessControlObject
  {
    // types

    // static members and constants

    public static new AccessControlEntry GetObject (ObjectID id)
    {
      return (AccessControlEntry) DomainObject.GetObject (id);
    }

    public static new AccessControlEntry GetObject (ObjectID id, bool includeDeleted)
    {
      return (AccessControlEntry) DomainObject.GetObject (id, includeDeleted);
    }

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

    public int Priority
    {
      get { return (int) DataContainer["Priority"]; }
      set { DataContainer["Priority"] = value; }
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
      get { return (DomainObjectCollection) GetRelatedObjects ("Permissions"); }
      set { } // marks property Permissions as modifiable
    }
  }
}
