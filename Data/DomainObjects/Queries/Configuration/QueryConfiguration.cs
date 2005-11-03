using System;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
/// <summary>
/// Represents the current query configuration.
/// </summary>
public class QueryConfiguration : ConfigurationBase
{
  // types

  // static members and constants

  private static QueryConfiguration s_queryConfiguration;

  /// <summary>
  /// Gets the current query configuration.
  /// </summary>
  /// <remarks>
  /// <para>If there is no current query configuration a new one is created.</para>
  /// <para>The file containing the query configuration is determined as follows:
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
  /// </para>
  /// </remarks>
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

  private QueryDefinitionCollection _queryDefinitions;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>QueryConfiguration</b> class from an XML configuration file and an XML schema file.
  /// </summary>
  /// <param name="configurationFile">Configuration information is read from this file.</param>
  /// <param name="schemaFile">The <paramref name="schemaFile"/> is used to verify the correctness of the specified <paramref name="configurationFile"/>.</param>
  /// <exception cref="QueryConfigurationException">The query configuration could not be read from the specified <paramref name="configurationFile"/>.</exception>
  public QueryConfiguration (string configurationFile, string schemaFile) 
      : this (new QueryConfigurationLoader (configurationFile, schemaFile))
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>QueryConfiguration</b> class from the specified <see cref="Rubicon.Data.DomainObjects.ConfigurationLoader.QueryConfigurationLoader"/>.
  /// </summary>
  /// <param name="loader">The <see cref="Rubicon.Data.DomainObjects.ConfigurationLoader.QueryConfigurationLoader"/> to be used for reading the <b>QueryConfiguration</b>. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="loader"/> is <see langword="null"/>.</exception>
  /// <exception cref="QueryConfigurationException">The query configuration could not be read from the configuration file.</exception>
  public QueryConfiguration (QueryConfigurationLoader loader) : base (loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

    _queryDefinitions = loader.GetQueryDefinitions ();
  }

  // methods and properties

  /// <summary>
  /// Gets the <see cref="QueryDefinition"/> through its unique ID.
  /// </summary>
  /// <param name="queryID">The name of the query. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="queryID"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="queryID"/> is an empty string.</exception>
  public QueryDefinition this [string queryID]
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("queryID", queryID);
      return _queryDefinitions[queryID]; 
    }
  }

  /// <summary>
  /// Gets the <see cref="QueryDefinition"/> through its index.
  /// </summary>
  /// <param name="index">The index of the requested object.</param>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
  ///   <paramref name="index"/> is equal to or greater than the number of items in the configuration.
  /// </exception>
  public QueryDefinition this [int index]
  {
    get { return _queryDefinitions[index]; }
  }

  /// <summary>
  /// Gets all configured <see cref="QueryDefinition"/>s.
  /// </summary>
  public QueryDefinitionCollection QueryDefinitions
  {
    get { return _queryDefinitions; }
  }

  /// <summary>
  /// Determines whether the <see cref="QueryConfiguration"/> contains a specific <see cref="QueryDefinition"/>.
  /// </summary>
  /// <param name="queryDefinition">The object to locate in the <see cref="QueryConfiguration"/>. Must not be <see langword="null"/>.</param>
  /// <returns><see langword="true"/> if the <see cref="QueryConfiguration"/> contains the <paramref name="queryDefinition"/>; otherwise <see langword="false"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="queryDefinition"/> is <see langword="null"/>.</exception>
  /// <remarks>This method only returns true, if the same reference is found in the collection.</remarks>
  public bool Contains (QueryDefinition queryDefinition)
  {
    ArgumentUtility.CheckNotNull ("queryDefinition", queryDefinition);

    return _queryDefinitions.Contains (queryDefinition);
  }
}
}
