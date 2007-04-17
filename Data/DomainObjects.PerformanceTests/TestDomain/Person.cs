using System;

using Rubicon.Data.DomainObjects;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
  public class Person : ClientBoundBaseClass
  {
    // types

    // static members and constants

    public static new Person GetObject (ObjectID id)
    {
      return (Person) DomainObject.GetObject (id);
    }

    public static new Person GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Person) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public Person ()
    {
    }

    public Person (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Person (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string FirstName
    {
      get { return (string) DataContainer.GetValue ("FirstName"); }
      set { DataContainer.SetValue ("FirstName", value); }
    }

    public string LastName
    {
      get { return (string) DataContainer.GetValue ("LastName"); }
      set { DataContainer.SetValue ("LastName", value); }
    }
  }
}
