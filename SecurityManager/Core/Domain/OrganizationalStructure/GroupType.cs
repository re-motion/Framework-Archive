using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Globalization;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Utilities;
using System.ComponentModel;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.GroupType")]
  [PermanentGuid ("BDBB9696-177B-4b73-98CF-321B2FBEAD0C")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class GroupType : OrganizationalStructureObject
  {
    public enum Methods
    {
      Search
    }

    public static GroupType NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<GroupType> ().With ();
      }
    }

    public static DomainObjectCollection FindAll (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.GroupType.FindAll");
      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    protected GroupType ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("GroupType")]
    public abstract ObjectList<Group> Groups { get; }

    [DBBidirectionalRelation ("GroupType")]
    public abstract ObjectList<GroupTypePosition> Positions { get; }

    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("SpecificGroupType")]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

    //TODO: UnitTests
    public override string DisplayName
    {
      get { return Name; }
    }
  }
}
