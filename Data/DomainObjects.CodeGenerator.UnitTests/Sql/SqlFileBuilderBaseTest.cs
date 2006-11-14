using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql;
using Rhino.Mocks;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.Sql
{
  //TODO: Run the generated SQL File against a database in the UnitTests and integrate this into the build
  //      Derive ClassWithAllDataTypes from an abstract class to ensure that all data types are selected in a UNION
  [TestFixture]
  public class SqlFileBuilderBaseTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private RdbmsProviderDefinition _firstStorageProviderDefinition;
    private RdbmsProviderDefinition _secondStorageProviderDefinition;
    private SqlFileBuilderBase _mockSqlFileBuilder;
    private MockRepository _mocks;
    private string _setupDBFileName;

    // construction and disposing

    public SqlFileBuilderBaseTest ()
    {
    }

    // methods and properties

    public override void TextFixtureSetUp ()
    {
      base.TextFixtureSetUp ();

      CleanUpTestData ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _mocks = new MockRepository ();
      _firstStorageProviderDefinition = (RdbmsProviderDefinition) StorageProviderConfiguration.StorageProviderDefinitions.GetMandatory ("FirstStorageProvider");
      _secondStorageProviderDefinition = (RdbmsProviderDefinition) StorageProviderConfiguration.StorageProviderDefinitions.GetMandatory ("SecondStorageProvider");
      _mockSqlFileBuilder = _mocks.DynamicMock<SqlFileBuilderBase> (MappingConfiguration, _firstStorageProviderDefinition);
      _setupDBFileName = @"TestDirectory\SetupDB_FirstStorageProvider.sql";
    }

    public override void TearDown ()
    {
      base.TearDown ();

      CleanUpTestData ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (MappingConfiguration, _mockSqlFileBuilder.MappingConfiguration);
      Assert.AreSame (_firstStorageProviderDefinition, _mockSqlFileBuilder.RdbmsProviderDefinition);
    }

    [Test]
    public void Classes ()
    {
      ClassDefinitionCollection classes = _mockSqlFileBuilder.Classes;

      Assert.IsNotNull (classes);
      Assert.AreEqual (16, classes.Count);
      Assert.IsTrue (classes.Contains (CeoClass));
      Assert.IsTrue (classes.Contains (CompanyClass));
      Assert.IsTrue (classes.Contains (CustomerClass));
      Assert.IsTrue (classes.Contains (PartnerClass));
      Assert.IsTrue (classes.Contains (MappingConfiguration.ClassDefinitions.GetMandatory ("DevelopmentPartner")));
      Assert.IsTrue (classes.Contains (AbstractWithoutConcreteClass));
      Assert.IsTrue (classes.Contains (MappingConfiguration.ClassDefinitions.GetMandatory ("Address")));
      Assert.IsTrue (classes.Contains (OrderClass));
      Assert.IsTrue (classes.Contains (OrderItemClass));
      Assert.IsTrue (classes.Contains (MappingConfiguration.ClassDefinitions.GetMandatory ("ClassWithAllDataTypes")));
      Assert.IsTrue (classes.Contains (MappingConfiguration.ClassDefinitions.GetMandatory ("Employee")));
      Assert.IsTrue (classes.Contains (ClassWithRelations));
      Assert.IsTrue (classes.Contains (MappingConfiguration.ClassDefinitions.GetMandatory ("ConcreteClass")));
      Assert.IsTrue (classes.Contains (DerivedClass));
      Assert.IsTrue (classes.Contains (SecondDerivedClass));
      Assert.IsTrue (classes.Contains (DerivedOfDerivedClass));
    }

    [Test]
    public void GetFileName ()
    {
      string actualFileName = SqlFileBuilderBase.GetFileName (_firstStorageProviderDefinition, @"c:\SomeDirectory", false);
      Assert.AreEqual (@"c:\SomeDirectory\SetupDB.sql", actualFileName);
    }

    [Test]
    public void GetFileNameWithMultipleStorageProviderTrue ()
    {
      string actualFileName = SqlFileBuilderBase.GetFileName (_firstStorageProviderDefinition, @"c:\SomeDirectory", true);
      Assert.AreEqual (@"c:\SomeDirectory\SetupDB_FirstStorageProvider.sql", actualFileName);
    }

    [Test]
    public void BuildWithStorageProviderDefinition ()
    {
      Directory.CreateDirectory ("TestDirectory");

      SqlFileBuilderBase.Build (typeof (SqlFileBuilderMock), MappingConfiguration, _firstStorageProviderDefinition, _setupDBFileName);

      Assert.IsTrue (File.Exists (_setupDBFileName));
      Assert.AreEqual ("Contents of SetupDB for StorageProvider\r\n  FirstStorageProvider", File.ReadAllText (_setupDBFileName));
    }

    [Test]
    public void BuildWithMappingConfigurationCreatesOutputDirectory ()
    {
      SqlFileBuilderBase.Build (typeof (SqlFileBuilderMock), MappingConfiguration, StorageProviderConfiguration, "TestDirectory");

      Assert.IsTrue (Directory.Exists (@"TestDirectory"));
    }

    [Test]
    public void BuildWithMappingConfigurationWithEmptyOutputDirectory ()
    {
      SqlFileBuilderBase.Build (typeof (SqlFileBuilderMock), MappingConfiguration, StorageProviderConfiguration, string.Empty);

      Assert.IsTrue (File.Exists (@"SetupDB_FirstStorageProvider.sql"));
      Assert.AreEqual ("Contents of SetupDB for StorageProvider\r\n  FirstStorageProvider", File.ReadAllText (@"SetupDB_FirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"SetupDB_SecondStorageProvider.sql"));
      Assert.AreEqual ("Contents of SetupDB for StorageProvider\r\n  SecondStorageProvider", File.ReadAllText (@"SetupDB_SecondStorageProvider.sql"));
    }

    [Test]
    public void BuildWithMappingConfigurationWithOneStorageProviderDefinition ()
    {
      StorageProviderConfiguration storageProviderConfiguration = new StorageProviderConfiguration ("emptyStorageProviders.xml");
      storageProviderConfiguration.StorageProviderDefinitions.Add (_firstStorageProviderDefinition);

      SqlFileBuilderBase.Build (typeof (SqlFileBuilderMock), MappingConfiguration, storageProviderConfiguration, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB.sql"));
      Assert.AreEqual ("Contents of SetupDB for StorageProvider\r\n  FirstStorageProvider", File.ReadAllText (@"TestDirectory\SetupDB.sql"));
    }

    [Test]
    public void BuildWithMappingConfiguration ()
    {
      SqlFileBuilderBase.Build (typeof (SqlFileBuilderMock), MappingConfiguration, StorageProviderConfiguration, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
      Assert.AreEqual ("Contents of SetupDB for StorageProvider\r\n  FirstStorageProvider", File.ReadAllText (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SecondStorageProvider.sql"));
      Assert.AreEqual ("Contents of SetupDB for StorageProvider\r\n  SecondStorageProvider", File.ReadAllText (@"TestDirectory\SetupDB_SecondStorageProvider.sql"));
      Assert.IsFalse (File.Exists (@"TestDirectory\SetupDB_NonRdbmsStorageProvider.sql"));
    }

    private void CleanUpTestData ()
    {
      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);

      if (File.Exists (@"SetupDB_FirstStorageProvider.sql"))
        File.Delete (@"SetupDB_FirstStorageProvider.sql");

      if (File.Exists (@"SetupDB_SecondStorageProvider.sql"))
        File.Delete (@"SetupDB_SecondStorageProvider.sql");
    }
  }
}
