using System;
using System.Configuration;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Development.UnitTesting.Configuration;

namespace Rubicon.Security.UnitTests.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForGlobalAccessTypeCacheProviderTest: TestBase
  {
    [Test]
    public void Test_FallbackToDefaultWellKnownDefaultGlobalAccessTypeCacheProvider()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (2, Configuration.GlobalAccessTypeCacheProviders.Count);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
      Assert.AreSame (Configuration.GlobalAccessTypeCacheProvider, Configuration.GlobalAccessTypeCacheProviders["None"]);
    }

    [Test]
    public void Test_GlobalAccessTypeCacheProviderIsAlwaysSameInstance()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.GlobalAccessTypeCacheProvider, Configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void Test_WithNoGlobalAccessTypeCacheProvider()
    {
      string xmlFragment = @"<rubicon.security defaultGlobalAccessTypeCacheProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void Test_WithRevisionBasedAccessTypeCacheProvider()
    {
      string xmlFragment = @"<rubicon.security defaultGlobalAccessTypeCacheProvider=""RevisionBased"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (RevisionBasedAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void Test_WithCustomGlobalAccessTypeCacheProvider()
    {
      string xmlFragment =
          @"
          <rubicon.security defaultGlobalAccessTypeCacheProvider=""Custom"">
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (GlobalAccessTypeCacheProviderMock), Configuration.GlobalAccessTypeCacheProvider);
      Assert.AreSame (Configuration.GlobalAccessTypeCacheProvider, Configuration.GlobalAccessTypeCacheProviders["Custom"]);
    }

    [Test]
    public void Test_WithGlobalAccessTypeCacheProvidersAndFallbackToDefaultWellKnownDefaultGlobalAccessTypeCacheProvider()
    {
      string xmlFragment =
          @"
          <rubicon.security>
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (3, Configuration.GlobalAccessTypeCacheProviders.Count);
      Assert.IsInstanceOfType (typeof (GlobalAccessTypeCacheProviderMock), Configuration.GlobalAccessTypeCacheProviders["Custom"]);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
      Assert.AreSame (Configuration.GlobalAccessTypeCacheProvider, Configuration.GlobalAccessTypeCacheProviders["None"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The provider 'Invalid' specified for the defaultGlobalAccessTypeCacheProvider does not exist in the providers collection.")]
    public void Test_WithCustomGlobalAccessTypeCacheProviderAndInvalidName()
    {
      string xmlFragment =
          @"
          <rubicon.security defaultGlobalAccessTypeCacheProvider=""Invalid"">
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.GlobalAccessTypeCacheProvider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The name of the entry 'None' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownGlobalAccessTypeCacheProviderForGlobalAccessTypeCacheNone()
    {
      string xmlFragment =
          @"
          <rubicon.security defaultGlobalAccessTypeCacheProvider=""None"">
            <globalAccessTypeCacheProviders>
              <add name=""None"" type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The name of the entry 'RevisionBased' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownGlobalAccessTypeCacheProviderForGlobalAccessTypeCacheRevisionBased()
    {
      string xmlFragment =
          @"
          <rubicon.security defaultGlobalAccessTypeCacheProvider=""RevisionBased"">
            <globalAccessTypeCacheProviders>
              <add name=""RevisionBased"" type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The value for the property 'defaultGlobalAccessTypeCacheProvider' is not valid. The error is: The string must be at least 1 characters long."
        )]
    public void Test_WithCustomGlobalAccessTypeCacheProviderNameEmpty()
    {
      string xmlFragment =
          @"
          <rubicon.security defaultGlobalAccessTypeCacheProvider="""">
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.GlobalAccessTypeCacheProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithGlobalAccessTypeCacheProvidersReadOnly()
    {
      string xmlFragment =
          @"
          <rubicon.security>
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.GlobalAccessTypeCacheProviders.Clear();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException),
        "Provider must implement the interface 'Rubicon.Security.IGlobalAccessTypeCacheProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface()
    {
      string xmlFragment =
          @"
          <rubicon.security>
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </globalAccessTypeCacheProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.GlobalAccessTypeCacheProvider;
    }
  }
}