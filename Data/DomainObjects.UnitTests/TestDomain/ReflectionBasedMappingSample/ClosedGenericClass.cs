using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClosedGenericClass: GenericClassNotInMapping<int>
  {
    protected ClosedGenericClass ()
    {
    }
  }
}
