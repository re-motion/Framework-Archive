using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Position")]
  public class Position : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new Position GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Position) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Position GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Position) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    public static DomainObjectCollection FindByClientID (ObjectID clientID, ClientTransaction clientTransaction)
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Position.FindByClientID");

      query.Parameters.Add ("@clientID", clientID);

      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    //TODO: Rewrite with test
    public static DomainObjectCollection FindAll (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Position.FindAll");
      return transaction.QueryManager.GetCollection (query);
    }

    // member fields

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

    public DomainObjectCollection ConcretePositions
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("ConcretePositions"); }
      set { } // marks property ConcretePositions as modifiable
    }

    public DomainObjectCollection Roles
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Roles"); }
      set { } // marks property Roles as modifiable
    }

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
      set { SetRelatedObject ("Client", value); }
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
