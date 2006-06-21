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
  [MultiLingualResources ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Group")]
  public class Group : OrganizationalStructureObject
  {
    // types

    // static members and constants

    public static Group Find (ClientTransaction transaction, string name)
    {
      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Group.FindGroup");
      query.Parameters.Add ("@name", name);

      DomainObjectCollection groups = transaction.QueryManager.GetCollection (query);
      if (groups.Count == 0)
        return null;

      return (Group) groups[0];
    }

    public static new Group GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Group) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Group GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Group) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    public static DomainObjectCollection GetByClientID (ObjectID clientID)
    {
      ClientTransaction clientTransaction = new ClientTransaction ();

      Query query = new Query ("Rubicon.SecurityManager.Domain.OrganizationalStructure.Group.FindByClientID");

      query.Parameters.Add ("@clientID", clientID);

      return (DomainObjectCollection) clientTransaction.QueryManager.GetCollection (query);
    }

    // member fields

    // construction and disposing

    protected internal Group (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Group (DataContainer dataContainer)
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

    public string ShortName
    {
      get { return (string) DataContainer["ShortName"]; }
      set { DataContainer["ShortName"] = value; }
    }

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
      set { SetRelatedObject ("Client", value); }
    }

    public Group Parent
    {
      get { return (Group) GetRelatedObject ("Parent"); }
      set { SetRelatedObject ("Parent", value); }
    }

    public DomainObjectCollection Children
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Children"); }
      set { } // marks property Children as modifiable
    }

    public GroupType GroupType
    {
      get { return (GroupType) GetRelatedObject ("GroupType"); }
      set { SetRelatedObject ("GroupType", value); }
    }

    public DomainObjectCollection Roles
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Roles"); }
      set { } // marks property Roles as modifiable
    }

    public DomainObjectCollection AccessControlEntries
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("AccessControlEntries"); }
      set { } // marks property AccessControlEntries as modifiable
    }

    public override string DisplayName
    {
      get { return string.Format ("{0} ({1})", ShortName, Name); }
    }
  }
}
