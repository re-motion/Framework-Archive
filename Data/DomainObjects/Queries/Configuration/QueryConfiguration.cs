using System;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
/// <summary>
/// Represents the current query configuration.
/// </summary>
public class QueryConfiguration
{
  // types

  // static members and constants

  private static QueryConfiguration s_queryConfiguration;

  /// <summary>
  /// Gets the current query configuration.
  /// </summary>
  /// <remarks>If there is no current query configuration a new one is created.</remarks>
  public static QueryConfiguration Current
  {
    get 
    {
      lock (typeof (QueryConfiguration))
      {
        if (s_queryConfiguration == null)
        {
          s_queryConfiguration = new QueryConfiguration (QueryConfigurationLoader.Create ());
        }
        
        return s_queryConfiguration;
      }
    }
  }

  /// <summary>
  /// Sets the current query configuration.
  /// </summary>
  /// <param name="queryConfiguration">The <b>QueryConfiguration</b> to which the current <b>QueryConfiguration</b> is set.</param>
  public static void SetCurrent (QueryConfiguration queryConfiguration)
  {
    lock (typeof (QueryConfiguration))
    {
      s_queryConfiguration = queryConfiguration;
    }
  }

  // member fields

  private string _applicationName;
  private QueryDefinitionCollection _queryDefinitions;
  private string _configurationFile;
  private string _schemaFile;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>QueryConfiguration</b> class from an XML configuration file and an XML schema file.
  /// </summary>
  /// <param name="configurationFile">Configuration information is read from this file.</param>
  /// <param name="schemaFile">The <i>schemaFile</i> is used to verify the correctness of the specified <i>configurationFile</i>.</param>
  /// <exception cref="QueryConfigurationException">The query configuration could not be read from the specified <i>configurationFile</i>.</exception>
  public QueryConfiguration (string configurationFile, string schemaFile) 
      : this (new QueryConfigurationLoader (configurationFile, schemaFile))
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>QueryConfiguration</b> class from the specified <see cref="Rubicon.Data.DomainObjects.ConfigurationLoader.QueryConfigurationLoader"/>.
  /// </summary>
  /// <param name="loader">The <see cref="Rubicon.Data.DomainObjects.ConfigurationLoader.QueryConfigurationLoader"/> to be used for reading the <b>QueryConfiguration</b>.</param>
  /// <remarks>
  /// The file containing the query configuration is determined as follows:
  /// <list type="bullet">
  ///   <item>
  ///     <description>
  ///       If the application configuration file (e.g. web.config, app.config) contains the keys 
  ///       <b>Rubicon.Data.DomainObjects.Queries.Configuration.ConfigurationFile</b> and
  ///       <b>Rubicon.Data.DomainObjects.Queries.Configuration.SchemaFile</b> specifying the configuration file and the schema file for verficiation those are used.
  ///     </description>  
  ///   </item>
  ///   <item>
  ///     <description>The files <b>queries.xml</b> and <b>queries.xsd</b> must be present in the same directory as the assemblies reside.</description>
  ///   </item>  
  /// </list>
  /// </remarks>
  /// <exception cref="System.ArgumentNullException"><i>loader</i> is a null reference.</exception>
  /// <exception cref="QueryConfigurationException">The query configuration could not be read from the configuration file.</exception>
  public QueryConfiguration (QueryConfigurationLoader loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

    _applicationName = loader.GetApplicationName ();
    _queryDefinitions = loader.GetQueryDefinitions ();
    _configurationFile = loader.ConfigurationFile;
    _schemaFile = loader.SchemaFile;
  }

  // methods and properties

  /// <summary>
  /// Gets the <see cref="QueryDefinition"/> through its unique ID.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>queryID</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>queryID</i> is an empty string.</exception>
  public QueryDefinition this [string queryID]
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("queryID", queryID);
      return _queryDefinitions[queryID]; 
    }
  }

  /// <summary>
  /// Gets the application name that is specified in the XML configuration file. 
  /// </summary>
  public string ApplicationName 
  {
    get { return _applicationName; }
  }

  /// <summary>
  /// Gets all configured <see cref="QueryDefinition"/>s.
  /// </summary>
  public QueryDefinitionCollection QueryDefinitions
  {
    get { return _queryDefinitions; }
  }

  /// <summary>
  /// Gets the configuration file.
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
}
}
