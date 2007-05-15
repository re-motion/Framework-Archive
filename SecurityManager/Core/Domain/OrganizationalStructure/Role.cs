using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Globalization;
using Rubicon.Security;
using Rubicon.Utilities;
using Rubicon.Data;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Role")]
  [PermanentGuid ("23C68C62-5B0F-4857-8DF2-C161C0077745")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Role : OrganizationalStructureObject
  {
    public static Role NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<Role> ().With ();
      }
    }

    protected Role ()
    {
    }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    public abstract Group Group { get; set; }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    public abstract Position Position { get; set; }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    public abstract User User { get; set; }


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

    protected override string GetOwningClient ()
    {
      if (User != null)
        return User.Client.UniqueIdentifier;

      if (Group != null)
        return Group.Client.UniqueIdentifier;

      return null;
    }
  }
}
