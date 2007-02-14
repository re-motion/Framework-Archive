using System;
using System.Configuration;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForSecurityProviderTest : TestBase
  {
    [Test]
    public void Test_WithDefaultService ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullSecurityService), Configuration.SecurityService);
    }

    [Test]
    public void Test_SecurityServiceIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.SecurityService, Configuration.SecurityService);
    }

    [Test]
    public void Test_WithNullSecurityService ()
    {
      string xmlFragment = @"<rubicon.security defaultSecurityProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullSecurityService), Configuration.SecurityService);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void Test_WithInvalidServiceType ()
    {
      string xmlFragment = @"<rubicon.security defaultSecurityProvider=""Invalid"" />";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Dev.Null = Configuration.SecurityService;
    }

    [Test]
    [Explicit]
    public void Test_WithSecurityManagerService ()
    {
      string xmlFragment = @"<rubicon.security defaultSecurityProvider=""SecurityManagerService"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Type expectedType = TypeUtility.GetType ("Rubicon.SecurityManager::SecurityService", true, false);

      Assert.IsInstanceOfType (expectedType, Configuration.SecurityService);
    }

    [Test]
    public void Test_WithCustomService ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider=""Custom"">
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityServiceMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (SecurityServiceMock), Configuration.SecurityService);
    }

    [Test]
    public void Test_WithSecurityServicesAndFallbackToDefaultWellKnownDefaultSecurityService ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityServiceMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (2, Configuration.SecurityServices.Count);
      Assert.IsInstanceOfType (typeof (SecurityServiceMock), Configuration.SecurityServices["Custom"]);
      Assert.IsInstanceOfType (typeof (NullSecurityService), Configuration.SecurityService);
      Assert.AreSame (Configuration.SecurityService, Configuration.SecurityServices["None"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The provider 'Invalid' specified for the defaultSecurityProvider does not exist in the providers collection.")]
    public void Test_WithCustomSecurityServiceAndInvalidName ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider=""Invalid"">
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityServiceMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.SecurityService;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), "The name of the entry 'None' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownSecurityServiceForNullSecurityService ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider=""None"">
            <securityProviders>
              <add name=""None"" type=""Rubicon.Security.UnitTests::Configuration.SecurityServiceMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), "The name of the entry 'SecurityManager' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownSecurityServiceForSecurityManagerSecurityService ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider=""SecurityManager"">
            <securityProviders>
              <add name=""SecurityManager"" type=""Rubicon.Security.UnitTests::Configuration.SecurityServiceMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The value for the property 'defaultSecurityProvider' is not valid. The error is: The string must be at least 1 characters long.")]
    public void Test_WithCustomSecurityServiceNameEmpty ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider="""">
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityServiceMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.SecurityService;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithSecurityServicesReadOnly ()
    {
      string xmlFragment =
          @"
          <rubicon.security>
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityServiceMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.SecurityServices.Clear ();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), "Provider must implement the interface 'Rubicon.Security.ISecurityService'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      string xmlFragment =
          @"
          <rubicon.security>
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.SecurityService;
    }
  }
}