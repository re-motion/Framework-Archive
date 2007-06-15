using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithMixedPropertiesNotInMapping : DomainObject
  {
    protected ClassWithMixedPropertiesNotInMapping ()
    {
    }

    public abstract bool Boolean { get; set; }
  }
}