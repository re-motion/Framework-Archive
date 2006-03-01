using System;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
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
  private string _schemaFile;
  private bool _resolveTypeNames;

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
    _schemaFile = loader.SchemaFile;
    _resolveTypeNames = loader.ResolveTypeNames;
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
  /// Gets the schema file that the <see cref="ConfigurationFile"/> has been validated against.
  /// </summary>
  public string SchemaFile
  {
    get { return _schemaFile; }
  }

  // TODO: Doc
  public bool ResolveTypeNames
  {
    get { return _resolveTypeNames; }
  }
}
}
