using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Rubicon.Security.Configuration;
using Rubicon.Security.Metadata;
using Rubicon.Security.Web;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Configuration
{
  [TestFixture]
  public class SecurityConfigurationTest
  {
    // types

    // static members

    // member fields

    private SecurityConfigurationMock _configuration;

    // construction and disposing

    public SecurityConfigurationTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _configuration = new SecurityConfigurationMock ();
      SecurityConfigurationMock.SetCurrent (null);
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (null);
    }

    [Test]
    public void GetSecurityConfigurationWithoutConfigurationSection ()
    {
      SecurityConfiguration configuration = SecurityConfiguration.Current;

      Assert.IsNotNull (configuration);
      Assert.IsNull (configuration.SecurityService);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithNamespace ()
    {
      string xmlFragment = @"<rubicon.security xmlns=""http://www.rubicon-it.com/Security/Configuration"" />";
      _configuration.DeserializeSection (xmlFragment);
      // Succeeded
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultService ()
    {
      string xmlFragment = @"<rubicon.security />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsNull (_configuration.SecurityService);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithNoService ()
    {
      string xmlFragment = @"<rubicon.security service=""None"" />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsNull (_configuration.SecurityService);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSecurityConfigurationWithInvalidServiceType ()
    {
      string xmlFragment = @"<rubicon.security service=""Invalid"" />";

      _configuration.DeserializeSection (xmlFragment);
      ISecurityService service = _configuration.SecurityService;
    }

    [Test]
    [Explicit]
    public void DeserializeSecurityConfigurationWithSecurityManagerService ()
    {
      string xmlFragment = @"<rubicon.security service=""SecurityManagerService"" />";
      _configuration.DeserializeSection (xmlFragment);
      Type expectedType = TypeUtility.GetType ("Rubicon.SecurityManager::SecurityService", true, false);
      
      Assert.IsInstanceOfType (expectedType, _configuration.SecurityService);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomService ()
    {
      string xmlFragment = @"
          <rubicon.security service=""Custom"">
            <customService type=""Rubicon.Security.UnitTests::Configuration.SecurityServiceMock"" />
          </rubicon.security>";

      _configuration.DeserializeSection (xmlFragment);

      Assert.IsInstanceOfType (typeof (SecurityServiceMock), _configuration.SecurityService);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSecurityConfigurationWithCustomServiceHavingInvalidType ()
    {
      string xmlFragment = @"
          <rubicon.security service=""Custom"">
            <customService type=""Invalid"" />
          </rubicon.security>";

      _configuration.DeserializeSection (xmlFragment);
      ISecurityService service = _configuration.SecurityService;
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultUserProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), _configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithNoUserProvider ()
    {
      string xmlFragment = @"<rubicon.security userProvider=""None"" />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsNull (_configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithThreadUserProvider ()
    {
      string xmlFragment = @"<rubicon.security userProvider=""Thread"" />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), _configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithHttpContextUserProvider ()
    {
      string xmlFragment = @"<rubicon.security userProvider=""HttpContext"" />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsInstanceOfType (typeof (HttpContextUserProvider), _configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomUserProvider ()
    {
      string xmlFragment = @"
          <rubicon.security userProvider=""Custom"">
            <customUserProvider type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
          </rubicon.security>";

      _configuration.DeserializeSection (xmlFragment);

      Assert.IsInstanceOfType (typeof (UserProviderMock), _configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultPermissionProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsInstanceOfType (typeof (PermissionReflector), _configuration.PermissionProvider);
    }

    [Test]
    public void PermissionProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<rubicon.security />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.AreSame (_configuration.PermissionProvider, _configuration.PermissionProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomPermissionProvider ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <customPermissionProvider type=""Rubicon.Security.UnitTests::Configuration.PermissionProviderMock"" />
          </rubicon.security>";

      _configuration.DeserializeSection (xmlFragment);

      Assert.IsInstanceOfType (typeof (PermissionProviderMock), _configuration.PermissionProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultFunctionalSecurityStrategy ()
    {
      string xmlFragment = @"<rubicon.security />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategy), _configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void FunctionalSecurityStrategyIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<rubicon.security />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.AreSame (_configuration.FunctionalSecurityStrategy, _configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomFunctionalSecurityStrategy ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <customFunctionalSecurityStrategy type=""Rubicon.Security.UnitTests::Configuration.FunctionalSecurityStrategyMock"" />
          </rubicon.security>";

      _configuration.DeserializeSection (xmlFragment);

      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategyMock), _configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), _configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithNoGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"<rubicon.security globalAccessTypeCacheProvider=""None"" />";
      _configuration.DeserializeSection (xmlFragment);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), _configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    [Explicit]
    public void DeserializeSecurityConfigurationWithDomainObjectGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"<rubicon.security globalAccessTypeCacheProvider=""ClientTransaction"" />";
      _configuration.DeserializeSection (xmlFragment);
      Type expectedType = TypeUtility.GetType ("Rubicon.Security.Data.DomainObjects::ClientTransactionAccessTypeCacheProvider", true, false);

      Assert.IsInstanceOfType (expectedType, _configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"
          <rubicon.security globalAccessTypeCacheProvider=""Custom"">
            <customGlobalAccessTypeCacheProvider type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
          </rubicon.security>";

      _configuration.DeserializeSection (xmlFragment);

      Assert.IsInstanceOfType (typeof (GlobalAccessTypeCacheProviderMock), _configuration.GlobalAccessTypeCacheProvider);
    }
  }
}