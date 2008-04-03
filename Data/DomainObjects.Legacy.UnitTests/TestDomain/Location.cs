using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Location : TestDomainBase
  {
    // types

    // static members and constants

    public static Location GetObject (ObjectID id)
    {
      return (Location) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public Location ()
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
