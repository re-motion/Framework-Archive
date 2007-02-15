using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Globalization;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.GroupTypePosition")]
  public class GroupTypePosition : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new GroupTypePosition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (GroupTypePosition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new GroupTypePosition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (GroupTypePosition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public GroupTypePosition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected GroupTypePosition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public GroupType GroupType
    {
      get { return (GroupType) GetRelatedObject ("GroupType"); }
      set { SetRelatedObject ("GroupType", value); }
    }

    public Position Position
    {
      get { return (Position) GetRelatedObject ("Position"); }
      set { SetRelatedObject ("Position", value); }
    }

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
