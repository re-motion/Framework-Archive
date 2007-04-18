using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [TestDomain]
  public abstract class ClassWithMixedPropertiesNotInMapping : DomainObject
  {
    protected ClassWithMixedPropertiesNotInMapping ()
    {
    }

    protected ClassWithMixedPropertiesNotInMapping (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public abstract bool Boolean { get; set; }
  }
}