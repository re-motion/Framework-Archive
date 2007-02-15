using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Globalization;

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

    // member fields

    // construction and disposing

    public Client (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
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

  }
}
