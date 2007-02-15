using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public class Client : DomainObject
  {
    // types

    // static members and constants

    public static new Client GetObject (ObjectID id)
    {
      return (Client) DomainObject.GetObject (id);
    }

    public static new Client GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Client) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public Client ()
    {
    }

    public Client (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected Client (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public DomainObjectCollection AssignedObjects
    {
      get { return GetRelatedObjects ("AssignedObjects"); }
    }

    public string Name
    {
      get { return DataContainer.GetString ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }
  }
}
