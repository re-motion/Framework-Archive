using System;
using System.Configuration;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Configuration
{
  [TestFixture]
  public class SecurityManagerConfigurationTest
  {
    private TestSecurityManagerConfiguration _configuration;

    [SetUp]
    public void SetUp ()
    {
      _configuration = new TestSecurityManagerConfiguration ();
    }

    [Test]
    public void DeserializeSection_DefaultFactory ()
    {
      string xmlFragment = @"<rubicon.securityManager />";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (OrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    public void DeserializeSection_WithNamespace ()
    {
      string xmlFragment = @"<rubicon.securityManager xmlns=""http://www.rubicon-it.com/SecurityManager/Configuration"" />";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (OrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    public void DeserializeSection_CustomFactory ()
    {
      string xmlFragment = @"
          <rubicon.securityManager xmlns=""http://www.rubicon-it.com/SecurityManager/Configuration"">
            <organizationalStructureFactory type=""Rubicon.SecurityManager.UnitTests::Configuration.TestOrganizationalStructureFactory"" />
          </rubicon.securityManager>";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (TestOrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSection_InvalidFactoryType ()
    {
      string xmlFragment = @"
          <rubicon.securityManager>
            <organizationalStructureFactory type=""Invalid"" />
          </rubicon.securityManager>";
      _configuration.DeserializeSection (xmlFragment);
      IOrganizationalStructureFactory factory = _configuration.OrganizationalStructureFactory;
    }
  }
}
