using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [TestDomain]
  public abstract class ClassWithMixedPropertiesNotInMapping : DomainObject
  {
    protected ClassWithMixedPropertiesNotInMapping (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    public abstract bool Boolean { get; set; }
  }
}