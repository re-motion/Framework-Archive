using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Ceo : TestDomainBase
  {
    // types

    // static members and constants

    public static Ceo GetObject (ObjectID id)
    {
      return (Ceo) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    // CEOs can only be created within this assembly.
    internal Ceo ()
    {
    }

    protected Ceo (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer.GetValue ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public Company Company
    {
      get { return (Company) GetRelatedObject ("Company"); }
      set { SetRelatedObject ("Company", value); }
    }
  }
}
