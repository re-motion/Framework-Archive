using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Official : TestDomainBase
  {
    // types

    // static members and constants

    public static Official GetObject (ObjectID id)
    {
      return (Official) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public Official ()
    {
    }

    protected Official (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer.GetValue ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public DomainObjectCollection Orders
    {
      get { return GetRelatedObjects ("Orders"); }
    }
  }
}
