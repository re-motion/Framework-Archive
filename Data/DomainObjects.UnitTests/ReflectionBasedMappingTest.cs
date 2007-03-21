using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.Database;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  public class ReflectionBasedMappingTest
  {
    [SetUp]
    public virtual void SetUp ()
    {
      DomainObjectsConfiguration.SetCurrent (null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      DomainObjectsConfiguration.SetCurrent (null);
    }
  }
}