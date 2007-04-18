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

    public static Supplier Create ()
    {
      return DomainObject.Create<Supplier> ();
    }
    
    protected Supplier (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    public abstract int SupplierQuality { get; set; }
  }
}
