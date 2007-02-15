using System;
using System.Configuration;
using System.Configuration.Provider;
using NUnit.Framework;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  [TestFixture]
  public class ProviderHelperTest
  {
    [Test]
    public void InstantiateProvider ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Rubicon.Core.UnitTests::Configuration.DerivedProviderMock");
      providerSettings.Parameters.Add ("description", "The Description");
      
      ProviderBase providerBase = ProviderHelper.InstantiateProvider (providerSettings, typeof (ProviderMock), typeof (IProvider));

      Assert.IsNotNull (providerBase);
      Assert.IsInstanceOfType (typeof (DerivedProviderMock), providerBase);
      Assert.AreEqual ("Custom", providerBase.Name);
      Assert.AreEqual ("The Description", providerBase.Description);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), "Type name must be specified for this provider.")]
    public void InstantiateProvider_WithMissingTypeName ()
    {
      ProviderHelper.InstantiateProvider (new ProviderSettings (), typeof (ProviderMock));
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), "Provider must implement the class 'Rubicon.Core.UnitTests.Configuration.ProviderMock'.")]
    public void InstantiateProvider_WithTypeNotDerivedFromRequiredBaseType ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Rubicon.Core.UnitTests::Configuration.OtherProviderMock");
      ProviderHelper.InstantiateProvider (providerSettings, typeof (ProviderMock));
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), "Provider must implement the interface 'Rubicon.Core.UnitTests.Configuration.IProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Rubicon.Core.UnitTests::Configuration.ProviderMock");
      ProviderHelper.InstantiateProvider (providerSettings, typeof (ProviderMock), typeof (IProvider));
    }

    [Test]
    public void InstantiateProviders ()
    {
      ProviderSettingsCollection providerSettingsCollection = new ProviderSettingsCollection();
      providerSettingsCollection.Add (new ProviderSettings ("Custom", "Rubicon.Core.UnitTests::Configuration.DerivedProviderMock"));
      ProviderCollection providerCollection = new ProviderCollection();

      ProviderHelper.InstantiateProviders (providerSettingsCollection, providerCollection, typeof (ProviderMock), typeof (IProvider));

      Assert.AreEqual (1, providerCollection.Count);
      ProviderBase providerBase = providerCollection["Custom"];
      Assert.IsInstanceOfType (typeof (DerivedProviderMock), providerBase);
      Assert.AreEqual ("Custom", providerBase.Name);
    }
  }
}