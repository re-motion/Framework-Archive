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
  public abstract class ProviderHelperBase<TProvider>
  {
    private SecurityConfiguration _configuration;
    private TProvider _provider;
    private ProviderCollection _providers;
    private readonly object _lockProviders = new object();
    private readonly object _lockProvider = new object();
    private ConfigurationProperty _providerSettingsProperty;
    private ConfigurationProperty _defaultProviderNameProperty;

    public ProviderHelperBase (SecurityConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _configuration = configuration;
    }

    protected abstract ConfigurationProperty CreateDefaultProviderNameProperty ();

    protected abstract ConfigurationProperty CreateProviderSettingsProperty ();

    protected abstract TProvider CastProviderBaseToProviderType (ProviderBase provider);

    public void InitializeProperties (ConfigurationPropertyCollection properties)
    {
      ArgumentUtility.CheckNotNull ("properties", properties);

      _providerSettingsProperty = CreateProviderSettingsProperty();
      _defaultProviderNameProperty = CreateDefaultProviderNameProperty();

      properties.Add (DefaultProviderNameProperty);
      properties.Add (_providerSettingsProperty);
    }

    public virtual void PostDeserialze ()
    {
    }

    public TProvider Provider
    {
      get
      {
        if (_provider == null)
        {
          lock (_lockProvider)
          {
            if (_provider == null)
              _provider = GetProviderFromConfiguration();
          }
        }
        return _provider;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        lock (_lockProvider)
        {
          _provider = value;
        }
      }
    }

    public ProviderCollection Providers
    {
      get
      {
        if (_providers == null)
        {
          lock (_lockProviders)
          {
            if (_providers == null)
              _providers = GetProvidersFromConfiguration();
          }
        }

        return _providers;
      }
    }

    protected SecurityConfiguration Configuration
    {
      get
      {
        return _configuration;
      }
    }

    protected ProviderSettingsCollection ProviderSettings
    {
      get
      {
        return (ProviderSettingsCollection) Configuration[_providerSettingsProperty];
      }
    }

    protected string DefaultProviderName
    {
      get
      {
        return (string) Configuration[DefaultProviderNameProperty];
      }
    }

    protected ConfigurationProperty DefaultProviderNameProperty
    {
      get
      {
        return _defaultProviderNameProperty;
      }
    }

    protected ConfigurationProperty CreateDefaultProviderNameProperty (string name, string defaultValue)
    {
      return new ConfigurationProperty (name, typeof (string), defaultValue, null, new StringValidator (1), ConfigurationPropertyOptions.None);
    }

    protected ConfigurationProperty CreateProviderSettingsProperty (string name)
    {
      return new ConfigurationProperty (name, typeof (ProviderSettingsCollection), null, ConfigurationPropertyOptions.None);
    }

    protected virtual void EnsureWellKownProviders (ProviderCollection collection)
    {
    }

    protected void EnsureWellKownProvider (ProviderCollection collection, string wellKnownName, Func<ProviderBase> createMethod)
    {
      if (ProviderSettings[wellKnownName] == null)
      {
        ProviderBase provider = createMethod ();
        provider.Initialize (wellKnownName, new NameValueCollection ());
        collection.Add (provider);
      }
    }

    protected Type GetTypeWithMatchingVersionNumber (string assemblyName, string typeName, ConfigurationProperty property)
    {
      AssemblyName securityAssemblyName = typeof (SecurityConfiguration).Assembly.GetName();
      AssemblyName realAssemblyName = new AssemblyName (securityAssemblyName.FullName);
      realAssemblyName.Name = assemblyName;

      return GetType (property, realAssemblyName, typeName);
    }

    protected Type GetType (ConfigurationProperty property, AssemblyName assemblyName, string typeName)
    {
      try
      {
        Assembly.Load (assemblyName);
      }
      catch (FileNotFoundException e)
      {
        PropertyInformation propertyInformation = Configuration.ElementInformation.Properties[property.Name];
        throw new ConfigurationErrorsException (
            string.Format (
                "The current value of property '{0}' requires that the assembly '{1}' is placed "
                    + "within the CLR's probing path for this application.",
                property.Name,
                assemblyName.FullName),
            e,
            propertyInformation.Source,
            propertyInformation.LineNumber);
      }

      return Type.GetType (Assembly.CreateQualifiedName (assemblyName.FullName, typeName), true, false);
    }

    private TProvider GetProviderFromConfiguration ()
    {
      if (Providers[DefaultProviderName] == null)
      {
        throw new ConfigurationErrorsException (
            string.Format (
                "The provider '{0}' specified for the {1} does not exist in the providers collection.",
                DefaultProviderName,
                DefaultProviderNameProperty.Name),
            Configuration.ElementInformation.Properties[DefaultProviderNameProperty.Name].Source,
            Configuration.ElementInformation.Properties[DefaultProviderNameProperty.Name].LineNumber);
      }

      return CastProviderBaseToProviderType (Providers[DefaultProviderName]);
    }

    private ProviderCollection GetProvidersFromConfiguration ()
    {
      ProviderCollection collection = new ProviderCollection ();
      EnsureWellKownProviders (collection);
      ProviderHelper.InstantiateProviders (ProviderSettings, collection, typeof (ProviderBase), typeof (TProvider));
      collection.SetReadOnly ();

      return collection;
    }
  }
}