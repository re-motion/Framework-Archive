using System;
using System.Configuration;
using System.Configuration.Provider;
using Rubicon.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  /// <summary>Helper class that loads implementations of <see cref="IGlobalAccessTypeCacheProvider"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class GlobalAccessTypeCacheProviderHelper : ProviderHelperBase<IGlobalAccessTypeCacheProvider>
  {
    private const string c_nullGlobalAccessTypeCacheProviderWellKnownName = "None";
    private const string c_revisionBasedGlobalAccessTypeCacheProviderWellKnownName = "RevisionBased";

    public GlobalAccessTypeCacheProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultGlobalAccessTypeCacheProvider", c_nullGlobalAccessTypeCacheProviderWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("globalAccessTypeCacheProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_nullGlobalAccessTypeCacheProviderWellKnownName);
      CheckForDuplicateWellKownProviderName (c_revisionBasedGlobalAccessTypeCacheProviderWellKnownName);
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownNullGlobalAccessTypeCacheProvider (collection);
      EnsureWellKnownRevisionBasedGlobalAccessTypeCacheProvider (collection);
    }

    private void EnsureWellKnownNullGlobalAccessTypeCacheProvider (ProviderCollection collection)
    {
      EnsureWellKownProvider (collection, c_nullGlobalAccessTypeCacheProviderWellKnownName, delegate { return new NullGlobalAccessTypeCacheProvider (); });
    }

    private void EnsureWellKnownRevisionBasedGlobalAccessTypeCacheProvider (ProviderCollection collection)
    {
      EnsureWellKownProvider (collection, c_revisionBasedGlobalAccessTypeCacheProviderWellKnownName, delegate { return new RevisionBasedAccessTypeCacheProvider (); });
    }
  }
}