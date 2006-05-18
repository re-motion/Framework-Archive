using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public class Person : DomainBase
  {
    // types

    // static members and constants

    public static new Person GetObject (ObjectID id)
    {
      return (Person) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Person ()
    {
    }

    public Person (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected Person (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string FirstName
    {
      get { return DataContainer.GetString ("FirstName"); }
      set { DataContainer.SetValue ("FirstName", value); }
    }

    public string LastName
    {
      get { return DataContainer.GetString ("LastName"); }
      set { DataContainer.SetValue ("LastName", value); }
    }

    public DateTime DateOfBirth
    {
      get { return DataContainer.GetDateTime ("DateOfBirth"); }
      set { DataContainer.SetValue ("DateOfBirth", value); }
    }
  }
}
