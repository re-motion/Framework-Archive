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

    // member fields
	
    // construction and disposing

    protected DomainTest ()
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      MappingConfiguration.SetCurrent (new MappingConfiguration (@"SecurityManagerMapping.xml", @"mapping.xsd"));
      StorageProviderConfiguration.SetCurrent (new StorageProviderConfiguration (@"SecurityManagerStorageProviders.xml", @"storageProviders.xsd"));
    }

    [SetUp]
    public virtual void SetUp ()
    {
      ClientTransaction.SetCurrent (null);
    }
  }
}
