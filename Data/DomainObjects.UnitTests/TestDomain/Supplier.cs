using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class Supplier : Partner
  {
    public new static Supplier NewObject ()
    {
      return NewObject<Supplier> ().With();
    }

    protected Supplier()
    {
    }

    protected Supplier (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract int SupplierQuality { get; set; }
  }
}
