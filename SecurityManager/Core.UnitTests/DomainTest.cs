using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.SecurityManager.UnitTests
{
  public abstract class DomainTest
  {
    // types

    // static members and constants

    // member fields
	
    // construction and disposing

    protected DomainTest ()
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      MappingConfiguration.SetCurrent (new MappingConfiguration (@"SecurityManagerMapping.xml"));
      StorageProviderConfiguration.SetCurrent (new StorageProviderConfiguration (@"SecurityManagerStorageProviders.xml"));
      QueryConfiguration.SetCurrent (new QueryConfiguration (@"SecurityManagerQueries.xml"));
    }

    [SetUp]
    public virtual void SetUp ()
    {
      ClientTransaction.SetCurrent (null);
    }
  }
}
