using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;

using NUnit.Framework;
using Rubicon.Development.UnitTesting;
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

    private SecurityConfiguration _configuration;

    // construction and disposing

    public SecurityConfigurationTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _configuration = new SecurityConfiguration ();
      SetCurrentSecurityConfiguration (null);
    }

    [TearDown]
    public void TearDown ()
    {
      SetCurrentSecurityConfiguration (null);
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
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      // Succeeded
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultService ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsNull (_configuration.SecurityService);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithNoService ()
    {
      string xmlFragment = @"<rubicon.security service=""None"" />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsNull (_configuration.SecurityService);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSecurityConfigurationWithInvalidServiceType ()
    {
      string xmlFragment = @"<rubicon.security service=""Invalid"" />";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      ISecurityService service = _configuration.SecurityService;
    }

    [Test]
    [Explicit]
    public void DeserializeSecurityConfigurationWithSecurityManagerService ()
    {
      string xmlFragment = @"<rubicon.security service=""SecurityManagerService"" />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
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

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

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

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      ISecurityService service = _configuration.SecurityService;
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultUserProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), _configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithNoUserProvider ()
    {
      string xmlFragment = @"<rubicon.security userProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsNull (_configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithThreadUserProvider ()
    {
      string xmlFragment = @"<rubicon.security userProvider=""Thread"" />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), _configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithHttpContextUserProvider ()
    {
      string xmlFragment = @"<rubicon.security userProvider=""HttpContext"" />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (HttpContextUserProvider), _configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomUserProvider ()
    {
      string xmlFragment = @"
          <rubicon.security userProvider=""Custom"">
            <customUserProvider type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (UserProviderMock), _configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultPermissionProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (PermissionReflector), _configuration.PermissionProvider);
    }

    [Test]
    public void PermissionProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.AreSame (_configuration.PermissionProvider, _configuration.PermissionProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomPermissionProvider ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <customPermissionProvider type=""Rubicon.Security.UnitTests::Configuration.PermissionProviderMock"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (PermissionProviderMock), _configuration.PermissionProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultFunctionalSecurityStrategy ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategy), _configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void FunctionalSecurityStrategyIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.AreSame (_configuration.FunctionalSecurityStrategy, _configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomFunctionalSecurityStrategy ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <customFunctionalSecurityStrategy type=""Rubicon.Security.UnitTests::Configuration.FunctionalSecurityStrategyMock"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategyMock), _configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), _configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithNoGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"<rubicon.security globalAccessTypeCacheProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), _configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithRevisionBasedAccessTypeCacheProvider ()
    {
      string xmlFragment = @"<rubicon.security globalAccessTypeCacheProvider=""RevisionBased"" />";
      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (RevisionBasedAccessTypeCacheProvider), _configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"
          <rubicon.security globalAccessTypeCacheProvider=""Custom"">
            <customGlobalAccessTypeCacheProvider type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (GlobalAccessTypeCacheProviderMock), _configuration.GlobalAccessTypeCacheProvider);
    }

    private void SetCurrentSecurityConfiguration (SecurityConfiguration configuration)
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (SecurityConfiguration), "SetCurrent", configuration);
    }
  }
}