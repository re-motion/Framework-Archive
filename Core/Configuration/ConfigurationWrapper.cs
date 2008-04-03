using System.Configuration;
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>Use the <see cref="ConfigurationWrapper"/> to encapsulate the access to the configiration data.</summary>
  /// <remarks>
  /// The .NET runtime does not provide an assignable well-known instance for the configuration. The <see cref="ConfigurationWrapper"/> is therefore
  /// the only option for injecting custom configuration data during design-time or for unit tests.
  /// </remarks>
  //TODO: Tests
  public abstract class ConfigurationWrapper
  {
    private static readonly DoubleCheckedLockingContainer<ConfigurationWrapper> s_current =
        new DoubleCheckedLockingContainer<ConfigurationWrapper> (delegate { return CreateFromConfigurationManager (); });

    public static ConfigurationWrapper CreateFromConfigurationObject (System.Configuration.Configuration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      return new ConfigurationWrapperFromConfigurationObject (configuration);
    }

    public static ConfigurationWrapper CreateFromConfigurationManager ()
    {
      return new ConfigurationWrapperFromConfigurationManager();
    }

    public static ConfigurationWrapper Current
    {
      get { return s_current.Value; }
    }

    public static void SetCurrent (ConfigurationWrapper configuration)
    {
      s_current.Value = configuration;
    }

    protected ConfigurationWrapper()
    {
    }

    public abstract object GetSection (string sectionName);

    public abstract ConnectionStringSettings GetConnectionString (string name);

    public abstract string GetAppSetting (string name);

    public object GetSection (string sectionName, bool throwIfNotFound)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sectionName", sectionName);

      object section = GetSection (sectionName);
      if (throwIfNotFound && section == null)
        throw new ConfigurationErrorsException (string.Format ("Required section '{0}' does not exist in the configuration.", sectionName));

      return section;
    }

    public ConnectionStringSettings GetConnectionString (string name, bool throwIfNotFound)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      ConnectionStringSettings connectionStringSettings = GetConnectionString (name);
      if (throwIfNotFound && connectionStringSettings == null)
        throw new ConfigurationErrorsException (string.Format ("Required connection string '{0}' does not exist in the configuration.", name));

      return connectionStringSettings;
    }

    public string GetAppSetting (string name, bool throwIfNotFound)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      string appSetting = GetAppSetting (name);
      if (throwIfNotFound && appSetting == null)
        throw new ConfigurationErrorsException (string.Format ("Required application setting '{0}' does not exist in the configuration.", name));

      return appSetting;
    }
  }
}