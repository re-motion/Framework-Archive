using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class IndustrialSector : TestDomainBase
{
  // types

  // static members and constants

  public static new IndustrialSector GetObject (ObjectID id)
  {
    return (IndustrialSector) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public IndustrialSector ()
  {
  }

  protected IndustrialSector (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public string Name
  {
    get { return DataContainer.GetString ("Name"); }
    set { DataContainer["Name"] = value; }
  }

  public DomainObjectCollection Companies
  {
    get { return GetRelatedObjects ("Companies"); }
  }
}
}
