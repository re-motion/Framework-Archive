using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.Sql
{
  [TestFixture]
  public class SqlFileBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private RdbmsProviderDefinition _firstStorageProviderDefinition;
    private RdbmsProviderDefinition _secondStorageProviderDefinition;
    private SqlFileBuilder _sqlFileBuilder;
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;

    // construction and disposing

    public SqlFileBuilderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _firstStorageProviderDefinition = (RdbmsProviderDefinition) StorageProviderConfiguration.StorageProviderDefinitions.GetMandatory ("FirstStorageProvider");
      _secondStorageProviderDefinition = (RdbmsProviderDefinition) StorageProviderConfiguration.StorageProviderDefinitions.GetMandatory ("SecondStorageProvider");
      _sqlFileBuilder = new SqlFileBuilder (MappingConfiguration, _firstStorageProviderDefinition);
      _firstStorageProviderSetupDBScript = File.ReadAllText (@"SetupDB_FirstStorageProvider.sql").Replace ("\r\n", "\n");
      _secondStorageProviderSetupDBScript = File.ReadAllText (@"SetupDB_SecondStorageProvider.sql").Replace ("\r\n", "\n");
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (MappingConfiguration, _sqlFileBuilder.MappingConfiguration);
      Assert.AreSame (_firstStorageProviderDefinition, _sqlFileBuilder.RdbmsProviderDefinition);
    }

    [Test]
    public void Classes ()
    {
      ClassDefinitionCollection classes = _sqlFileBuilder.Classes;

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
    public void GetDatabaseName ()
    {
      Assert.AreEqual ("CodeGeneratorUnitTests1", _sqlFileBuilder.GetDatabaseName ());
    }

    [Test]
    public void GetScriptForSecondStorageProvider ()
    {
      SqlFileBuilder sqlFileBuilder = new SqlFileBuilder (MappingConfiguration, _secondStorageProviderDefinition);
      string expectedScript = File.ReadAllText (@"SetupDB_SecondStorageProvider.sql").Replace ("\r\n", "\n");

      Assert.AreEqual (expectedScript, sqlFileBuilder.GetScript ());
    }

    [Test]
    public void GetScriptForFirstStorageProvider ()
    {
      Assert.AreEqual (_firstStorageProviderSetupDBScript, _sqlFileBuilder.GetScript ());
    }

    [Test]
    public void GetFileName ()
    {
      string actualFileName = SqlFileBuilder.GetFileName (_firstStorageProviderDefinition, @"c:\SomeDirectory", false);
      Assert.AreEqual (@"c:\SomeDirectory\SetupDB.sql", actualFileName);
    }

    [Test]
    public void GetFileNameWithMultipleStorageProviderTrue ()
    {
      string actualFileName = SqlFileBuilder.GetFileName (_firstStorageProviderDefinition, @"c:\SomeDirectory", true);
      Assert.AreEqual (@"c:\SomeDirectory\SetupDB_FirstStorageProvider.sql", actualFileName);
    }

    [Test]
    public void BuildWithStorageProviderDefinition ()
    {
      DeleteOnDemand ("SetupDB_FirstStorageProvider.sql");

      SqlFileBuilder.Build (MappingConfiguration, _firstStorageProviderDefinition, "SetupDB_FirstStorageProvider.sql");

      Assert.IsTrue (File.Exists ("SetupDB_FirstStorageProvider.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScript, File.ReadAllText ("SetupDB_FirstStorageProvider.sql"));
    }

    [Test]
    public void BuildWithMappingConfiguration ()
    {
      if (!Directory.Exists ("TestDirectory"))
        Directory.CreateDirectory ("TestDirectory");

      DeleteOnDemand (@"TestDirectory\SetupDB_FirstStorageProvider.sql");
      DeleteOnDemand (@"TestDirectory\SetupDB_SecondStorageProvider.sql");
      DeleteOnDemand (@"TestDirectory\SetupDB_NonRdbmsStorageProvider.sql");

      SqlFileBuilder.Build (MappingConfiguration, StorageProviderConfiguration, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_FirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB_SecondStorageProvider.sql"));
      Assert.AreEqual (_secondStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB_SecondStorageProvider.sql"));
      Assert.IsFalse (File.Exists (@"TestDirectory\SetupDB_NonRdbmsStorageProvider.sql"));
    }

    [Test]
    public void BuildWithMappingConfigurationCreatesOutputDirectory ()
    {
      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);

      SqlFileBuilder.Build (MappingConfiguration, StorageProviderConfiguration, "TestDirectory");

      Assert.IsTrue (Directory.Exists (@"TestDirectory"));
    }

    [Test]
    public void BuildWithMappingConfigurationWithOneStorageProviderDefinition ()
    {
      if (!Directory.Exists ("TestDirectory"))
        Directory.CreateDirectory ("TestDirectory");

      DeleteOnDemand (@"TestDirectory\SetupDB.sql");

      StorageProviderConfiguration storageProviderConfiguration = new StorageProviderConfiguration ("emptyStorageProviders.xml");
      storageProviderConfiguration.StorageProviderDefinitions.Add (_firstStorageProviderDefinition);

      SqlFileBuilder.Build (MappingConfiguration, storageProviderConfiguration, "TestDirectory");

      Assert.IsTrue (File.Exists (@"TestDirectory\SetupDB.sql"));
      Assert.AreEqual (_firstStorageProviderSetupDBScript, File.ReadAllText (@"TestDirectory\SetupDB.sql"));
    }

    [Test]
    public void BuildWithMappingConfigurationWithEmptyOutputDirectory ()
    {
      DeleteOnDemand (@"SetupDB_FirstStorageProvider.sql");
      DeleteOnDemand (@"SetupDB_SecondStorageProvider.sql");

      SqlFileBuilder.Build (MappingConfiguration, StorageProviderConfiguration, string.Empty);

      Assert.IsTrue (File.Exists (@"SetupDB_FirstStorageProvider.sql"));
      Assert.IsTrue (File.Exists (@"SetupDB_SecondStorageProvider.sql"));
    }

    private void DeleteOnDemand (string fileName)
    {
      if (File.Exists (fileName))
        File.Delete (fileName);
    }
  }
}
