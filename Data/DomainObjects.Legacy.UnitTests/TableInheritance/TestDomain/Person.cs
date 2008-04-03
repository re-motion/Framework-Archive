using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class Person : DomainBase
  {
    // types

    // static members and constants

    public static Person GetObject (ObjectID id)
    {
      return (Person) RepositoryAccessor.GetObject (id, false);
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
