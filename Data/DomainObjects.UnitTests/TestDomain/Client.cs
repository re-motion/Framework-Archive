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

    public static Client NewObject ()
    {
      return DomainObject.NewObject<Client> ().With();
    }

    protected Client ()
    {
    }

    protected Client (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public abstract Client ParentClient { get; set; }
  }
}
