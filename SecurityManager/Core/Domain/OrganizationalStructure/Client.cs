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
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Client : OrganizationalStructureObject
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

    internal static Client NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<Client> ().With ();
      }
    }

    public static new Client GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Client) DomainObject.GetObject (id, clientTransaction);
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

    protected Client ()
    {
      UniqueIdentifier = Guid.NewGuid ().ToString ();
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string UniqueIdentifier { get; set; }

    public abstract bool IsAbstract { get; set; }

    [DBBidirectionalRelation ("Children")]
    public abstract Client Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<Client> Children { get; }

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
