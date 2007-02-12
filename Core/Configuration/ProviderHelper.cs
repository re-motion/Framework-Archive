using System;
using System.Configuration;
using System.Configuration.Provider;
using Rubicon.Utilities;

namespace Rubicon.Configuration
{
  /// <summary>
  /// Provides methods for creating provider instances, either individually or in a batch.
  /// </summary>
  public static class ProviderHelper
  {
    /// <summary>Initializes a collection of providers of the given type using the supplied settings.</summary>
    /// <param name="providerSettingsCollection">A collection of settings to be passed to the provider upon initialization.</param>
    /// <param name="providerCollection">The collection used to contain the initialized providers after the method returns.</param>
    /// <param name="providerType">The <see cref="Type"/> of the providers to be initialized.</param>
    /// <param name="providerInterfaces">The list of interfaces each provider must implement.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="providerSettingsCollection" /> is null.<para>- or -</para>
    /// <paramref name="providerCollection" /> is null.<para>- or -</para>
    /// <paramref name="providerType"/> is null.
    /// </exception>
    public static void InstantiateProviders (
        ProviderSettingsCollection providerSettingsCollection, 
        ProviderCollection providerCollection, 
        Type providerType,
        params Type[] providerInterfaces)
    {
      ArgumentUtility.CheckNotNull ("providerSettingsCollection", providerSettingsCollection);
      ArgumentUtility.CheckNotNull ("providerCollection", providerCollection);
      ArgumentUtility.CheckNotNull ("providerType", providerType);

      foreach (ProviderSettings providerSettings in providerSettingsCollection)
        providerCollection.Add (InstantiateProvider (providerSettings, providerType, providerInterfaces));
    }

    /// <summary>Initializes and returns a single provider of the given type using the supplied settings.</summary>
    /// <param name="providerSettings">The settings to be passed to the provider upon initialization.</param>
    /// <param name="providerType">The <see cref="Type"/> of the providers to be initialized.</param>
    /// <param name="providerInterfaces">The list of interfaces each provider must implement.</param>
    /// <returns>A new provider of the given type using the supplied settings.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="providerSettings" /> is null.<para>- or -</para>
    /// <paramref name="providerType"/> is null.
    /// </exception>
    /// <exception cref="ConfigurationErrorsException">
    /// The provider threw an exception while it was being initialized.<para>- or -</para>
    /// An error occurred while attempting to resolve a <see cref="Type"/> instance for the provider specified in <paramref name="providerSettings"/>.
    /// </exception>
    public static ProviderBase InstantiateProvider (ProviderSettings providerSettings, Type providerType, params Type[] providerInterfaces)
    {
      ArgumentUtility.CheckNotNull ("providerSettings", providerSettings);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("providerType", providerType, typeof (ProviderBase));
      ArgumentUtility.CheckNotNullOrItemsNull ("providerInterfaces", providerInterfaces);

      try
      {
        ProviderBase providerBase = CreateProvider (providerSettings, providerType, providerInterfaces);
        providerBase.Initialize (providerSettings.Name, NameValueCollectionUtility.Clone (providerSettings.Parameters));
        return providerBase;
      }
      catch (Exception e)
      {
        if (e is ConfigurationException)
          throw;

        throw new ConfigurationErrorsException (
            e.Message,
            providerSettings.ElementInformation.Properties["type"].Source,
            providerSettings.ElementInformation.Properties["type"].LineNumber);
      }
    }

    private static ProviderBase CreateProvider (ProviderSettings providerSettings, Type providerType, Type[] providerInterfaces)
    {
      if (string.IsNullOrEmpty (providerSettings.Type))
        throw new ArgumentException ("Type name must be specified for this provider.");
      
      Type actualType = TypeUtility.GetType (providerSettings.Type);
      
      if (!providerType.IsAssignableFrom (actualType))
        throw new ArgumentException (string.Format ("Provider must implement the class '{0}'.", providerType.FullName));

      foreach (Type interfaceType in providerInterfaces)
      {
        if (!interfaceType.IsAssignableFrom (actualType))
          throw new ArgumentException (string.Format ("Provider must implement the interface '{0}'.", interfaceType.FullName));
      }

      return (ProviderBase) Activator.CreateInstance (actualType);
    }
  }
}