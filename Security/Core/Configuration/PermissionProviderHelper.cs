using System;
using System.Configuration;
using System.Configuration.Provider;
using Rubicon.Security.Metadata;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  public class PermissionProviderHelper : ProviderHelperBase<IPermissionProvider>
  {
    private const string c_permissionReflectorWellKnownName = "Reflection";

    public PermissionProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultPermissionProvider", c_permissionReflectorWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("permissionProviders");
    }

    protected override IPermissionProvider CastProviderBaseToProviderType (ProviderBase provider)
    {
      return (IPermissionProvider) provider;
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownReflectionPermissionProvider (collection);
    }

    private void EnsureWellKnownReflectionPermissionProvider (ProviderCollection collection)
    {
      EnsureWellKownProvider (collection, c_permissionReflectorWellKnownName, delegate { return new PermissionReflector(); });
    }
  }
}