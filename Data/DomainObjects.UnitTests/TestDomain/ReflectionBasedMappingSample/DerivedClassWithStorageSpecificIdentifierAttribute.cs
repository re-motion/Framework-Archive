using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [Instantiable]
  public abstract class DerivedClassWithStorageSpecificIdentifierAttribute : BaseClassWithoutStorageSpecificIdentifierAttribute
  {
    protected DerivedClassWithStorageSpecificIdentifierAttribute()
    {
    }

    protected DerivedClassWithStorageSpecificIdentifierAttribute (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}