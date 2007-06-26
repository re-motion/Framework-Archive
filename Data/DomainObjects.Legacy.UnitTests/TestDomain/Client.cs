using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Client : TestDomainBase
  {
    // types

    // static members and constants

    public static new Client GetObject (ObjectID id)
    {
      return (Client) DomainObject.GetObject (id);
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
