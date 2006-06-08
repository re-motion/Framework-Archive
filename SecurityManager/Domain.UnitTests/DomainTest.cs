using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.SecurityManager.Domain.UnitTests
{
  public abstract class DomainTest
  {
    // types

    // static members and constants

    private static readonly MappingConfiguration s_mappingConfiguration = 
        new MappingConfiguration (@"SecurityManagerMapping.xml", @"mapping.xsd");
    private static readonly StorageProviderConfiguration s_storageProviderConfiguration =
        new StorageProviderConfiguration (@"SecurityManagerStorageProviders.xml", @"storageProviders.xsd");

    // member fields
	
    // construction and disposing

    protected DomainTest ()
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      StorageProviderConfiguration.SetCurrent (s_storageProviderConfiguration);
    }

    [SetUp]
    public virtual void SetUp ()
    {
      ClientTransaction.SetCurrent (null);
    }
  }
}
