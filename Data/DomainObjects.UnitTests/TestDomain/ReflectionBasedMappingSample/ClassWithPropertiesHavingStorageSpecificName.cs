using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithPropertiesHavingStorageSpecificNameAttribute: TestDomainBase
  {
    protected ClassWithPropertiesHavingStorageSpecificNameAttribute (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract int NoAttribute { get; set; }

    [AutomaticProperty]
    [StubStorageSpecificName ("CustomName")]
    public abstract int StorageSpecificName { get; set; }

  }
}