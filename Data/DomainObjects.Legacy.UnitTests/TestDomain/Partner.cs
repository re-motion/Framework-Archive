using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class Partner : Company
  {
    // types

    // static members and constants

    public static new Partner GetObject (ObjectID id)
    {
      return (Partner) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public Partner ()
    {
    }

    protected Partner (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public Person ContactPerson
    {
      get { return (Person) GetRelatedObject ("ContactPerson"); }
      set { SetRelatedObject ("ContactPerson", value); }
    }
  }
}
