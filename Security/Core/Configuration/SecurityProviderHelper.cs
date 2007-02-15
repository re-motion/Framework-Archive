using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  /// <summary>Helper class that loads implementations of <see cref="ISecurityService"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class SecurityProviderHelper : ProviderHelperBase<ISecurityService>
  {
    private const string c_nullSecurityServiceWellKnownName = "None";
    private const string c_securityManagerSecurityServiceWellKnownName = "SecurityManager";

    private readonly object _lock = new object();
    private Type _securityManagerServiceType;

    public SecurityProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultSecurityProvider", c_nullSecurityServiceWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("securityProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_nullSecurityServiceWellKnownName);
      CheckForDuplicateWellKownProviderName (c_securityManagerSecurityServiceWellKnownName);
      
      if (DefaultProviderName.Equals (c_securityManagerSecurityServiceWellKnownName, StringComparison.Ordinal))
        EnsureSecurityManagerServiceTypeInitialized();
    }

    protected override ISecurityService CastProviderBaseToProviderType (ProviderBase provider)
    {
      return (ISecurityService) provider;
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownNullSecurityService (collection);
      EnsureWellKnownSecurityManagerSecurityService (collection);
    }

    private void EnsureWellKnownNullSecurityService (ProviderCollection collection)
    {
      EnsureWellKownProvider (collection, c_nullSecurityServiceWellKnownName, delegate { return new NullSecurityService(); });
    }

    private void EnsureWellKnownSecurityManagerSecurityService (ProviderCollection collection)
    {
      if (_securityManagerServiceType != null)
      {
        EnsureWellKownProvider (
            collection,
            c_securityManagerSecurityServiceWellKnownName,
            delegate { return (ProviderBase) Activator.CreateInstance (_securityManagerServiceType); });
      }
    }

    private void EnsureSecurityManagerServiceTypeInitialized ()
    {
      if (_securityManagerServiceType == null)
      {
        lock (_lock)
        {
          if (_securityManagerServiceType == null)
          {
            _securityManagerServiceType = GetType (
                DefaultProviderNameProperty,
                new AssemblyName ("Rubicon.SecurityManager"),
                "Rubicon.SecurityManager.SecurityService");
          }
        }
      }
    }
  }
}