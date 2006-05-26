using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
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

    protected AbstractClassWithoutDerivations (ClientTransaction clientTransaction)
      : base (clientTransaction)
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
