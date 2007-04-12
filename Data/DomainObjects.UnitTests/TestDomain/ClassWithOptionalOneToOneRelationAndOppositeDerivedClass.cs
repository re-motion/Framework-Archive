using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithOptionalOneToOneRelationAndOppositeDerivedClass")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithOptionalOneToOneRelationAndOppositeDerivedClass : TestDomainBase
  {
    protected ClassWithOptionalOneToOneRelationAndOppositeDerivedClass (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    public abstract Company Company { get; set; }
  }
}
