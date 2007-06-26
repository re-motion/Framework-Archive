using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class SpecialOfficial : Official
  {
    // types

    // static members and constants

    public static new SpecialOfficial GetObject (ObjectID id)
    {
      return (SpecialOfficial) DomainObject.GetObject (id);
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
