using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  public abstract class AbstractClassNotInMapping : DomainObject
  {
    public static AbstractClassNotInMapping Create ()
    {
      return DomainObject.Create<AbstractClassNotInMapping> ();
    }

    protected AbstractClassNotInMapping (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }
  }
}
