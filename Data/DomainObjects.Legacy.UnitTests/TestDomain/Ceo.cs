using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Ceo : TestDomainBase
  {
    // types

    // static members and constants

    public static new Ceo GetObject (ObjectID id)
    {
      return (Ceo) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    // CEOs can only be created within this assembly.
    internal Ceo ()
    {
    }

    // CEOs can only be created within this assembly.
    internal Ceo (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Ceo (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return DataContainer.GetString ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public Company Company
    {
      get { return (Company) GetRelatedObject ("Company"); }
      set { SetRelatedObject ("Company", value); }
    }
  }
}
