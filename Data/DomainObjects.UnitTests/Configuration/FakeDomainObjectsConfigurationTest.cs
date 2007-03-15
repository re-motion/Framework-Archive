using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration
{
  [TestFixture]
  public class FakeDomainObjectsConfigurationTest
  {
    [Test]
    public void GetStorage()
    {
      PersistenceConfiguration storage = new PersistenceConfiguration();
      IDomainObjectsConfiguration configuration = new FakeDomainObjectsConfiguration (storage);

      Assert.AreSame (storage, configuration.Storage);
    }
  }
}