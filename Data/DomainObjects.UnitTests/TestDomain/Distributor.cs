using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class Distributor : Partner
{
  // types

  // static members and constants

  public static new Distributor GetObject (ObjectID id)
  {
    return (Distributor) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public Distributor ()
  {
  }

  protected Distributor (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public int NumberOfShops
  {
    get { return DataContainer.GetInt32 ("NumberOfShops"); }
    set { DataContainer["NumberOfShops"] = value; }
  }
}
}
