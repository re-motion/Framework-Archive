using System;
using System.Configuration;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Development.UnitTesting.Configuration;
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
      Assert.IsInstanceOfType (typeof (NullSecurityProvider), Configuration.SecurityProvider);
    }

    [Test]
    public void Test_SecurityProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.SecurityProvider, Configuration.SecurityProvider);
    }

    [Test]
    public void Test_WithNullSecurityProvider ()
    {
      string xmlFragment = @"<rubicon.security defaultSecurityProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullSecurityProvider), Configuration.SecurityProvider);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void Test_WithInvalidServiceType ()
    {
      string xmlFragment = @"<rubicon.security defaultSecurityProvider=""Invalid"" />";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Dev.Null = Configuration.SecurityProvider;
    }

    [Test]
    [Explicit]
    public void Test_WithSecurityManagerService ()
    {
      string xmlFragment = @"<rubicon.security defaultSecurityProvider=""SecurityManagerService"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Type expectedType = TypeUtility.GetType ("Rubicon.SecurityManager::SecurityService", true, false);

      Assert.IsInstanceOfType (expectedType, Configuration.SecurityProvider);
    }

    [Test]
    public void Test_WithCustomService ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider=""Custom"">
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityProviderMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (SecurityProviderMock), Configuration.SecurityProvider);
    }

    [Test]
    public void Test_WithSecurityProvidersAndFallbackToDefaultWellKnownDefaultSecurityProvider ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityProviderMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (2, Configuration.SecurityProviders.Count);
      Assert.IsInstanceOfType (typeof (SecurityProviderMock), Configuration.SecurityProviders["Custom"]);
      Assert.IsInstanceOfType (typeof (NullSecurityProvider), Configuration.SecurityProvider);
      Assert.AreSame (Configuration.SecurityProvider, Configuration.SecurityProviders["None"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultSecurityProvider does not exist in the providers collection.")]
    public void Test_WithCustomSecurityProviderAndInvalidName ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider=""Invalid"">
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityProviderMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.SecurityProvider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = "The name of the entry 'None' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownSecurityProviderForNullSecurityProvider ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider=""None"">
            <securityProviders>
              <add name=""None"" type=""Rubicon.Security.UnitTests::Configuration.SecurityProviderMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = "The name of the entry 'SecurityManager' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownSecurityProviderForSecurityManagerSecurityService ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider=""SecurityManager"">
            <securityProviders>
              <add name=""SecurityManager"" type=""Rubicon.Security.UnitTests::Configuration.SecurityProviderMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The value for the property 'defaultSecurityProvider' is not valid. The error is: The string must be at least 1 characters long.")]
    public void Test_WithCustomSecurityProviderNameEmpty ()
    {
      string xmlFragment = @"
          <rubicon.security defaultSecurityProvider="""">
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityProviderMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.SecurityProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithSecurityProvidersReadOnly ()
    {
      string xmlFragment =
          @"
          <rubicon.security>
            <securityProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.SecurityProviderMock"" />
            </securityProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.SecurityProviders.Clear ();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), "Provider must implement the interface 'Rubicon.Security.ISecurityProvider'.")]
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

      Dev.Null = Configuration.SecurityProvider;
    }
  }
}