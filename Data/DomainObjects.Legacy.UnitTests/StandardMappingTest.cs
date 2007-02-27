using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Database;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests
{
  public class StandardMappingTest : DatabaseTest
  {
    // types

    // static members and constants

    private const string c_createTestDataFileName = "CreateTestData.sql";

    private static readonly MappingConfiguration s_mappingConfiguration = MappingConfiguration.CreateConfigurationFromFileBasedLoader(@"Mapping.xml");

    // member fields

    private DomainObjectIDs _domainObjectIDs;

    // construction and disposing

    protected StandardMappingTest ()
      : base (new StandardMappingTestDataLoader (c_connectionString), c_createTestDataFileName)
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      TestMappingConfiguration.Reset ();

      _domainObjectIDs = new DomainObjectIDs ();
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return _domainObjectIDs; }
    }
  }
}
