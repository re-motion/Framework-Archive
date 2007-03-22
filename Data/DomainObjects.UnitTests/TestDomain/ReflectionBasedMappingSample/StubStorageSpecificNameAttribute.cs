using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public class StubStorageSpecificNameAttribute: StorageSpecificNameAttribute
  {
    public StubStorageSpecificNameAttribute (string name)
        : base (name)
    {
    }
  }
}