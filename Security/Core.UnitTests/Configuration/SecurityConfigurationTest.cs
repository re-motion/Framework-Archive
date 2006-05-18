using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Rubicon.Security.Configuration;
using Rubicon.Utilities;
using System.IO;
using System.Configuration;

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
    }

    [Test]
    public void DeserializeSecurityConfigurationWithDefaultServiceType ()
    {
      string xmlFragment = @"<securityConfiguration />";

      _configuration.DeserializeSection (xmlFragment);
      Assert.AreEqual (SecurityServiceType.None, _configuration.SecurityServiceType);
      Assert.IsNull (_configuration.SecurityService);
    }

    //[Test]
    //public void DeserializeSecurityConfigurationWithValidServiceType ()
    //{
    //  string xmlFragment = @"<securityConfiguration service=""SecurityManagerService"" />";

    //  _configuration.DeserializeSection (xmlFragment);
    //  Assert.AreEqual (SecurityServiceType.SecurityManagerService, _configuration.SecurityServiceType);
    //  Assert.IsNotNull (_configuration.CustomService);
    //  Assert.IsInstanceOfType (typeof (SecurityManagerService), _configuration.SecurityService);
    //}

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSecurityConfigurationWithInvalidServiceType ()
    {
      string xmlFragment = @"<securityConfiguration service=""Invalid"" />";

      _configuration.DeserializeSection (xmlFragment);
      SecurityServiceType type = _configuration.SecurityServiceType;
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomService ()
    {
      string xmlFragment = @"
          <securityConfiguration service=""Custom"">
            <customService type=""Rubicon.Security.UnitTests.Configuration.SecurityServiceMock, Rubicon.Security.UnitTests"" />
          </securityConfiguration>";

      _configuration.DeserializeSection (xmlFragment);
      Assert.IsNotNull (_configuration.CustomService);
      Assert.AreSame (typeof (SecurityServiceMock), _configuration.CustomService.Type);
      Assert.IsNotNull (_configuration.SecurityService);
      Assert.IsInstanceOfType (typeof (SecurityServiceMock), _configuration.SecurityService);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSecurityConfigurationWithCustomServiceHavingInvalidType ()
    {
      string xmlFragment = @"
          <securityConfiguration service=""Custom"">
            <customService type=""Invalid"" />
          </securityConfiguration>";

      _configuration.DeserializeSection (xmlFragment);
      Assert.IsNotNull (_configuration.CustomService);
      Type type = _configuration.CustomService.Type;
    }

    [Test]
    public void DeserializeSecurityConfigurationWithCustomUserProvider ()
    {
      string xmlFragment = @"
          <securityConfiguration userProvider=""Custom"">
            <customUserProvider type=""Rubicon.Security.UnitTests.Configuration.UserProviderMock, Rubicon.Security.UnitTests"" />
          </securityConfiguration>";

      _configuration.DeserializeSection (xmlFragment);
      Assert.IsNotNull (_configuration.CustomUserProvider);
      Assert.AreSame (typeof (UserProviderMock), _configuration.CustomUserProvider.Type);
      Assert.IsNotNull (_configuration.UserProvider);
      Assert.IsInstanceOfType (typeof (UserProviderMock), _configuration.UserProvider);
    }

    [Test]
    public void DeserializeSecurityConfigurationWithThreadUserProvider ()
    {
      string xmlFragment = @"<securityConfiguration userProvider=""Thread"" />";

      _configuration.DeserializeSection (xmlFragment);
      Assert.IsNotNull (_configuration.UserProvider);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), _configuration.UserProvider);
    }
  }
}