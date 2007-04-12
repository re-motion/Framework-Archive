using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class Location : TestDomainBase
  {
    public static new Location GetObject (ObjectID id)
    {
      return (Location) DomainObject.GetObject (id);
    }

    public static Location Create ()
    {
      return DomainObject.Create<Location> ();
    }

    protected Location (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [Mandatory]
    public abstract Client Client { get; set; }
  }
}
