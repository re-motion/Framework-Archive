using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Configuration;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
public class MappingLoader : BaseLoader
{
  // types

  // static members and constants
 
  public const string ConfigurationAppSettingKey = "Rubicon.Data.DomainObjects.Mapping.ConfigurationFile";
  public const string SchemaAppSettingKey = "Rubicon.Data.DomainObjects.Mapping.SchemaFile";

  private const string c_defaultConfigurationFile = "mapping.xml";
  private const string c_defaultSchemaFile = "mapping.xsd";
  
  public static MappingLoader Create ()
  {
    return new MappingLoader (
        LoaderUtility.GetXmlFileName (ConfigurationAppSettingKey, c_defaultConfigurationFile),
        LoaderUtility.GetXmlFileName (SchemaAppSettingKey, c_defaultSchemaFile));
  }

  // member fields

  // construction and disposing

  public MappingLoader (string configurationFile, string schemaFile)
  {
    try
    {
      base.Initialize (
          configurationFile, 
          schemaFile, 
          new PrefixNamespace[] {PrefixNamespace.MappingNamespace}, 
          PrefixNamespace.MappingNamespace);
    }
    catch (XmlSchemaException e)
    {
      throw CreateMappingException (e, "Error while reading mapping: {0}", e.Message);
    }
    catch (XmlException e)
    {
      throw CreateMappingException (e, "Error while reading mapping: {0}", e.Message);
    }
  }

  // methods and properties

  public ClassDefinitionCollection GetClassDefinitions ()
  {
    ClassDefinitionLoader classDefinitionLoader = new ClassDefinitionLoader (Document, NamespaceManager);        
    return classDefinitionLoader.GetClassDefinitions ();
  }

  public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
  {
    ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

    RelationDefinitionLoader relationDefinitionLoader = new RelationDefinitionLoader (
        Document, NamespaceManager, classDefinitions);
        
    return relationDefinitionLoader.GetRelationDefinitions ();
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return CreateMappingException (null, message, args);
  }

  private MappingException CreateMappingException (Exception inner, string message, params object[] args)
  {
    return new MappingException (string.Format (message, args), inner);
  }
}
}
