using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class Supplier : Partner
  {
    // types

    // static members and constants

    public static new Supplier GetObject (ObjectID id)
    {
      return (Supplier) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public Supplier ()
    {
    }

    protected Supplier (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public int SupplierQuality
    {
      get { return (int) DataContainer.GetValue ("SupplierQuality"); }
      set { DataContainer.SetValue ("SupplierQuality", value); }
    }
  }
}
