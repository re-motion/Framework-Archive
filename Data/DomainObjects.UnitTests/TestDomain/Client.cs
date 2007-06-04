using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Client : TestDomainBase
  {
    public static Client NewObject ()
    {
      return NewObject<Client> ().With();
    }

    public new static Client GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Client> (id);
    }

    public new static Client GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return Client.GetObject (id);
      }
    }

    protected Client ()
    {
    }

    public abstract Client ParentClient { get; set; }
  }
}
