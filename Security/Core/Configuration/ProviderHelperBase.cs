using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.IO;
using System.Reflection;
using Rubicon.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  /// <summary>Abstract base class for <see cref="ProviderHelperBase{T}"/>.</summary>
  public abstract class ProviderHelperBase
  {
    /// <summary>Initializes properties and adds them to the given <see cref="ConfigurationPropertyCollection"/>.</summary>
    public abstract void InitializeProperties (ConfigurationPropertyCollection properties);

    public abstract void PostDeserialze ();
  }

  /// <summary>Base for helper classes that load specific providers from the <see cref="SecurityConfiguration"/> section.</summary>
  public abstract class ProviderHelperBase<TProvider> : ProviderHelperBase where TProvider : class
  {
    private readonly SecurityConfiguration _configuration;
    private readonly DoubleCheckedLockingContainer<TProvider> _provider;
    private readonly DoubleCheckedLockingContainer<ProviderCollection> _providers;
    private ConfigurationProperty _providerSettingsProperty;
    private ConfigurationProperty _defaultProviderNameProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderHelperBase{TProvider}"/> class. 
    /// </summary>
    /// <param name="configuration">
    /// The <see cref="SecurityConfiguration"/> holding the <see cref="ProviderSettings"/> 
    /// loaded from the security configuration section for <see cref="Rubicon.Security"/>
    /// </param>
    protected ProviderHelperBase (SecurityConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _configuration = configuration;
      _provider = new DoubleCheckedLockingContainer<TProvider> (delegate { return GetProviderFromConfiguration(); });
      _providers = new DoubleCheckedLockingContainer<ProviderCollection> (delegate { return GetProvidersFromConfiguration(); });
    }

    protected abstract ConfigurationProperty CreateDefaultProviderNameProperty();

    protected abstract ConfigurationProperty CreateProviderSettingsProperty();

    protected abstract TProvider CastProviderBaseToProviderType (ProviderBase provider);

    /// <summary>Initializes properties and adds them to the given <see cref="ConfigurationPropertyCollection"/>.</summary>
    public override void InitializeProperties (ConfigurationPropertyCollection properties)
    {
      ArgumentUtility.CheckNotNull ("properties", properties);

      _providerSettingsProperty = CreateProviderSettingsProperty();
      _defaultProviderNameProperty = CreateDefaultProviderNameProperty();

      properties.Add (DefaultProviderNameProperty);
      properties.Add (_providerSettingsProperty);
    }

    public override void PostDeserialze()
    {
    }

    /// <summary>Get and set the provider.</summary>
    public TProvider Provider
    {
      get { return _provider.Value; }
      set { _provider.Value = value; }
    }

    public ProviderCollection Providers
    {
      get { return _providers.Value; }
    }

    protected SecurityConfiguration Configuration
    {
      get { return _configuration; }
    }

    protected ProviderSettingsCollection ProviderSettings
    {
      get { return (ProviderSettingsCollection) Configuration[_providerSettingsProperty]; }
    }

    protected string DefaultProviderName
    {
      get { return (string) Configuration[DefaultProviderNameProperty]; }
    }

    protected ConfigurationProperty DefaultProviderNameProperty
    {
      get { return _defaultProviderNameProperty; }
    }

    protected ConfigurationProperty CreateDefaultProviderNameProperty (string name, string defaultValue)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      return new ConfigurationProperty (name, typeof (string), defaultValue, null, new StringValidator (1), ConfigurationPropertyOptions.None);
    }

    protected ConfigurationProperty CreateProviderSettingsProperty (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      return new ConfigurationProperty (name, typeof (ProviderSettingsCollection), null, ConfigurationPropertyOptions.None);
    }

    protected virtual void EnsureWellKownProviders (ProviderCollection collection)
    {
    }

    protected void EnsureWellKownProvider (ProviderCollection collection, string wellKnownName, Func<ProviderBase> createMethod)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      ArgumentUtility.CheckNotNullOrEmpty ("wellKnownName", wellKnownName);
      ArgumentUtility.CheckNotNull ("createMethod", createMethod);

      ProviderBase provider = createMethod();
      provider.Initialize (wellKnownName, new NameValueCollection());
      collection.Add (provider);
    }

    protected void CheckForDuplicateWellKownProviderName (string wellKnownName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("wellKnownName", wellKnownName);

      if (ProviderSettings[wellKnownName] != null)
      {
        throw CreateConfigurationErrorsException (
            null,
            ProviderSettings[wellKnownName].ElementInformation.Properties["name"],
            "The name of the entry '{0}' identifies a well known provider and cannot be reused for custom providers.",
            wellKnownName);
      }
    }

    protected Type GetTypeWithMatchingVersionNumber (ConfigurationProperty property, string assemblyName, string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("assemblyName", assemblyName);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      ArgumentUtility.CheckNotNull ("property", property);

      AssemblyName securityAssemblyName = typeof (SecurityConfiguration).Assembly.GetName();
      AssemblyName realAssemblyName = new AssemblyName (securityAssemblyName.FullName);
      realAssemblyName.Name = assemblyName;

      return GetType (property, realAssemblyName, typeName);
    }

    protected Type GetType (ConfigurationProperty property, AssemblyName assemblyName, string typeName)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNull ("assemblyName", assemblyName);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      try
      {
        Assembly.Load (assemblyName);
      }
      catch (FileNotFoundException e)
      {
        throw CreateConfigurationErrorsException (
            e,
            Configuration.ElementInformation.Properties[property.Name],
            "The current value of property '{0}' requires that the assembly '{1}' is placed within the CLR's probing path for this application.",
            property.Name,
            assemblyName.FullName);
      }

      return Type.GetType (Assembly.CreateQualifiedName (assemblyName.FullName, typeName), true, false);
    }

    private TProvider GetProviderFromConfiguration()
    {
      if (Providers[DefaultProviderName] == null)
      {
        throw CreateConfigurationErrorsException (
            null,
            Configuration.ElementInformation.Properties[DefaultProviderNameProperty.Name],
            "The provider '{0}' specified for the {1} does not exist in the providers collection.",
            DefaultProviderName,
            DefaultProviderNameProperty.Name);
      }

      return CastProviderBaseToProviderType (Providers[DefaultProviderName]);
    }

    private ProviderCollection GetProvidersFromConfiguration()
    {
      ProviderCollection collection = new ProviderCollection();
      EnsureWellKownProviders (collection);
      ProviderHelper.InstantiateProviders (ProviderSettings, collection, typeof (ProviderBase), typeof (TProvider));
      collection.SetReadOnly();

      return collection;
    }

    private ConfigurationErrorsException CreateConfigurationErrorsException (
        FileNotFoundException e, PropertyInformation propertyInformation, string message, params object[] args)
    {
      return new ConfigurationErrorsException (string.Format (message, args), e, propertyInformation.Source, propertyInformation.LineNumber);
    }
  }
}