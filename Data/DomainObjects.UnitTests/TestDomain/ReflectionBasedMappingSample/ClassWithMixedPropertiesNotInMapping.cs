using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [IgnoreForMapping]
  public abstract class ClassWithMixedPropertiesNotInMapping: TestDomainBase
  {
    protected ClassWithMixedPropertiesNotInMapping (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract bool Boolean { get; set; }
  }
}