using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class Person : DomainBase
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

    public DateTime DateOfBirth
    {
      get { return (DateTime) DataContainer.GetValue ("DateOfBirth"); }
      set { DataContainer.SetValue ("DateOfBirth", value); }
    }

    public Address Address
    {
      get { return (Address) GetRelatedObject ("Address"); }
    }

    public new void Delete ()
    {
      base.Delete ();
    }
  }
}
