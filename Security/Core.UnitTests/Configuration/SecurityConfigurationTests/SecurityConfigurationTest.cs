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

namespace Rubicon.Security.UnitTests.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class SecurityConfigurationTest : TestBase
  {
    [Test]
    public void GetSecurityConfigurationWithoutConfigurationSection ()
    {
      SecurityConfiguration configuration = SecurityConfiguration.Current;

      Assert.IsNotNull (configuration);
      Assert.IsNull (configuration.SecurityService);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithNamespace ()
    {
      string xmlFragment = @"<rubicon.security xmlns=""http://www.rubicon-it.com/Security/Configuration"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      // Succeeded
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithDefaultService ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsNull (Configuration.SecurityService);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithNoService ()
    {
      string xmlFragment = @"<rubicon.security service=""None"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsNull (Configuration.SecurityService);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSecurityConfiguration_WithInvalidServiceType ()
    {
      string xmlFragment = @"<rubicon.security service=""Invalid"" />";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      ISecurityService service = Configuration.SecurityService;
    }

    [Test]
    [Explicit]
    public void DeserializeSecurityConfiguration_WithSecurityManagerService ()
    {
      string xmlFragment = @"<rubicon.security service=""SecurityManagerService"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Type expectedType = TypeUtility.GetType ("Rubicon.SecurityManager::SecurityService", true, false);

      Assert.IsInstanceOfType (expectedType, Configuration.SecurityService);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithCustomService ()
    {
      string xmlFragment = @"
          <rubicon.security service=""Custom"">
            <customService type=""Rubicon.Security.UnitTests::Configuration.SecurityServiceMock"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (SecurityServiceMock), Configuration.SecurityService);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSecurityConfiguration_WithCustomServiceHavingInvalidType ()
    {
      string xmlFragment = @"
          <rubicon.security service=""Custom"">
            <customService type=""Invalid"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      ISecurityService service = Configuration.SecurityService;
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithDefaultUserProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), Configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithNoUserProvider ()
    {
      string xmlFragment = @"<rubicon.security userProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsNull (Configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithThreadUserProvider ()
    {
      string xmlFragment = @"<rubicon.security userProvider=""Thread"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), Configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithHttpContextUserProvider ()
    {
      string xmlFragment = @"<rubicon.security userProvider=""HttpContext"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (HttpContextUserProvider), Configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithCustomUserProvider ()
    {
      string xmlFragment = @"
          <rubicon.security userProvider=""Custom"">
            <customUserProvider type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (UserProviderMock), Configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithDefaultFunctionalSecurityStrategy ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategy), Configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void FunctionalSecurityStrategyIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.FunctionalSecurityStrategy, Configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithCustomFunctionalSecurityStrategy ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <customFunctionalSecurityStrategy type=""Rubicon.Security.UnitTests::Configuration.FunctionalSecurityStrategyMock"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategyMock), Configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithDefaultGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithNoGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"<rubicon.security globalAccessTypeCacheProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithRevisionBasedAccessTypeCacheProvider ()
    {
      string xmlFragment = @"<rubicon.security globalAccessTypeCacheProvider=""RevisionBased"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (RevisionBasedAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithCustomGlobalAccessTypeCacheProvider ()
    {
      string xmlFragment = @"
          <rubicon.security globalAccessTypeCacheProvider=""Custom"">
            <customGlobalAccessTypeCacheProvider type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (GlobalAccessTypeCacheProviderMock), Configuration.GlobalAccessTypeCacheProvider);
    }
  }
}