using System;
using System.Configuration;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Development.UnitTesting.Configuration;
using Rubicon.Security.Web;

namespace Rubicon.Security.UnitTests.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForUserProviderTest : TestBase
  {
    [Test]
    public void Test_WithDefaultUserProvider ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), Configuration.UserProvider);
    }

    [Test]
    public void Test_UserProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<rubicon.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.UserProvider, Configuration.UserProvider);
    }

    [Test]
    public void Test_WithThreadUserProvider ()
    {
      string xmlFragment = @"<rubicon.security defaultUserProvider=""Thread"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), Configuration.UserProvider);
    }

    [Test]
    public void Test_WithHttpContextUserProvider ()
    {
      string xmlFragment = @"<rubicon.security defaultUserProvider=""HttpContext"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (HttpContextUserProvider), Configuration.UserProvider);
    }

    [Test]
    public void Test_WithCustomUserProvider ()
    {
      string xmlFragment = @"
          <rubicon.security defaultUserProvider=""Custom"">
            <userProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </userProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (UserProviderMock), Configuration.UserProvider);
    }

    [Test]
    public void Test_WithUserProvidersAndFallbackToDefaultWellKnownDefaultUserProvider ()
    {
      string xmlFragment = @"
          <rubicon.security>
            <userProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </userProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (2, Configuration.UserProviders.Count);
      Assert.IsInstanceOfType (typeof (UserProviderMock), Configuration.UserProviders["Custom"]);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), Configuration.UserProvider);
      Assert.AreSame (Configuration.UserProvider, Configuration.UserProviders["Thread"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
         "The provider 'Invalid' specified for the defaultUserProvider does not exist in the providers collection.")]
    public void Test_WithCustomUserProviderAndInvalidName ()
    {
      string xmlFragment = @"
          <rubicon.security defaultUserProvider=""Invalid"">
            <userProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </userProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.UserProvider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), 
        "The name of the entry 'Thread' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownUserProviderForThreadUserProvider ()
    {
      string xmlFragment = @"
          <rubicon.security defaultUserProvider=""Thread"">
            <userProviders>
              <add name=""Thread"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </userProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), 
        "The name of the entry 'HttpContext' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownUserProviderForHttpContextUserProvider ()
    {
      string xmlFragment = @"
          <rubicon.security defaultUserProvider=""HttpContext"">
            <userProviders>
              <add name=""HttpContext"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </userProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The value for the property 'defaultUserProvider' is not valid. The error is: The string must be at least 1 characters long.")]
    public void Test_WithCustomUserProviderNameEmpty ()
    {
      string xmlFragment = @"
          <rubicon.security defaultUserProvider="""">
            <userProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </userProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.UserProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithUserProvidersReadOnly ()
    {
      string xmlFragment =
          @"
          <rubicon.security>
            <userProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.UserProviderMock"" />
            </userProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.UserProviders.Clear ();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), "Provider must implement the interface 'Rubicon.Security.IUserProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      string xmlFragment =
          @"
          <rubicon.security>
            <userProviders>
              <add name=""Custom"" type=""Rubicon.Security.UnitTests::Configuration.PermissionProviderMock"" />
            </userProviders>
          </rubicon.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.UserProvider;
    }
  }
}