using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class Client : TestDomainBase
{
  // types

  // static members and constants

  public static new Client GetObject (ObjectID id)
  {
    return (Client) DomainObject.GetObject (id);
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

  public Client ParentClient
  {
    get { return (Client) GetRelatedObject ("ParentClient"); }
    set { SetRelatedObject ("ParentClient", value); }
  }
}
}
