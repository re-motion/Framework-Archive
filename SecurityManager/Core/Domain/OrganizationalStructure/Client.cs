using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Globalization;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Client")]
  public class Client : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static new Client GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Client) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Client GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Client) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    public static Client FindByUnqiueIdentifier (ClientTransaction clientTransaction, string uniqueIdentifier)
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Client.FindByUnqiueIdentifier");
      query.Parameters.Add ("@uniqueIdentifier", uniqueIdentifier);

      DomainObjectCollection clients = clientTransaction.QueryManager.GetCollection (query);
      if (clients.Count == 0)
        return null;

      return (Client) clients[0];
    }

    // member fields

    // construction and disposing

    public Client (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
      UniqueIdentifier = Guid.NewGuid ().ToString ();
    }

    protected Client (DataContainer dataContainer)
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

    public string UniqueIdentifier
    {
      get { return (string) DataContainer["UniqueIdentifier"]; }
      set { DataContainer["UniqueIdentifier"] = value; }
    }
  }
}
