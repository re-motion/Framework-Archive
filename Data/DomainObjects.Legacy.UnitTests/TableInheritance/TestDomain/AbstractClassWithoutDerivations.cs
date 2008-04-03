using System;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public abstract class AbstractClassWithoutDerivations : DomainObject
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    protected AbstractClassWithoutDerivations ()
    {
    }

    protected AbstractClassWithoutDerivations (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public DomainBase DomainBase
    {
      get { return (DomainBase) GetRelatedObject ("DomainBase"); }
    }
  }
}
