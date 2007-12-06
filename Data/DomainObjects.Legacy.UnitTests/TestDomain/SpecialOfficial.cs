using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class SpecialOfficial : Official
  {
    // types

    // static members and constants

    public static new SpecialOfficial GetObject (ObjectID id)
    {
      return (SpecialOfficial) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public SpecialOfficial ()
    {
    }

    protected SpecialOfficial (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
