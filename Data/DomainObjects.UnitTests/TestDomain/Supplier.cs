using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class Supplier : Partner
{
  // types

  // static members and constants

  public static new Supplier GetObject (ObjectID id)
  {
    return (Supplier) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public Supplier ()
  {
  }

  public Supplier (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected Supplier (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public int SupplierQuality
  {
    get { return DataContainer.GetInt32 ("SupplierQuality"); }
    set { DataContainer.SetValue ("SupplierQuality", value); }
  }
}
}
