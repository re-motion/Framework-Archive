using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [NotAbstract]
  public abstract class Supplier : Partner
  {
    public static new Supplier GetObject (ObjectID id)
    {
      return (Supplier) DomainObject.GetObject (id);
    }

    public new static Supplier NewObject ()
    {
      return DomainObject.NewObject<Supplier> ().With();
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
