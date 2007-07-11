using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClosedGenericClassWithOneSideRelationProperties
      : GenericClassWithOneSideRelationPropertiesNotInMapping<ClosedGenericClassWithManySideRelationProperties>
  {
    protected ClosedGenericClassWithOneSideRelationProperties ()
    {
    }
  }
}