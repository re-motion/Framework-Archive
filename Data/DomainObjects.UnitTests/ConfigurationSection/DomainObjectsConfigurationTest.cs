using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.ConfigurationSection
{
  [TestFixture]
  public class DomainObjectsConfigurationTest
  {
    private DomainObjectsConfiguration _domainObjectsConfiguration;

    [SetUp]
    public void SetUp ()
    {
    }

    [Test]
    public void Initialize_Stub()
    {
      PersistenceConfiguration storage = new PersistenceConfiguration();
      DomainObjectsConfiguration domainObjectsConfiguration = new StubDomainObjectsConfiguration (storage);
    }
  }
}