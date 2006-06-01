using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using NUnit.Framework;

using Rubicon.Security.Service.Domain.Configuration;
using Rubicon.Security.Service.Domain.OrganizationalStructure;

namespace Rubicon.Security.Service.Domain.UnitTests.Configuration
{
  [TestFixture]
  public class SecurityDomainConfigurationTest
  {
    private TestSecurityDomainConfiguration _configuration;

    [SetUp]
    public void SetUp ()
    {
      _configuration = new TestSecurityDomainConfiguration ();
    }

    [Test]
    public void DeserializeWithDefaultFactory ()
    {
      string xmlFragment = @"<rubicon.security.service.domain />";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (OrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    public void DeserializeWithNamespace ()
    {
      string xmlFragment = @"<rubicon.security.service.domain xmlns=""http://www.rubicon-it.com/Security/Service/Domain/Configuration"" />";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (OrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    public void DeserializeWithCustomFactory ()
    {
      string xmlFragment = @"
          <rubicon.security.service.domain xmlns=""http://www.rubicon-it.com/Security/Service/Domain/Configuration"">
            <customOrganizationalStructureFactory type=""Rubicon.Security.Service.Domain.UnitTests::Configuration.TestOrganizationalStructureFactory"" />
          </rubicon.security.service.domain>";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (TestOrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeWithInvalidFactoryType ()
    {
      string xmlFragment = @"
          <rubicon.security.service.domain>
            <customOrganizationalStructureFactory type=""Invalid"" />
          </rubicon.security.service.domain>";
      _configuration.DeserializeSection (xmlFragment);
      IOrganizationalStructureFactory factory = _configuration.OrganizationalStructureFactory;
    }
  }
}
