using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.IO;
using System.Text;
using System.Reflection;

using Rubicon.Configuration;
using Rubicon.Security.Metadata;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  public class SecurityConfiguration : ConfigurationSection
  {
    // types

    // static members

    private const string c_permissionReflectorWellKnownName = "PermissionReflector";
    
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
                s_current = new SecurityConfiguration ();
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

    private ISecurityService _securityService;
    private IUserProvider _userProvider;
    private IFunctionalSecurityStrategy _functionalSecurityStrategy;
    private IPermissionProvider _permissionProvider;
    private IGlobalAccessTypeCacheProvider _globalAccessTypeCacheProvider;

    private ProviderCollection _permissionProviders;

    private ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _xmlnsProperty;
    private readonly ConfigurationProperty _customSecurityServiceProperty;
    private readonly ConfigurationProperty _securityServiceTypeProperty;
    private readonly ConfigurationProperty _customUserProviderProperty;
    private readonly ConfigurationProperty _userProviderTypeProperty;
    private readonly ConfigurationProperty _defaultPermissionProviderNameProperty;
    private readonly ConfigurationProperty _permissionProviderSettingsProperty;
    private readonly ConfigurationProperty _customFunctionalSecurityStrategyProperty;
    private readonly ConfigurationProperty _customGlobalAccessTypeCacheProviderProperty;
    private readonly ConfigurationProperty _globalAccessTypeCacheProviderTypeProperty;
    private Type _httpContextUserProviderType;
    private Type _securityManagerServiceType;

    private readonly object _lock = new object ();
    private readonly object _lockPermissionProvider = new object ();

    // construction and disposing

    public SecurityConfiguration ()
    {
      _xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);

      _customSecurityServiceProperty = new ConfigurationProperty (
          "customService", typeof (TypeElement<ISecurityService>), null, ConfigurationPropertyOptions.None);

      _securityServiceTypeProperty = new ConfigurationProperty (
          "service", typeof (SecurityServiceType), SecurityServiceType.None, ConfigurationPropertyOptions.None);

      _customUserProviderProperty = new ConfigurationProperty (
          "customUserProvider", typeof (TypeElement<IUserProvider>), null, ConfigurationPropertyOptions.None);

      _userProviderTypeProperty = new ConfigurationProperty (
          "userProvider", typeof (UserProviderType), UserProviderType.Thread, ConfigurationPropertyOptions.None);

      _defaultPermissionProviderNameProperty = new ConfigurationProperty (
          "defaultPermissionProvider", 
          typeof (string), 
          c_permissionReflectorWellKnownName, 
          null, 
          new StringValidator (1), 
          ConfigurationPropertyOptions.None);

      _permissionProviderSettingsProperty = new ConfigurationProperty (
          "permissionProviders", typeof (ProviderSettingsCollection), null, ConfigurationPropertyOptions.None);

      _customFunctionalSecurityStrategyProperty = new ConfigurationProperty (
          "customFunctionalSecurityStrategy",
          typeof (TypeElement<IFunctionalSecurityStrategy>),
          null,
          ConfigurationPropertyOptions.None);

      _customGlobalAccessTypeCacheProviderProperty = new ConfigurationProperty (
          "customGlobalAccessTypeCacheProvider",
          typeof (TypeElement<IGlobalAccessTypeCacheProvider>),
          null,
          ConfigurationPropertyOptions.None);

      _globalAccessTypeCacheProviderTypeProperty = new ConfigurationProperty (
          "globalAccessTypeCacheProvider",
          typeof (GlobalAccessTypeCacheProviderType),
          GlobalAccessTypeCacheProviderType.None,
          ConfigurationPropertyOptions.None);

      _properties = new ConfigurationPropertyCollection ();
      _properties.Add (_xmlnsProperty);
      _properties.Add (_customSecurityServiceProperty);
      _properties.Add (_securityServiceTypeProperty);
      _properties.Add (_customUserProviderProperty);
      _properties.Add (_userProviderTypeProperty);
      _properties.Add (_defaultPermissionProviderNameProperty);
      _properties.Add (_permissionProviderSettingsProperty);
      _properties.Add (_customFunctionalSecurityStrategyProperty);
      _properties.Add (_customGlobalAccessTypeCacheProviderProperty);
      _properties.Add (_globalAccessTypeCacheProviderTypeProperty);
    }

    // methods and properties

    protected override void PostDeserialize ()
    {
      base.PostDeserialize ();

      if (SecurityServiceType == SecurityServiceType.SecurityManagerService)
        EnsureSecurityManagerServiceTypeInitialized ();

      if (UserProviderType == UserProviderType.HttpContext)
        EnsureHttpContextUserProviderTypeInitialized ();
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }


    public ISecurityService SecurityService
    {
      get
      {
        if (_securityService == null)
          _securityService = GetSecurityServiceFromConfiguration ();

        return _securityService;
      }
      set
      {
        _securityService = value;
      }
    }

    protected TypeElement<ISecurityService> CustomService
    {
      get { return (TypeElement<ISecurityService>) this[_customSecurityServiceProperty]; }
      set { this[_customSecurityServiceProperty] = value; }
    }

    protected SecurityServiceType SecurityServiceType
    {
      get { return (SecurityServiceType) this[_securityServiceTypeProperty]; }
      set { this[_securityServiceTypeProperty] = value; }
    }

    private ISecurityService GetSecurityServiceFromConfiguration ()
    {
      switch (SecurityServiceType)
      {
        case SecurityServiceType.None:
          return null;
        case SecurityServiceType.SecurityManagerService:
          EnsureSecurityManagerServiceTypeInitialized ();
          return (ISecurityService) Activator.CreateInstance (_securityManagerServiceType);
        case SecurityServiceType.Custom:
          return (ISecurityService) Activator.CreateInstance (CustomService.Type);
        default:
          throw new InvalidOperationException (string.Format ("Choice '{0}' is not supported when selecting the SecurityService.", SecurityServiceType));
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
                _securityServiceTypeProperty,
                new AssemblyName ("Rubicon.SecurityManager"),
                "Rubicon.SecurityManager.SecurityService");
          }
        }
      }
    }


    public IUserProvider UserProvider
    {
      get
      {
        if (_userProvider == null)
          _userProvider = GetUserProviderFromConfiguration ();

        return _userProvider;
      }
      set
      {
        _userProvider = value;
      }
    }

    protected TypeElement<IUserProvider> CustomUserProvider
    {
      get { return (TypeElement<IUserProvider>) this[_customUserProviderProperty]; }
      set { this[_customUserProviderProperty] = value; }
    }

    protected UserProviderType UserProviderType
    {
      get { return (UserProviderType) this[_userProviderTypeProperty]; }
      set { this[_userProviderTypeProperty] = value; }
    }

    private IUserProvider GetUserProviderFromConfiguration ()
    {
      switch (UserProviderType)
      {
        case UserProviderType.None:
          return null;
        case UserProviderType.Thread:
          return new ThreadUserProvider ();
        case UserProviderType.HttpContext:
          EnsureHttpContextUserProviderTypeInitialized ();
          return (IUserProvider) Activator.CreateInstance (_httpContextUserProviderType);
        case UserProviderType.Custom:
          return (IUserProvider) Activator.CreateInstance (CustomUserProvider.Type);
        default:
          throw new InvalidOperationException (string.Format ("Choice '{0}' is not supported when selecting the UserProvider.", UserProviderType));
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
                "Rubicon.Security.Web",
                "Rubicon.Security.Web.HttpContextUserProvider",
                _userProviderTypeProperty);
          }
        }
      }
    }


    public IPermissionProvider PermissionProvider
    {
      get
      {
        if (_permissionProvider == null)
          _permissionProvider = GetPermissionProviderFromConfiguration ();

        return _permissionProvider;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _permissionProvider = value;
      }
    }

    public ProviderCollection PermissionProviders
    {
      get
      {
        if (_permissionProviders == null)
        {
          lock (_lockPermissionProvider)
          {
            if (_permissionProviders == null)
              _permissionProviders = GetPermissionProvidersFromConfiguration ();
          }
        }

        return _permissionProviders;
      }
    }

    protected ProviderSettingsCollection PermissionProviderSettings
    {
      get { return (ProviderSettingsCollection) base[_permissionProviderSettingsProperty]; }
    }

    protected string DefaultPermissionProviderName
    {
      get { return (string) this[_defaultPermissionProviderNameProperty]; }
      set { this[_defaultPermissionProviderNameProperty] = value; }
    }

    private IPermissionProvider GetPermissionProviderFromConfiguration ()
    {
      if (PermissionProviders[DefaultPermissionProviderName] == null)
        throw new ConfigurationErrorsException (string.Format ("The provider '{0}' specified for the defaultPermissionProvider does not exist in the providers collection.", DefaultPermissionProviderName), ElementInformation.Properties[_defaultPermissionProviderNameProperty.Name].Source, ElementInformation.Properties[_defaultPermissionProviderNameProperty.Name].LineNumber);

      return (IPermissionProvider) PermissionProviders[DefaultPermissionProviderName];
    }

    private ProviderCollection GetPermissionProvidersFromConfiguration ()
    {
      ProviderCollection collection = new ProviderCollection ();
      EnsureWellKnownPermissionProvider (collection);
      ProviderHelper.InstantiateProviders (PermissionProviderSettings, collection, typeof (ProviderBase), typeof (IPermissionProvider));
      collection.SetReadOnly ();
      
      return collection;
    }

    private void EnsureWellKnownPermissionProvider (ProviderCollection collection)
    {
      if (PermissionProviderSettings[c_permissionReflectorWellKnownName] == null)
      {
        PermissionReflector permissionReflector = new PermissionReflector ();
        permissionReflector.Initialize (c_permissionReflectorWellKnownName, new NameValueCollection ());
        collection.Add (permissionReflector);
      }
    }


    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get
      {
        if (_functionalSecurityStrategy == null)
          _functionalSecurityStrategy = GetFunctionalSecurityStrategyFromConfiguration ();

        return _functionalSecurityStrategy;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _functionalSecurityStrategy = value;
      }
    }

    protected TypeElement<IFunctionalSecurityStrategy> CustomFunctionalSecurityStrategy
    {
      get { return (TypeElement<IFunctionalSecurityStrategy>) this[_customFunctionalSecurityStrategyProperty]; }
      set { this[_customFunctionalSecurityStrategyProperty] = value; }
    }

    private IFunctionalSecurityStrategy GetFunctionalSecurityStrategyFromConfiguration ()
    {
      Type customFunctionalSecurityStrategyType = CustomFunctionalSecurityStrategy.Type;
      if (customFunctionalSecurityStrategyType == null)
        return new FunctionalSecurityStrategy ();

      return (IFunctionalSecurityStrategy) Activator.CreateInstance (customFunctionalSecurityStrategyType);
    }


    public IGlobalAccessTypeCacheProvider GlobalAccessTypeCacheProvider
    {
      get
      {
        if (_globalAccessTypeCacheProvider == null)
          _globalAccessTypeCacheProvider = GetGlobalAccessTypeCacheProviderFromConfiguration ();

        return _globalAccessTypeCacheProvider;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _globalAccessTypeCacheProvider = value;
      }
    }

    protected TypeElement<IGlobalAccessTypeCacheProvider> CustomGlobalAccessTypeCacheProvider
    {
      get { return (TypeElement<IGlobalAccessTypeCacheProvider>) this[_customGlobalAccessTypeCacheProviderProperty]; }
      set { this[_customGlobalAccessTypeCacheProviderProperty] = value; }
    }

    protected GlobalAccessTypeCacheProviderType GlobalAccessTypeCacheProviderType
    {
      get { return (GlobalAccessTypeCacheProviderType) this[_globalAccessTypeCacheProviderTypeProperty]; }
      set { this[_globalAccessTypeCacheProviderTypeProperty] = value; }
    }

    private IGlobalAccessTypeCacheProvider GetGlobalAccessTypeCacheProviderFromConfiguration ()
    {
      switch (GlobalAccessTypeCacheProviderType)
      {
        case GlobalAccessTypeCacheProviderType.None:
          return new NullGlobalAccessTypeCacheProvider ();
        case GlobalAccessTypeCacheProviderType.RevisionBased:
          return new RevisionBasedAccessTypeCacheProvider ();
        case GlobalAccessTypeCacheProviderType.Custom:
          return (IGlobalAccessTypeCacheProvider) Activator.CreateInstance (CustomGlobalAccessTypeCacheProvider.Type);
        default:
          throw new InvalidOperationException (string.Format ("Choice '{0}' is not supported when selecting the GlobalAccessTypeCacheProvider.", GlobalAccessTypeCacheProviderType));
      }
    }


    private Type GetTypeWithMatchingVersionNumber (string assemblyName, string typeName, ConfigurationProperty property)
    {
      AssemblyName securityAssemblyName = typeof (SecurityConfiguration).Assembly.GetName ();
      AssemblyName realAssemblyName = new AssemblyName (securityAssemblyName.FullName);
      realAssemblyName.Name = assemblyName;

      return GetType (property, realAssemblyName, typeName);
    }

    private Type GetType (ConfigurationProperty property, AssemblyName assemblyName, string typeName)
    {
      try
      {
        Assembly.Load (assemblyName);
      }
      catch (FileNotFoundException e)
      {
        PropertyInformation propertyInformation = ElementInformation.Properties[property.Name];
        throw new ConfigurationErrorsException (string.Format ("The current value of property '{0}' requires that the assembly '{1}' is placed "
                + "within the CLR's probing path for this application.", property.Name, assemblyName.FullName),
            e,
            propertyInformation.Source,
            propertyInformation.LineNumber);
      }

      return Type.GetType (Assembly.CreateQualifiedName (assemblyName.FullName, typeName), true, false);
    }
  }

  public enum SecurityServiceType
  {
    None,
    SecurityManagerService,
    //SecurityManagerWebService,
    Custom
  }

  public enum UserProviderType
  {
    None,
    Thread,
    HttpContext,
    Custom
  }

  public enum GlobalAccessTypeCacheProviderType
  {
    None,
    RevisionBased,
    Custom
  }
}
