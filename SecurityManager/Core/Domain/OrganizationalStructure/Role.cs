using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;
using System.Collections.Generic;
using System.Security.Principal;
using Rubicon.Security;
using Rubicon.Collections;
using Rubicon.Security.Configuration;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Role")]
  public class Role : OrganizationalStructureObject
  {
    // types

    // static members and constants

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


    public List<Group> GetPossibleGroups (ObjectID clientID)
    {
      ArgumentUtility.CheckNotNull ("clientID", clientID);

      List<Group> groups = new List<Group> ();
      foreach (Group group in Group.FindByClientID (clientID, ClientTransaction))
        groups.Add (group);

      return FilterByAccess (groups, SecurityManagerAccessTypes.AssignRole);
    }

    public List<Position> GetPossiblePositions (Group group)
    {
      ArgumentUtility.CheckNotNull ("group", group);

      List<Position> positions = new List<Position> ();
      if (group.GroupType == null)
      {
        foreach (Position position in Position.FindAll (ClientTransaction))
          positions.Add (position);
      }
      else
      {
        foreach (GroupTypePosition groupTypePosition in group.GroupType.Positions)
        {
          if (!positions.Contains (groupTypePosition.Position))
            positions.Add (groupTypePosition.Position);
        }
      }

      return FilterByAccess (positions, SecurityManagerAccessTypes.AssignRole);
    }

    private List<T> FilterByAccess<T> (List<T> securableObjects, params Enum[] requiredAccessTypeEnums) where T : ISecurableObject
    {
      if (SecurityConfiguration.Current.SecurityService == null)
        return securableObjects;

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      AccessType[] requiredAccessTypes = Array.ConvertAll<Enum, AccessType> (requiredAccessTypeEnums, AccessType.Get);
      List<T> filteredObjects = new List<T> ();
      foreach (T securableObject in securableObjects)
      {
        if (securityClient.HasAccess (securableObject, requiredAccessTypes))
          filteredObjects.Add (securableObject);
      }

      return filteredObjects;
    }
  }
}
