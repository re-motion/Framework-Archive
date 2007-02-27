using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Location : TestDomainBase
  {
    // types

    // static members and constants

    public static new Location GetObject (ObjectID id)
    {
      return (Location) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Location ()
    {
    }

    public Location (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Location (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
      set { SetRelatedObject ("Client", value); }
    }
  }
}
