using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [TestDomain]
  public class ClassNotInMapping : DomainObject
  {
    public ClassNotInMapping ()
      : base ()
    {
    }

    protected ClassNotInMapping (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }
  }
}
