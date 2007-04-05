using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_AbstractClassWithoutDerivations")]
  [TestDomain]
  public abstract class AbstractClassWithoutDerivations : DomainObject
  {
    protected AbstractClassWithoutDerivations (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
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