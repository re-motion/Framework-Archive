using System;
using System.Configuration;
using System.IO;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Utilities;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Represents the current query configuration.
  /// </summary>
  public class QueryConfiguration : ExtendedConfigurationSection
  {
    // public const string DefaultConfigurationFile = "queries.xml";
    public static readonly string DefaultConfigurationFile = QueryFileElement.GetRootedPath ("queries.xml");

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty _queryFilesProperty;

    private readonly DoubleCheckedLockingContainer<QueryDefinitionCollection> _queries;

    public QueryConfiguration ()
    {
      _queries = new DoubleCheckedLockingContainer<QueryDefinitionCollection> (delegate { return LoadAllQueryDefinitions (); });

      _queryFilesProperty = new ConfigurationProperty (
          "queryFiles",
          typeof (ConfigurationElementCollection<QueryFileElement>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_queryFilesProperty);
    }

    public QueryConfiguration (params string[] configurationFiles) : this()
    {
      ArgumentUtility.CheckNotNull ("configurationFiles", configurationFiles);

      for (int i = 0; i < configurationFiles.Length; i++)
      {
        string configurationFile = configurationFiles[i];
        QueryFileElement element = new QueryFileElement (configurationFile, configurationFile);
        QueryFiles.Add (element);
      }
    }

    private QueryDefinitionCollection LoadAllQueryDefinitions ()
    {
      if (QueryFiles.Count == 0)
        return new QueryConfigurationLoader (DefaultConfigurationFile).GetQueryDefinitions ();
      else
      {
        QueryDefinitionCollection result = new QueryDefinitionCollection ();

        for (int i = 0; i < QueryFiles.Count; i++)
        {
          QueryConfigurationLoader loader = new QueryConfigurationLoader (QueryFiles[i].RootedFileName);
          result.Merge (loader.GetQueryDefinitions ());
        }
        return result;
      }
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