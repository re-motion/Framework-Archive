using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration
{
  [TestFixture]
  public class FakeDomainObjectsConfigurationTest
  {
    [Test]
    public void Initialize()
    {
      PersistenceConfiguration storage = new PersistenceConfiguration ();
      MappingLoaderConfiguration mappingLoader = new MappingLoaderConfiguration ();
      IDomainObjectsConfiguration configuration = new FakeDomainObjectsConfiguration (mappingLoader, storage);
    
      Assert.AreSame (mappingLoader, configuration.MappingLoader);
      Assert.AreSame (storage, configuration.Storage);
    }
  }
}