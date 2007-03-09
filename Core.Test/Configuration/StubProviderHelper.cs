using System;
using System.Configuration;
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

    protected override void EnsureWellKownProviders (System.Configuration.Provider.ProviderCollection collection)
    {
      base.EnsureWellKownProviders (collection);

      EnsureWellKownProvider (collection, "WellKnown", delegate { return new FakeWellKnownProvider(); });
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