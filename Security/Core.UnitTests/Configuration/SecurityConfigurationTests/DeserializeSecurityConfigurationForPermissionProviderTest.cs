using System;
using System.Configuration;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForPermissionProviderTest : TestBase
  {
    [Test]
    public void Test_FallbackToDefaultWellKnownDefaultPermissionProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (1, Configuration.PermissionProviders.Count);
      Assert.IsInstanceOfType (typeof (PermissionReflector), Configuration.PermissionProvider);
      Assert.AreSame (Configuration.PermissionProvider, Configuration.PermissionProviders["Reflection"]);
    }

    [Test]
    public void Test_PermissionProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.PermissionProvider, Configuration.PermissionProvider);
    }

    [Test]
    public void Test_CustomPermissionProvider ()
    {
      string xmlFragment = @"
          <rubicon.security defaultPermissionProvider=""Custom"">
            <permissionProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (PermissionProviderMock), Configuration.PermissionProvider);
      Assert.AreSame (Configuration.PermissionProvider, Configuration.PermissionProviders["Custom"]);
    }

    [Test]
    public void Test_WithPermissionProvidersAndFallbackToDefaultWellKnownDefaultPermissionProvider ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <permissionProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (2, Configuration.PermissionProviders.Count);
      Assert.IsInstanceOfType (typeof (PermissionProviderMock), Configuration.PermissionProviders["Custom"]);
      Assert.IsInstanceOfType (typeof (PermissionReflector), Configuration.PermissionProvider);
      Assert.AreSame (Configuration.PermissionProvider, Configuration.PermissionProviders["Reflection"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The provider 'Invalid' specified for the defaultPermissionProvider does not exist in the providers collection.")]
    public void Test_WithCustomPermissionProviderAndInvalidName ()
    {
      string xmlFragment = @"
          <rubicon.security defaultPermissionProvider=""Invalid"">
            <permissionProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      object dummy = Configuration.PermissionProvider;
    }

    [Test]
    public void Test_NoDuplicateWellKnownPermissionProviderForPermissionReflector ()
    {
      string xmlFragment = @"
          <rubicon.security defaultPermissionProvider=""Reflection"">
            <permissionProviders>
              <add name=""Reflection"" type=""Rubicon.Security.UnitTests::Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (PermissionProviderMock), Configuration.PermissionProvider);
      Assert.AreSame (Configuration.PermissionProvider, Configuration.PermissionProviders["Reflection"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The value for the property 'defaultPermissionProvider' is not valid. The error is: The string must be at least 1 characters long.")]
    public void Test_WithCustomPermissionProviderNameEmpty ()
    {
      string xmlFragment = @"
          <rubicon.security defaultPermissionProvider="""">
            <permissionProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      object dummy = Configuration.PermissionProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithPermissionProvidersReadOnly ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <permissionProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.PermissionProviders.Clear();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException),
        "Provider must implement the interface 'Rubicon.Security.Metadata.IPermissionProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <permissionProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </permissionProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      object dummy = Configuration.PermissionProvider;
    }
  }
}