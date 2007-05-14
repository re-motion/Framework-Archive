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

    public abstract int SupplierQuality { get; set; }
  }
}
