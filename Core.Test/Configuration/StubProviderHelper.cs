using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Reflection;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class StubProviderHelper: ProviderHelperBase<IFakeProvider>
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing

    public StubProviderHelper (ExtendedConfigurationSection configurationSection)
        : base (configurationSection)
    {
    }

    // methods and properties

    public override void PostDeserialze()
    {
      base.PostDeserialze();
      
      CheckForDuplicateWellKownProviderName ("WellKnown");
    }

    public new Type GetTypeWithMatchingVersionNumber (ConfigurationProperty property, string assemblyName, string typeName)
    {
      return base.GetTypeWithMatchingVersionNumber (property, assemblyName, typeName);
    }

    public new Type GetType (ConfigurationProperty property, AssemblyName assemblyName, string typeName)
    {
      return base.GetType (property, assemblyName, typeName);
    }

    public new void InstantiateProviders (
        ProviderSettingsCollection providerSettingsCollection,
        ProviderCollection providerCollection,
        Type providerType,
        params Type[] providerInterfaces)
    {
      base.InstantiateProviders (providerSettingsCollection, providerCollection, providerType, providerInterfaces);
    }

    public new ExtendedProviderBase InstantiateProvider (ProviderSettings providerSettings, Type providerType, params Type[] providerInterfaces)
    {
      return base.InstantiateProvider (providerSettings, providerType, providerInterfaces);
    }

    protected override void EnsureWellKownProviders (System.Configuration.Provider.ProviderCollection collection)
    {
      base.EnsureWellKownProviders (collection);

      collection.Add (new FakeWellKnownProvider ("WellKnown", new NameValueCollection()));
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty()
    {
      return CreateDefaultProviderNameProperty ("defaultProvider", "Default Value");
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty()
    {
      return CreateProviderSettingsProperty ("providers");
    }
  }
}