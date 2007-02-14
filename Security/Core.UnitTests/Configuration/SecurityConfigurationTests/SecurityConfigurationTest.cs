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
      Assert.IsInstanceOfType (typeof (NullSecurityService), Configuration.SecurityService);
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
            <functionalSecurityStrategy type=""Rubicon.Security.UnitTests::Configuration.FunctionalSecurityStrategyMock"" />
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategyMock), Configuration.FunctionalSecurityStrategy);
    }
  }
}