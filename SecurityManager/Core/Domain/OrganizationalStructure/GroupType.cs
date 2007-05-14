using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Data;
using System.ComponentModel;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.GroupType")]
  [PermanentGuid ("BDBB9696-177B-4b73-98CF-321B2FBEAD0C")]
  public class GroupType : OrganizationalStructureObject
  {
    // types

    public enum Methods
    {
      Search
    }
    
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
