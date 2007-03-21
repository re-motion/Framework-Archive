using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithNSideRelationProperties: TestDomainBase
  {
    protected ClassWithNSideRelationProperties (ObjectID objectID)
    {
    }

    [AutomaticProperty]
    public abstract ClassWithOneSideRelationProperties NoAttribute { get; set; }
  }
}