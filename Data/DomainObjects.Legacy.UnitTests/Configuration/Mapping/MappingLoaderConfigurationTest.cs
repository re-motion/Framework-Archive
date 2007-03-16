using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Development.UnitTesting.Configuration;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
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
            <loader type=""Rubicon.Data.DomainObjects.Legacy::ConfigurationLoader.FileBasedConfigurationLoader.MappingLoader""/>
          </mapping>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (MappingLoader), _configuration.MappingLoader);
    }
  }
}