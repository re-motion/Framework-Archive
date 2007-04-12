using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class Client : TestDomainBase
  {
    public static new Client GetObject (ObjectID id)
    {
      return (Client) DomainObject.GetObject (id);
    }

    public static Client Create ()
    {
      return DomainObject.Create<Client> ();
    }

    protected Client (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    public abstract Client ParentClient { get; set; }
  }
}
