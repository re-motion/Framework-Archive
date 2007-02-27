using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Represents the common information all configuration classes provide.
/// </summary>
public class ConfigurationBase
{
  // types

  // static members and constants

  // member fields

  private string _applicationName;
  private string _configurationFile;
  private bool _resolveTypes;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>ConfigurationBase</b> class from the specified <see cref="Rubicon.Data.DomainObjects.ConfigurationLoader.BaseFileLoader"/>.
  /// </summary>
  /// <param name="loader">The <see cref="Rubicon.Data.DomainObjects.ConfigurationLoader.BaseFileLoader"/> to be used for reading the configuration. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="loader"/> is <see langword="null"/>.</exception>
  protected ConfigurationBase (BaseFileLoader loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

    _applicationName = loader.GetApplicationName ();
    _configurationFile = loader.ConfigurationFile;
    _resolveTypes = loader.ResolveTypes;
  }

  // methods and properties

  /// <summary>
  /// Gets the application name that is specified in the XML configuration file. 
  /// </summary>
  public string ApplicationName
  {
    get { return _applicationName; }
  }

  /// <summary>
  /// Gets the XML configuration file.
  /// </summary>
  public string ConfigurationFile
  {
    get { return _configurationFile; }
  }

  /// <summary>
  /// Gets a flag whether type names in the configuration file should be resolved to their corresponding .NET <see cref="Type"/>.
  /// </summary>
  public bool ResolveTypes
  {
    get { return _resolveTypes; }
  }
}
}
