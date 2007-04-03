using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Development.UnitTesting.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  [TestFixture]
  public class ProviderHelperBaseTest
  {
    private StubProviderHelper _providerHelper;
    private ConfigurationPropertyCollection _propertyCollection;
    private StubExtendedConfigurationSection _stubConfigurationSection;

    [SetUp]
    public void SetUp()
    {
      _stubConfigurationSection = new StubExtendedConfigurationSection();
      _providerHelper = _stubConfigurationSection.GetStubProviderHelper();
      _propertyCollection = _stubConfigurationSection.GetProperties();
      _providerHelper.InitializeProperties (_propertyCollection);
    }

    [Test]
    public void Initialize()
    {
      Assert.AreEqual (2, _propertyCollection.Count);

      ConfigurationProperty defaultProviderProperty = _propertyCollection["defaultProvider"];
      Assert.IsNotNull (defaultProviderProperty);
      Assert.AreEqual (typeof (string), defaultProviderProperty.Type);
      Assert.AreEqual ("Default Value", defaultProviderProperty.DefaultValue);
      Assert.IsFalse (defaultProviderProperty.IsRequired);
      Assert.IsInstanceOfType (typeof (StringValidator), defaultProviderProperty.Validator);

      ConfigurationProperty providersProperty = _propertyCollection["providers"];
      Assert.IsNotNull (providersProperty);
      Assert.AreEqual (typeof (ProviderSettingsCollection), providersProperty.Type);
      Assert.IsNull (providersProperty.DefaultValue);
      Assert.IsFalse (providersProperty.IsRequired);
      Assert.IsInstanceOfType (typeof (DefaultValidator), providersProperty.Validator);
    }

    [Test]
    public void GetProviders()
    {
      string xmlFragment = @"
          <stubConfigSection>
            <providers>
              <add name=""Fake"" type=""Rubicon.Core.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);

      Assert.AreEqual (2, _providerHelper.Providers.Count);
      Assert.IsInstanceOfType (typeof (FakeProvider), _providerHelper.Providers["Fake"]);
    }

    [Test]
    public void GetProvider()
    {
      string xmlFragment = @"
          <stubConfigSection defaultProvider=""Fake"">
            <providers>
              <add name=""Fake"" type=""Rubicon.Core.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);

      Assert.IsInstanceOfType (typeof (FakeProvider), _providerHelper.Provider);
      Assert.AreSame (_providerHelper.Providers["Fake"], _providerHelper.Provider);
    }

    [Test]
    public void GetProvider_WithWellKnownProvider ()
    {
      string xmlFragment = @"<stubConfigSection defaultProvider=""WellKnown"" />";
      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);
      Assert.IsInstanceOfType (typeof (FakeWellKnownProvider), _providerHelper.Provider);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultProvider does not exist in the providers collection.")]
    public void GetProvider_WithInvalidProviderName()
    {
      string xmlFragment = @"
          <stubConfigSection defaultProvider=""Invalid"">
            <providers>
              <add name=""Fake"" type=""Rubicon.Core.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);

      Dev.Null = _providerHelper.Provider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The name of the entry 'WellKnown' identifies a well known provider and cannot be reused for custom providers.")]
    public void PostDeserialize_DuplicateWellKnownProvider ()
    {
      string xmlFragment = @"
          <stubConfigSection defaultProvider=""WellKnown"">
            <providers>
              <add name=""WellKnown"" type=""Rubicon.Core.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);
    }

    [Test]
    public void GetType_Test ()
    {
      Type type = _providerHelper.GetType (
          _propertyCollection["defaultProvider"], 
          typeof (FakeProvider).Assembly.GetName(), 
          "Rubicon.Core.UnitTests.Configuration.FakeProvider");

      Assert.AreSame (typeof (FakeProvider), type);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), 
        ExpectedMessage = "The current value of property 'defaultProvider' requires that the assembly 'Invalid' is placed within the CLR's probing path for this application.")]
    public void GetType_WithInvalidAssemblyName ()
    {
      _providerHelper.GetType (
          _propertyCollection["defaultProvider"],
          new AssemblyName ("Invalid"),
          "Rubicon.Core.UnitTests.Configuration.FakeProvider");
    }
    
    [Test]
    public void GetTypeWithMatchingVersionNumber ()
    {
      Type type = _providerHelper.GetTypeWithMatchingVersionNumber (
          _propertyCollection["defaultProvider"],
          "Rubicon.Core.UnitTests",
          "Rubicon.Core.UnitTests.Configuration.FakeProvider");

      Assert.AreSame (typeof (FakeProvider), type);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void GetTypeWithMatchingVersionNumber_WithInvalidAssemblyName ()
    {
      _providerHelper.GetTypeWithMatchingVersionNumber (
         _propertyCollection["defaultProvider"],
         "Invalid",
         "Rubicon.Core.UnitTests.Configuration.FakeProvider");
    }

    [Test]
    public void InstantiateProvider ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Rubicon.Core.UnitTests::Configuration.FakeProvider");
      providerSettings.Parameters.Add ("description", "The Description");

      ProviderBase providerBase = _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase), typeof (IFakeProvider));

      Assert.IsNotNull (providerBase);
      Assert.IsInstanceOfType (typeof (FakeProvider), providerBase);
      Assert.AreEqual ("Custom", providerBase.Name);
      Assert.AreEqual ("The Description", providerBase.Description);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), "Type name must be specified for this provider.")]
    public void InstantiateProvider_WithMissingTypeName ()
    {
      _providerHelper.InstantiateProvider (new ProviderSettings (), typeof (FakeProviderBase));
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), "Provider must implement the class 'Rubicon.Core.UnitTests.Configuration.FakeProviderBase'.")]
    public void InstantiateProvider_WithTypeNotDerivedFromRequiredBaseType ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Rubicon.Core.UnitTests::Configuration.FakeOtherProvider");
      _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase));
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), "Provider must implement the interface 'Rubicon.Core.UnitTests.Configuration.IFakeProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Rubicon.Core.UnitTests::Configuration.FakeProviderBase");
      _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase), typeof (IFakeProvider));
    }

    [Test]
    public void InstantiateProviders ()
    {
      ProviderSettingsCollection providerSettingsCollection = new ProviderSettingsCollection ();
      providerSettingsCollection.Add (new ProviderSettings ("Custom", "Rubicon.Core.UnitTests::Configuration.FakeProvider"));
      ProviderCollection providerCollection = new ProviderCollection ();

      _providerHelper.InstantiateProviders (providerSettingsCollection, providerCollection, typeof (FakeProviderBase), typeof (IFakeProvider));

      Assert.AreEqual (1, providerCollection.Count);
      ProviderBase providerBase = providerCollection["Custom"];
      Assert.IsInstanceOfType (typeof (FakeProvider), providerBase);
      Assert.AreEqual ("Custom", providerBase.Name);
    }
  }
}