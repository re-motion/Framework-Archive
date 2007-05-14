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

    protected Location()
    {
    }

    [Mandatory]
    public abstract Client Client { get; set; }
  }
}
