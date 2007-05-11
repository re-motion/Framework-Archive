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
