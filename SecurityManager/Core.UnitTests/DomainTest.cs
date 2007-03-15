using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.SecurityManager.Persistence;
using Rubicon.SecurityManager.UnitTests.Configuration;

namespace Rubicon.SecurityManager.UnitTests
{
  public abstract class DomainTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    protected DomainTest()
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
      MappingConfiguration.SetCurrent (MappingConfiguration.CreateConfigurationFromFileBasedLoader (@"SecurityManagerMapping.xml"));
      QueryConfiguration.SetCurrent (new QueryConfiguration (@"SecurityManagerQueries.xml"));

      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
      providers.Add (
          new RdbmsProviderDefinition (
              "SecurityManager",
              typeof (SecurityManagerSqlProvider),
              "Integrated Security=SSPI;Initial Catalog=RubiconSecurityManager;Data Source=localhost"));
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration (providers, providers["SecurityManager"]);
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (persistenceConfiguration));
    }

    [SetUp]
    public virtual void SetUp()
    {
      ClientTransaction.SetCurrent (null);
    }
  }
}