using System;
using System.Configuration;
using System.Configuration.Provider;
using Rubicon.Configuration;
using Rubicon.Security.Metadata;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  /// <summary> The configuration section for <see cref="Rubicon.Security"/>. </summary>
  public class SecurityConfiguration : ConfigurationSection
  {
    // types

    // static members

    private static SecurityConfiguration s_current;

    public static SecurityConfiguration Current
    {
      get
      {
        if (s_current == null)
        {
          lock (typeof (SecurityConfiguration))
          {
            if (s_current == null)
            {
              s_current = (SecurityConfiguration) ConfigurationManager.GetSection ("rubicon.security");
              if (s_current == null)
                s_current = new SecurityConfiguration();
            }
          }
        }

        return s_current;
      }
    }

    protected static void SetCurrent (SecurityConfiguration configuration)
    {
      lock (typeof (SecurityConfiguration))
      {
        s_current = configuration;
      }
    }

    // member fields

    private IFunctionalSecurityStrategy _functionalSecurityStrategy;

    private ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _xmlnsProperty;
    private readonly ConfigurationProperty _functionalSecurityStrategyProperty;

    private PermissionProviderHelper _permissionProviderHelper;
    private SecurityProviderHelper _securityServiceHelper;
    private UserProviderHelper _userProviderHelper;
    private GlobalAccessTypeCacheProviderHelper _globalAccessTypeCacheProviderHelper;

    // construction and disposing

    public SecurityConfiguration ()
    {
      _permissionProviderHelper = new PermissionProviderHelper (this);
      _securityServiceHelper = new SecurityProviderHelper (this);
      _userProviderHelper = new UserProviderHelper (this);
      _globalAccessTypeCacheProviderHelper = new GlobalAccessTypeCacheProviderHelper (this);

      _xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);

      _functionalSecurityStrategyProperty = new ConfigurationProperty (
          "functionalSecurityStrategy",
          typeof (TypeElement<IFunctionalSecurityStrategy, FunctionalSecurityStrategy>),
          null,
          ConfigurationPropertyOptions.None);

      _properties = new ConfigurationPropertyCollection();
      _properties.Add (_xmlnsProperty);
      _permissionProviderHelper.InitializeProperties (_properties);
      _securityServiceHelper.InitializeProperties (_properties);
      _userProviderHelper.InitializeProperties (_properties);
      _globalAccessTypeCacheProviderHelper.InitializeProperties (_properties);
      _properties.Add (_functionalSecurityStrategyProperty);
    }

    // methods and properties

    protected override void PostDeserialize ()
    {
      base.PostDeserialize();

      _permissionProviderHelper.PostDeserialze();
      _securityServiceHelper.PostDeserialze();
      _userProviderHelper.PostDeserialze();
      _globalAccessTypeCacheProviderHelper.PostDeserialze();
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    protected internal new object this [ConfigurationProperty property]
    {
      get { return base[property]; }
      set { base[property] = value; }
    }

    public ISecurityService SecurityService
    {
      get { return _securityServiceHelper.Provider; }
      set { _securityServiceHelper.Provider = value; }
    }

    public ProviderCollection SecurityServices
    {
      get { return _securityServiceHelper.Providers; }
    }

    public IUserProvider UserProvider
    {
      get { return _userProviderHelper.Provider; }
      set { _userProviderHelper.Provider = value; }
    }

    public ProviderCollection UserProviders
    {
      get { return _userProviderHelper.Providers; }
    }


    public IPermissionProvider PermissionProvider
    {
      get { return _permissionProviderHelper.Provider; }
      set { _permissionProviderHelper.Provider = value; }
    }

    public ProviderCollection PermissionProviders
    {
      get { return _permissionProviderHelper.Providers; }
    }


    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get
      {
        if (_functionalSecurityStrategy == null)
          _functionalSecurityStrategy = FunctionalSecurityStrategyProperty.CreateInstance();

        return _functionalSecurityStrategy;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _functionalSecurityStrategy = value;
      }
    }

    protected TypeElement<IFunctionalSecurityStrategy> FunctionalSecurityStrategyProperty
    {
      get { return (TypeElement<IFunctionalSecurityStrategy>) this[_functionalSecurityStrategyProperty]; }
      set { this[_functionalSecurityStrategyProperty] = value; }
    }


    public IGlobalAccessTypeCacheProvider GlobalAccessTypeCacheProvider
    {
      get { return _globalAccessTypeCacheProviderHelper.Provider; }
      set { _globalAccessTypeCacheProviderHelper.Provider = value; }
    }

    public ProviderCollection GlobalAccessTypeCacheProviders
    {
      get { return _globalAccessTypeCacheProviderHelper.Providers; }
    }
  }
}