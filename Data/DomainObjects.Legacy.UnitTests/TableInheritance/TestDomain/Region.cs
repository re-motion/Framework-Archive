using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class Region : DomainObject
  {
    // types

    // static members and constants

    public static new Region GetObject (ObjectID id)
    {
      return (Region) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Region ()
    {
    }

    protected Region (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer.GetValue ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public DomainObjectCollection Customers
    {
      get { return GetRelatedObjects ("Customers"); }
    }
  }
}
