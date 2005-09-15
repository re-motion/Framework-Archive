using System;

using Rubicon.Data.DomainObjects;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
public class Client : DomainObject
{
  // types

  // static members and constants

  public static new Client GetObject (ObjectID id)
  {
    return (Client) DomainObject.GetObject (id);
  }

  public static new Client GetObject (ObjectID id, bool includeDeleted)
  {
    return (Client) DomainObject.GetObject (id, includeDeleted);
  }

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

  public Client ()
  {
  }

  public Client (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected Client (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public string Name
  {
    get { return (string) DataContainer["Name"]; }
    set { DataContainer["Name"] = value; }
  }

  public DomainObjectCollection Files
  {
    get { return (DomainObjectCollection) GetRelatedObjects ("Files"); }
  }

}
}
