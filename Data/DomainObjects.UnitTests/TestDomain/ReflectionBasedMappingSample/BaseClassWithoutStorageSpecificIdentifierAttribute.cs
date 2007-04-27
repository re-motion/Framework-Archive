using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [TestDomain]
  public abstract class BaseClassWithoutStorageSpecificIdentifierAttribute : DomainObject
  {
    protected BaseClassWithoutStorageSpecificIdentifierAttribute ()
    {
    }

    protected BaseClassWithoutStorageSpecificIdentifierAttribute (DataContainer dataContainer)
      : base (dataContainer)
    {
    }
  }
}
