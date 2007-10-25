using System;
using System.Configuration;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Represents the current query configuration.
  /// </summary>
  public class QueryConfiguration : ExtendedConfigurationSection
  {
    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty _queryFilesProperty;

    private readonly DoubleCheckedLockingContainer<QueryConfigurationLoader> _loader;
    private readonly DoubleCheckedLockingContainer<QueryDefinitionCollection> _queries;

    public QueryConfiguration ()
    {
      _queries = new DoubleCheckedLockingContainer<QueryDefinitionCollection> (delegate { return _loader.Value.GetQueryDefinitions (); });

      _queryFilesProperty = new ConfigurationProperty (
          "queryFiles",
          typeof (ConfigurationElementCollection<QueryFileElement>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_queryFilesProperty);

      // TODO: support multiple files in this delegate
      _loader = new DoubleCheckedLockingContainer<QueryConfigurationLoader> (delegate
      {
        if (QueryFiles.Count > 0)
          return new QueryConfigurationLoader (QueryFiles[0].FileName);
        else
          return QueryConfigurationLoader.Create ();
      });
    }

    public QueryConfiguration (string configurationFile) : this()
    {
      ArgumentUtility.CheckNotNull ("configurationFile", configurationFile);

      QueryFileElement element = new QueryFileElement (configurationFile, configurationFile);
      QueryFiles.Add (element);
    }

    public ConfigurationElementCollection<QueryFileElement> QueryFiles
    {
      get { return (ConfigurationElementCollection<QueryFileElement>) this[_queryFilesProperty]; }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public QueryDefinitionCollection QueryDefinitions
    {
      get { return _queries.Value; }
    }
  }
}