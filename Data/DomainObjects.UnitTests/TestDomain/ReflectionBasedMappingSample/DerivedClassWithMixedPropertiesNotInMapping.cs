using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [IgnoreForMapping]
  public abstract class DerivedClassWithMixedPropertiesNotInMapping: ClassWithMixedProperties
  {
    protected DerivedClassWithMixedPropertiesNotInMapping (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract DateTime DateTime { get; set; }
  }
}