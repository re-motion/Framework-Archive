using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Globalization;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.GroupTypePosition")]
  [PermanentGuid ("E2BF5572-DDFF-4319-8824-B41653950860")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class GroupTypePosition : OrganizationalStructureObject
  {
    public static GroupTypePosition NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<GroupTypePosition> ().With ();
      }
    }

    protected GroupTypePosition ()
    {
    }

    [DBBidirectionalRelation ("Positions")]
    [Mandatory]
    public abstract GroupType GroupType { get; set; }

    [DBBidirectionalRelation ("GroupTypes")]
    [Mandatory]
    public abstract Position Position { get; set; }

    //TODO: UnitTests
    public override string DisplayName
    {
      get
      {
        string groupTypeName = (GroupType != null) ? GroupType.Name : string.Empty;
        string positionName = (Position != null) ? Position.Name : string.Empty;

        return string.Format ("{0} / {1}", groupTypeName, positionName); 
      }
    }
  }
}
