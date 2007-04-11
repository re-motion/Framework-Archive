using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithMixedPropertiesNotInMapping : ReflectionBasedMappingTestDomainBase
  {
    protected ClassWithMixedPropertiesNotInMapping (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract bool Boolean { get; set; }
  }
}