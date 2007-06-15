using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class GenericClassNotInMapping<T> : DomainObject
  {
    protected GenericClassNotInMapping ()
    {
    }
  }
}