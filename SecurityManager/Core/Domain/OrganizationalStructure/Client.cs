using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Globalization;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;
using System.Runtime.Remoting.Messaging;
using Rubicon.Data;
using Rubicon.Security;
using System.ComponentModel;
using System.Collections.Generic;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Client")]
  [PermanentGuid ("BD8FB1A4-E300-4663-AB1E-D6BD7B106619")]
  public class Client : OrganizationalStructureObject
  {
    // constants

    // types

    public enum Methods
    {
      Search
    }

    // static members

    private static readonly string s_currentKey = typeof (Client).AssemblyQualifiedName + "_Current";

    public static Client Current
    {
      get { return (Client) CallContext.GetData (s_currentKey); }
      set { CallContext.SetData (s_currentKey, value); }
    }

    public static new Client GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Client) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Client GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Client) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    public static DomainObjectCollection FindAll (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Client.FindAll");
      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    public static Client FindByUnqiueIdentifier (string uniqueIdentifier, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Client.FindByUnqiueIdentifier");
      query.Parameters.Add ("@uniqueIdentifier", uniqueIdentifier);

      DomainObjectCollection clients = clientTransaction.QueryManager.GetCollection (query);
      if (clients.Count == 0)
        return null;

      return (Client) clients[0];
    }

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
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

    public bool IsAbstract
    {
      get { return (bool) DataContainer["IsAbstract"]; }
      set { DataContainer["IsAbstract"] = value; }
    }

    public Client Parent
    {
      get { return (Client) GetRelatedObject ("Parent"); }
      set { SetRelatedObject ("Parent", value); }
    }

    public DomainObjectCollection Children
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Children"); }
      set { } // marks property Children as modifiable
    }

    public override string DisplayName
    {
      get { return Name; }
    }

    protected override string GetOwningClient ()
    {
      return UniqueIdentifier;
    }

    // TODO: UnitTests
    public List<Client> GetPossibleParentClients (ObjectID clientID)
    {
      List<Client> clients = new List<Client> ();

      foreach (Client client in Client.FindAll (ClientTransaction))
      {
        if ((!Children.Contains (client.ID)) && (client.ID != this.ID))
          clients.Add (client);
      }
      return clients;
    }

    public DomainObjectCollection GetHierachy ()
    {
      DomainObjectCollection clients = new DomainObjectCollection ();
      clients.Add (this);
      foreach (Client client in Children)
        clients.Combine (client.GetHierachy ());

      return clients;
    }
  }
}
