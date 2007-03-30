using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public abstract class AbstractClassNotInMapping : TestDomainBase
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AbstractClassNotInMapping ()
    {
    }

    public AbstractClassNotInMapping (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected AbstractClassNotInMapping (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
