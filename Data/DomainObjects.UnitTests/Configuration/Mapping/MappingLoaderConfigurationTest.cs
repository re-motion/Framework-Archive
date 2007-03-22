using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Development.UnitTesting.Configuration;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class MappingLoaderConfigurationTest
  {
    private MappingLoaderConfiguration _configuration;

    [SetUp]
    public void SetUp()
    {
      _configuration = new MappingLoaderConfiguration();
    }

    [Test]
    public void Deserialize_WithCustomMappingLoader()
    {
      string xmlFragment =
          @"<mapping>
            <loader type=""Rubicon.Data.DomainObjects.UnitTests::Configuration.Mapping.FakeMappingLoader""/>
          </mapping>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FakeMappingLoader), _configuration.MappingLoader);
    }

    [Test]
    public void Deserialize_WithDefaultMappingLoader ()
    {
      string xmlFragment = @"<mapping />";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (MappingReflector), _configuration.MappingLoader);
    }

    [Test]
    public void Deserialize_WithCustomDomainObjectFactory ()
    {
      string xmlFragment =
          @"<mapping>
            <domainObjectFactory type=""Rubicon.Data.DomainObjects.UnitTests::Configuration.Mapping.FakeDomainObjectFactory""/>
          </mapping>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FakeDomainObjectFactory), _configuration.DomainObjectFactory);
    }

    [Test]
    public void Deserialize_WithDefaultDomainObjectFactory()
    {
      string xmlFragment = @"<mapping />";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (DomainObjectFactory), _configuration.DomainObjectFactory);
    }
  }
}