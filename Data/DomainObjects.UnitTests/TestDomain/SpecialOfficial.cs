using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [NotAbstract]
  public abstract class SpecialOfficial : Official
  {
    public static SpecialOfficial Create ()
    {
      return DomainObject.Create<SpecialOfficial>();
    }

    protected SpecialOfficial (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}
