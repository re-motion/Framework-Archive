using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Client : TestDomainBase
  {
    // types

    // static members and constants

    public static Client GetObject (ObjectID id)
    {
      return (Client) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public Client ()
    {
    }

    protected Client (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public Client ParentClient
    {
      get { return (Client) GetRelatedObject ("ParentClient"); }
      set { SetRelatedObject ("ParentClient", value); }
    }
  }
}
