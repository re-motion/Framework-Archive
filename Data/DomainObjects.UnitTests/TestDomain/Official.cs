using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class Official : TestDomainBase
{
  // types

  // static members and constants

  public static new Official GetObject (ObjectID id)
  {
    return (Official) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public Official ()
  {
  }

  protected Official (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public string Name
  {
    get { return DataContainer.GetString ("Name"); }
    set { DataContainer["Name"] = value; }
  }

  public DomainObjectCollection Orders
  {
    get { return GetRelatedObjects ("Orders"); }
  }
}
}
