using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  public abstract class AbstractClassNotInMapping : DomainObject
  {
    protected AbstractClassNotInMapping (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }
  }
}
