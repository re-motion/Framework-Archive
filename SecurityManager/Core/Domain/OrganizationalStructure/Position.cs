using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Security;
using System.Collections.Generic;
using Rubicon;
using Rubicon.SecurityManager.Configuration;
using System.ComponentModel;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Position")]
  [PermanentGuid ("5BBE6C4D-DC88-4a27-8BFF-0AC62EE34333")]
  public class Position : OrganizationalStructureObject, ISecurableObject, ISecurityContextFactory
  {
    // types

    //public enum Methods
    //{
    //  Create
    //}

    // static members and constants

    public static new Position GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Position) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Position GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Position) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    public static DomainObjectCollection FindAll (ClientTransaction clientTransaction)
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Position.FindAll");

      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    [DemandMethodPermission (SecurityManagerAccessTypes.AssignRole)]
    [System.ComponentModel.EditorBrowsable (System.ComponentModel.EditorBrowsableState.Never)]
    public static void Dummy_AssignRole ()
    {
    }

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static Position Create (ClientTransaction clientTransaction)
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreatePosition (clientTransaction);
    //}

    // member fields

    private IObjectSecurityStrategy _securityStrategy;

    private DomainObjectCollection _accessControlEntriesToBeDeleted;
    private DomainObjectCollection _rolesToBeDeleted;
    private DomainObjectCollection _groupTypePositionsToBeDeleted;

    // construction and disposing

    protected internal Position (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Position (DataContainer dataContainer)
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

    [PermanentGuid ("5C31F600-88F3-4ff7-988C-0E45A857AB4B")]
    public Delegation Delegation
    {
      get { return (Delegation) DataContainer["Delegation"]; }
      set { DataContainer["Delegation"] = value; }
    }

    public DomainObjectCollection GroupTypes
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("GroupTypes"); }
      set { } // marks property GroupTypes as modifiable
    }

    // Must not be private because PermissionReflection would not work with derived classes.
    [EditorBrowsable (EditorBrowsableState.Never)]
    protected DomainObjectCollection Roles
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Roles"); }
    }

    // Must not be private because PermissionReflection would not work with derived classes.
    [EditorBrowsable (EditorBrowsableState.Never)]
    protected DomainObjectCollection AccessControlEntries
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlEntries"); }
    }

    public override string DisplayName
    {
      get { return Name; }
    }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _accessControlEntriesToBeDeleted = AccessControlEntries.Clone ();
      _rolesToBeDeleted = Roles.Clone ();
      _groupTypePositionsToBeDeleted = GroupTypes.Clone ();
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      foreach (AccessControlEntry accessControlEntry in _accessControlEntriesToBeDeleted)
        accessControlEntry.Delete ();
      _accessControlEntriesToBeDeleted = null;

      foreach (Role role in _rolesToBeDeleted)
        role.Delete ();
      _rolesToBeDeleted = null;

      foreach (GroupTypePosition groupTypePosition in _groupTypePositionsToBeDeleted)
        groupTypePosition.Delete ();
      _groupTypePositionsToBeDeleted = null;
    }

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      if (_securityStrategy == null)
        _securityStrategy = new ObjectSecurityStrategy (this);

      return _securityStrategy;
    }

    SecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      string owner = string.Empty;
      string ownerGroup = string.Empty;
      string ownerClient = string.Empty;

      return new SecurityContext (GetType (), owner, ownerGroup, ownerClient, GetStates (), GetAbstractRoles ());
    }

    protected virtual IDictionary<string, Enum> GetStates ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Delegation", Delegation);

      return states;
    }

    protected virtual IList<Enum> GetAbstractRoles ()
    {
      return new List<Enum> ();
    }
  }
}
