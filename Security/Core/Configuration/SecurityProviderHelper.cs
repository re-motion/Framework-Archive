using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Reflection;
using Rubicon.Security;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  /// <summary>Helper class that loads implementations of <see cref="ISecurityProvider"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class SecurityProviderHelper : ProviderHelperBase<ISecurityProvider>
  {
    private const string c_nullSecurityProviderWellKnownName = "None";
    private const string c_securityManagerSecurityProviderWellKnownName = "SecurityManager";

    private readonly object _sync = new object();
    private Type _securityManagerSecurityServiceType;

    public SecurityProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultSecurityProvider", c_nullSecurityProviderWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("securityProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_nullSecurityProviderWellKnownName);
      CheckForDuplicateWellKownProviderName (c_securityManagerSecurityProviderWellKnownName);
      
      if (DefaultProviderName.Equals (c_securityManagerSecurityProviderWellKnownName, StringComparison.Ordinal))
        EnsureSecurityManagerServiceTypeInitialized();
    }

    protected override ISecurityProvider CastProviderBaseToProviderType (ProviderBase provider)
    {
      return (ISecurityProvider) provider;
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownNullSecurityProvider (collection);
      EnsureWellKnownSecurityManagerSecurityProvider (collection);
    }

    private void EnsureWellKnownNullSecurityProvider (ProviderCollection collection)
    {
      EnsureWellKownProvider (collection, c_nullSecurityProviderWellKnownName, delegate { return new NullSecurityProvider(); });
    }

    private void EnsureWellKnownSecurityManagerSecurityProvider (ProviderCollection collection)
    {
      if (_securityManagerSecurityServiceType != null)
      {
        EnsureWellKownProvider (
            collection,
            c_securityManagerSecurityProviderWellKnownName,
            delegate { return (ProviderBase) Activator.CreateInstance (_securityManagerSecurityServiceType); });
      }
    }

    private void EnsureSecurityManagerServiceTypeInitialized ()
    {
      if (_securityManagerSecurityServiceType == null)
      {
        lock (_sync)
        {
          if (_securityManagerSecurityServiceType == null)
          {
            _securityManagerSecurityServiceType = GetType (
                DefaultProviderNameProperty,
                new AssemblyName ("Rubicon.SecurityManager"),
                "Rubicon.SecurityManager.SecurityService");
          }
        }
      }
    }
  }
}