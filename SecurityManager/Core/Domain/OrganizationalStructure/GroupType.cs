using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Globalization;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.GroupType")]
  public class GroupType : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new GroupType GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (GroupType) DomainObject.GetObject (id, clientTransaction);
    }

    public static new GroupType GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (GroupType) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    public static DomainObjectCollection FindAll (ClientTransaction clientTransaction)
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.GroupType.FindAll");

      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    // member fields

    // construction and disposing

    public GroupType (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected GroupType (DataContainer dataContainer)
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

    public DomainObjectCollection Groups
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Groups"); }
      set { } // marks property Groups as modifiable
    }

    public DomainObjectCollection Positions
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Positions"); }
      set { } // marks property Positions as modifiable
    }

    private DomainObjectCollection AccessControlEntries
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlEntries"); }
    }

    //TODO: UnitTests
    public override string DisplayName
    {
      get { return Name; }
    }
  }
}
