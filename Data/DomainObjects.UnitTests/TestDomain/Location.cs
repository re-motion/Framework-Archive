using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Location : TestDomainBase
  {
    public static Location NewObject ()
    {
      return NewObject<Location> ().With();
    }

    public new static Location GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Location> (id);
    }

    public new static Location GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return Location.GetObject (id);
      }
    }

    protected Location()
    {
    }

    [Mandatory]
    public abstract Client Client { get; set; }
  }
}
