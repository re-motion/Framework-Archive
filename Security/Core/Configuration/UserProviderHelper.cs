using System;
using System.Configuration;
using System.Configuration.Provider;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  /// <summary>Helper class that loads implementations of <see cref="IUserProvider"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class UserProviderHelper : ProviderHelperBase<IUserProvider>
  {
    private const string c_threadUserProviderWellKnownName = "Thread";
    private const string c_httpContexUserProviderWellKnownName = "HttpContext";

    private readonly object _lock = new object();
    private Type _httpContextUserProviderType;

    public UserProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultUserProvider", c_threadUserProviderWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("userProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_threadUserProviderWellKnownName);
      CheckForDuplicateWellKownProviderName (c_httpContexUserProviderWellKnownName);

      if (DefaultProviderName.Equals (c_httpContexUserProviderWellKnownName, StringComparison.Ordinal))
        EnsureHttpContextUserProviderTypeInitialized();
    }

    protected override IUserProvider CastProviderBaseToProviderType (ProviderBase provider)
    {
      return (IUserProvider) provider;
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownThreadUserProvider (collection);
      EnsureWellKnownHttpContextUserProvider (collection);
    }

    private void EnsureWellKnownThreadUserProvider (ProviderCollection collection)
    {
      EnsureWellKownProvider (collection, c_threadUserProviderWellKnownName, delegate { return new ThreadUserProvider(); });
    }

    private void EnsureWellKnownHttpContextUserProvider (ProviderCollection collection)
    {
      if (_httpContextUserProviderType != null)
      {
        EnsureWellKownProvider (
            collection,
            c_httpContexUserProviderWellKnownName,
            delegate { return (ProviderBase) Activator.CreateInstance (_httpContextUserProviderType); });
      }
    }

    private void EnsureHttpContextUserProviderTypeInitialized ()
    {
      if (_httpContextUserProviderType == null)
      {
        lock (_lock)
        {
          if (_httpContextUserProviderType == null)
          {
            _httpContextUserProviderType = GetTypeWithMatchingVersionNumber (
                DefaultProviderNameProperty,
                "Rubicon.Security.Web",
                "Rubicon.Security.Web.HttpContextUserProvider");
          }
        }
      }
    }
  }
}