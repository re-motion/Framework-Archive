using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class Person : TestDomainBase
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

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public Partner AssociatedPartnerCompany
    {
      get { return (Partner) GetRelatedObject ("AssociatedPartnerCompany"); }
      set { SetRelatedObject ("AssociatedPartnerCompany", value); }
    }
  }
}
