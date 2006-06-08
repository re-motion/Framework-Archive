using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using NUnit.Framework;

using Rubicon.SecurityManager.Domain.Configuration;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.Domain.UnitTests.Configuration
{
  [TestFixture]
  public class DomainConfigurationTest
  {
    private TestDomainConfiguration _configuration;

    [SetUp]
    public void SetUp ()
    {
      _configuration = new TestDomainConfiguration ();
    }

    [Test]
    public void DeserializeSection_DefaultFactory ()
    {
      string xmlFragment = @"<rubicon.securityManager.domain />";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (OrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    public void DeserializeSection_WithNamespace ()
    {
      string xmlFragment = @"<rubicon.securityManager.domain xmlns=""http://www.rubicon-it.com/SecurityManager/Domain/Configuration"" />";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (OrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    public void DeserializeSection_CustomFactory ()
    {
      string xmlFragment = @"
          <rubicon.securityManager.domain xmlns=""http://www.rubicon-it.com/SecurityManager/Domain/Configuration"">
            <customOrganizationalStructureFactory type=""Rubicon.SecurityManager.Domain.UnitTests::Configuration.TestOrganizationalStructureFactory"" />
          </rubicon.securityManager.domain>";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (TestOrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSection_InvalidFactoryType ()
    {
      string xmlFragment = @"
          <rubicon.securityManager.domain>
            <customOrganizationalStructureFactory type=""Invalid"" />
          </rubicon.securityManager.domain>";
      _configuration.DeserializeSection (xmlFragment);
      IOrganizationalStructureFactory factory = _configuration.OrganizationalStructureFactory;
    }
  }
}
