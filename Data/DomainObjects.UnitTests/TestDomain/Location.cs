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

    public static Location NewObject ()
    {
      return DomainObject.NewObject<Location> ().With();
    }

    protected Location()
    {
    }

    protected Location (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [Mandatory]
    public abstract Client Client { get; set; }
  }
}
