using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithNSideRelationProperties: TestDomainBase
  {
    protected ClassWithNSideRelationProperties (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract ClassWithOneSideRelationProperties NoAttribute { get; set; }

    [AutomaticProperty]
    [Mandatory]
    public abstract ClassWithOneSideRelationProperties NotNullable { get; set; }
  }
}