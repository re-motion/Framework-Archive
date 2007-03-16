using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Development.UnitTesting.Configuration;

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
    public void Deserialize_WithDefaultMappingLoader()
    {
      string xmlFragment = @"<mapping />";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (MappingReflector), _configuration.MappingLoader);
    }
  }
}