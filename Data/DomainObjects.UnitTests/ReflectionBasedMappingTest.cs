using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Database;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  public class ReflectionBasedMappingTest: DatabaseTest
  {
    private const string c_createTestDataFileName = "CreateTestData.sql";

    protected ReflectionBasedMappingTest()
        : base (new StandardMappingTestDataLoader (c_connectionString), c_createTestDataFileName)
    {
    }

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
      DomainObjectsConfiguration.SetCurrent (StandardConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      TestMappingConfiguration.Reset();
    }

    public override void SetUp()
    {
      base.SetUp();
      DomainObjectsConfiguration.SetCurrent (StandardConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return StandardConfiguration.Instance.GetDomainObjectIDs(); }
    }
  }
}