using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration
{
  [TestFixture]
  public class DomainObjectsConfigurationTest
  {
    [Test]
    public void GetAndSet()
    {
      IDomainObjectsConfiguration configuration = new FakeDomainObjectsConfiguration (new PersistenceConfiguration());
      DomainObjectsConfiguration.SetCurrent (configuration);
      
      Assert.AreSame (configuration, DomainObjectsConfiguration.Current);
    }

    [Test]
    public void Get ()
    {
      DomainObjectsConfiguration.SetCurrent (null);
      Assert.IsNotNull (DomainObjectsConfiguration.Current);
    }
  }
}