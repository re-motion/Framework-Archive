using System;

using Rubicon.Data.DomainObjects;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
public class File : DomainObject
{
  // types

  // static members and constants

  public static new File GetObject (ObjectID id)
  {
    return (File) DomainObject.GetObject (id);
  }

  public static new File GetObject (ObjectID id, bool includeDeleted)
  {
    return (File) DomainObject.GetObject (id, includeDeleted);
  }

  public static new File GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (File) DomainObject.GetObject (id, clientTransaction);
  }

  public static new File GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    return (File) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  // member fields

  // construction and disposing

  public File ()
  {
  }

  public File (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected File (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public string Number
  {
    get { return (string) DataContainer["Number"]; }
    set { DataContainer["Number"] = value; }
  }

  public Client Client
  {
    get { return (Client) GetRelatedObject ("Client"); }
    set { SetRelatedObject ("Client", value); }
  }

}
}
