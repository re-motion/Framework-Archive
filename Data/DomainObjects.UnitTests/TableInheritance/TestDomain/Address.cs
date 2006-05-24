using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public class Address : DomainObject
  {
    // types

    // static members and constants

    public static new Address GetObject (ObjectID id)
    {
      return (Address) DomainObject.GetObject (id);
    }

    public static new Address GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Address) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public Address ()
    {
    }

    public Address (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected Address (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Street
    {
      get { return DataContainer.GetString ("Street"); }
      set { DataContainer.SetValue ("Street", value); }
    }

    public string Zip
    {
      get { return DataContainer.GetString ("Zip"); }
      set { DataContainer.SetValue ("Zip", value); }
    }

    public string City
    {
      get { return DataContainer.GetString ("City"); }
      set { DataContainer.SetValue ("City", value); }
    }

    public string Country
    {
      get { return DataContainer.GetString ("Country"); }
      set { DataContainer.SetValue ("Country", value); }
    }

    public Person Person
    {
      get { return (Person) GetRelatedObject ("Person"); }
      set { SetRelatedObject ("Person", value); }
    }
  }
}
