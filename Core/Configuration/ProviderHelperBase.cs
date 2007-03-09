using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.IO;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Configuration
{
  /// <summary>Abstract base class for <see cref="ProviderHelperBase{T}"/>.</summary>
  public abstract class ProviderHelperBase
  {
    /// <summary>Initializes properties and adds them to the given <see cref="ConfigurationPropertyCollection"/>.</summary>
    public abstract void InitializeProperties (ConfigurationPropertyCollection properties);

    public abstract void PostDeserialze();
  }

  /// <summary>Base for helper classes that load specific providers from the <see cref="System.Configuration.ConfigurationSection"/> section.</summary>
  public abstract class ProviderHelperBase<TProvider>: ProviderHelperBase where TProvider: class
  {
    private readonly ExtendedConfigurationSection _configurationSection;
    private readonly DoubleCheckedLockingContainer<TProvider> _provider;
    private readonly DoubleCheckedLockingContainer<ProviderCollection> _providers;
    private ConfigurationProperty _providerSettingsProperty;
    private ConfigurationProperty _defaultProviderNameProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderHelperBase{TProvider}"/> class. 
    /// </summary>
    /// <param name="configurationSection">
    /// The <see cref="System.Configuration.ConfigurationSection"/> holding the <see cref="ProviderSettings"/> 
    /// loaded from the security configuration section for <see cref="Rubicon.Security"/>
    /// </param>
    protected ProviderHelperBase (ExtendedConfigurationSection configurationSection)
    {
      ArgumentUtility.CheckNotNull ("configurationSection", configurationSection);

      _configurationSection = configurationSection;
      _provider = new DoubleCheckedLockingContainer<TProvider> (delegate { return GetProviderFromConfiguration(); });
      _providers = new DoubleCheckedLockingContainer<ProviderCollection> (delegate { return GetProvidersFromConfiguration(); });
    }

    protected abstract ConfigurationProperty CreateDefaultProviderNameProperty();

    protected abstract ConfigurationProperty CreateProviderSettingsProperty();

    /// <summary>Initializes properties and adds them to the given <see cref="ConfigurationPropertyCollection"/>.</summary>
    public override void InitializeProperties (ConfigurationPropertyCollection properties)
    {
      ArgumentUtility.CheckNotNull ("properties", properties);

      _providerSettingsProperty = CreateProviderSettingsProperty();
      _defaultProviderNameProperty = CreateDefaultProviderNameProperty();

      properties.Add (_providerSettingsProperty);
      properties.Add (_defaultProviderNameProperty);
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

    protected ExtendedConfigurationSection ConfigurationSection
    {
      get { return _configurationSection; }
    }

    protected ProviderSettingsCollection ProviderSettings
    {
      get { return (ProviderSettingsCollection) _configurationSection[_providerSettingsProperty]; }
    }

    protected string DefaultProviderName
    {
      get { return (string) _configurationSection[DefaultProviderNameProperty]; }
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

      AssemblyName frameworkAssemblyName = typeof (ExtendedConfigurationSection).Assembly.GetName();
      AssemblyName realAssemblyName = new AssemblyName (frameworkAssemblyName.FullName);
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
            _configurationSection.ElementInformation.Properties[property.Name],
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
            _configurationSection.ElementInformation.Properties[DefaultProviderNameProperty.Name],
            "The provider '{0}' specified for the {1} does not exist in the providers collection.",
            DefaultProviderName,
            DefaultProviderNameProperty.Name);
      }

      return (TProvider) (object) Providers[DefaultProviderName];
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