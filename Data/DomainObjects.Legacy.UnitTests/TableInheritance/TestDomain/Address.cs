using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class Address : DomainObject
  {
    // types

    // static members and constants

    public static Address GetObject (ObjectID id)
    {
      return (Address) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public Address ()
    {
    }

    protected Address (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Street
    {
      get { return (string) DataContainer.GetValue ("Street"); }
      set { DataContainer.SetValue ("Street", value); }
    }

    public string Zip
    {
      get { return (string) DataContainer.GetValue ("Zip"); }
      set { DataContainer.SetValue ("Zip", value); }
    }

    public string City
    {
      get { return (string) DataContainer.GetValue ("City"); }
      set { DataContainer.SetValue ("City", value); }
    }

    public string Country
    {
      get { return (string) DataContainer.GetValue ("Country"); }
      set { DataContainer.SetValue ("Country", value); }
    }

    public Person Person
    {
      get { return (Person) GetRelatedObject ("Person"); }
      set { SetRelatedObject ("Person", value); }
    }
  }
}
