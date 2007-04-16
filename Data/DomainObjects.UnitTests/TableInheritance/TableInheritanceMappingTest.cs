using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Database;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  public class TableInheritanceMappingTest: DatabaseTest
  {
    private const string c_createTestDataFileName = "CreateTableInheritanceTestData.sql";

    public TableInheritanceMappingTest()
        : base (new TestDataLoader (c_connectionString), c_createTestDataFileName)
    {
    }

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
      DomainObjectsConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
    }

    public override void SetUp()
    {
      base.SetUp();
      DomainObjectsConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetDomainObjectsConfiguration ());
      MappingConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetMappingConfiguration ());
      ClientTransaction.SetCurrent (null);
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return TableInheritanceConfiguration.Instance.GetDomainObjectIDs (); }
    }
  }
}