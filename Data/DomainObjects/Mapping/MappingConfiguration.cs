using System;

using Rubicon.Data.DomainObjects.Configuration.Loader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Configuration.Mapping
{
public class MappingConfiguration
{
  // types

  // static members and constants

  private static MappingConfiguration s_mappingConfiguration;

  public static MappingConfiguration Current
  {
    get 
    {
      lock (typeof (MappingConfiguration))
      {
        if (s_mappingConfiguration == null)
        {
          s_mappingConfiguration = new MappingConfiguration (MappingLoader.Create ());
        }
        
        return s_mappingConfiguration;
      }
    }
  }

  public static void SetCurrent (MappingConfiguration mappingConfiguration)
  {
    lock (typeof (MappingConfiguration))
    {
      s_mappingConfiguration = mappingConfiguration;
    }
  }

  // member fields

  private ClassDefinitionCollection _classDefinitions;
  private RelationDefinitionCollection _relationDefinitions;
  private string _configurationFile;
  private string _schemaFile;

  // construction and disposing

  public MappingConfiguration (string configurationFile, string schemaFile) 
      : this (new MappingLoader (configurationFile, schemaFile))
  {
  }

  public MappingConfiguration (MappingLoader loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

    _classDefinitions = loader.GetClassDefinitions ();
    _relationDefinitions = loader.GetRelationDefinitions (_classDefinitions);
    _configurationFile = loader.ConfigurationFile;
    _schemaFile = loader.SchemaFile;
  }

  // methods and properties

  public ClassDefinitionCollection ClassDefinitions
  {
    get { return _classDefinitions; }
  }

  public RelationDefinitionCollection RelationDefinitions
  {
    get { return _relationDefinitions; }
  }

  public string ConfigurationFile
  {
    get { return _configurationFile; }
  }

  public string SchemaFile
  {
    get { return _schemaFile; }
  }
}
}