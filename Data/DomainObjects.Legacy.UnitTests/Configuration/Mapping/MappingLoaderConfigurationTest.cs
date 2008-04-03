using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
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
            <loader type=""Remotion.Data.DomainObjects.Legacy::ConfigurationLoader.XmlBasedConfigurationLoader.MappingLoader""/>
          </mapping>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (MappingLoader), _configuration.CreateMappingLoader());
    }
  }
}