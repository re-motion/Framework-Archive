using System;
using System.Collections.Specialized;
using System.Configuration;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Development.UnitTesting;
using Rubicon.Development.UnitTesting.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsProviderDefinitionTest : ReflectionBasedMappingTest
  {
    private StorageProviderDefinition _definition;

    public override void SetUp()
    {
      base.SetUp();

      _definition = new RdbmsProviderDefinition ("StorageProviderID", typeof (SqlProvider), "ConnectionString");

      FakeConfigurationWrapper configurationWrapper = new FakeConfigurationWrapper();
      configurationWrapper.SetUpConnectionString ("SqlProvider", "ConnectionString", null);
      ConfigurationWrapper.SetCurrent (configurationWrapper);
    }

    [Test]
    public void Initialize_FromArguments()
    {
      RdbmsProviderDefinition provider = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreSame (typeof (SqlProvider), provider.StorageProviderType);
      Assert.AreEqual ("ConnectionString", provider.ConnectionString);
      Assert.IsNotNull (provider.TypeConversionProvider);
    }

    [Test]
    public void Initialize_FromConfig()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");
      config.Add ("providerType", "Rubicon.Data.DomainObjects::Persistence.Rdbms.SqlProvider");
      config.Add ("connectionString", "SqlProvider");

      RdbmsProviderDefinition provider = new RdbmsProviderDefinition ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
      Assert.AreSame (typeof (SqlProvider), provider.StorageProviderType);
      Assert.AreEqual ("ConnectionString", provider.ConnectionString);
      Assert.IsEmpty (config);
      Assert.IsNotNull (provider.TypeConversionProvider);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The attribute 'providerType' is missing in the configuration of the 'Provider' provider.")]
    public void Initialize_FromConfig_WithMissingProviderType()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");
      config.Add ("connectionString", "SqlProvider");

      Dev.Null = new RdbmsProviderDefinition ("Provider", config);
    }

    [Test]
    public void IsIdentityTypeSupportedFalse()
    {
      Assert.IsFalse (_definition.IsIdentityTypeSupported (typeof (int)));
    }

    [Test]
    public void IsIdentityTypeSupportedTrue()
    {
      Assert.IsTrue (_definition.IsIdentityTypeSupported (typeof (Guid)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void IsIdentityTypeSupportedNull()
    {
      _definition.IsIdentityTypeSupported (null);
    }

    [Test]
    public void CheckValidIdentityType()
    {
      _definition.CheckIdentityType (typeof (Guid));
    }

    [Test]
    [ExpectedException (typeof (IdentityTypeNotSupportedException),
        ExpectedMessage = "The StorageProvider 'Rubicon.Data.DomainObjects.Persistence.Rdbms.SqlProvider' does not support identity values of type 'System.String'.")]
    public void CheckInvalidIdentityType()
    {
      _definition.CheckIdentityType (typeof (string));
    }

    [Test]
    public void CheckDetailsOfInvalidIdentityType()
    {
      try
      {
        _definition.CheckIdentityType (typeof (string));
        Assert.Fail ("Test expects an IdentityTypeNotSupportedException.");
      }
      catch (IdentityTypeNotSupportedException ex)
      {
        Assert.AreEqual (typeof (SqlProvider), ex.StorageProviderType);
        Assert.AreEqual (typeof (string), ex.InvalidIdentityType);
      }
    }
  }
}