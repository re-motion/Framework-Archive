using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithOptionalOneToOneRelationAndOppositeDerivedClass")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithOptionalOneToOneRelationAndOppositeDerivedClass : TestDomainBase
  {
    protected ClassWithOptionalOneToOneRelationAndOppositeDerivedClass ()
    {
    }

    protected ClassWithOptionalOneToOneRelationAndOppositeDerivedClass (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public abstract Company Company { get; set; }
  }
}
