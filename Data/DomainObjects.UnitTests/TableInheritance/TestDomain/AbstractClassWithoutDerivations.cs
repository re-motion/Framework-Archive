using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_AbstractClassWithoutDerivations")]
  [TableInheritanceTestDomain]
  public abstract class AbstractClassWithoutDerivations : DomainObject
  {
    protected AbstractClassWithoutDerivations ()
    {
    }

    protected AbstractClassWithoutDerivations (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("AbstractClassesWithoutDerivations")]
    public abstract DomainBase DomainBase { get; }
  }
}