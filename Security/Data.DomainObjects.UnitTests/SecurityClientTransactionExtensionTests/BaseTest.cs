using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using NUnit.Framework;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.SecurityClientTransactionExtensionTests
{
  public class BaseTest
  {
    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
      MappingConfiguration.SetCurrent (new MappingConfiguration (@"Security.Data.DomainObjects.UnitTests.Mapping.xml"));
      StorageProviderConfiguration.SetCurrent (new StorageProviderConfiguration (@"Security.Data.DomainObjects.UnitTests.StorageProviders.xml"));
      QueryConfiguration.SetCurrent (new QueryConfiguration (@"Security.Data.DomainObjects.UnitTests.Queries.xml"));
    }
  }
}