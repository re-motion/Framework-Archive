using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.SecurityClientTransactionExtensionTests
{
  public class BaseTest
  {
    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
      MappingConfiguration.SetCurrent (MappingConfiguration.CreateConfigurationFromFileBasedLoader (@"Rubicon.Security.Data.DomainObjects.UnitTests.Mapping.xml"));
      StorageProviderConfiguration.SetCurrent (StorageProviderConfiguration.CreateConfigurationFromFileBasedLoader (@"Rubicon.Security.Data.DomainObjects.UnitTests.StorageProviders.xml"));
      QueryConfiguration.SetCurrent (new QueryConfiguration (@"Rubicon.Security.Data.DomainObjects.UnitTests.Queries.xml"));
    }
  }
}