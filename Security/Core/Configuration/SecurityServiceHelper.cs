using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Reflection;
using System.Security.Principal;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  public class SecuritySeviceHelper : ProviderHelperBase<ISecurityService>
  {
    private const string c_nullSecurityServiceWellKnownName = "None";
    private const string c_securityManagerSecurityServiceWellKnownName = "SecurityManager";

    private readonly object _lock = new object();
    private Type _securityManagerServiceType;

    public SecuritySeviceHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultSecurityService", c_nullSecurityServiceWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("securityServices");
    }

    public override void PostDeserialze ()
    {
      if (DefaultProviderName.Equals (c_securityManagerSecurityServiceWellKnownName, StringComparison.Ordinal))
        EnsureSecurityManagerServiceTypeInitialized ();
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
      EnsureWellKownProvider (collection, c_nullSecurityServiceWellKnownName, delegate { return new NullSecurityService (); });
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