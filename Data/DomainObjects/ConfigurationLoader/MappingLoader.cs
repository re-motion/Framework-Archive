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
public class MappingLoader : BaseFileLoader
{
  // types

  // static members and constants
 
  public const string ConfigurationAppSettingKey = "Rubicon.Data.DomainObjects.Mapping.ConfigurationFile";
  public const string SchemaAppSettingKey = "Rubicon.Data.DomainObjects.Mapping.SchemaFile";

  public const string DefaultConfigurationFile = "mapping.xml";
  public const string DefaultSchemaFile = "mapping.xsd";
  
  public static MappingLoader Create ()
  {
    return new MappingLoader (
        LoaderUtility.GetXmlFileName (ConfigurationAppSettingKey, DefaultConfigurationFile),
        LoaderUtility.GetXmlFileName (SchemaAppSettingKey, DefaultSchemaFile),
        true);
  }

  // member fields

  // construction and disposing

  public MappingLoader (string configurationFile, string schemaFile, bool resolveTypes)
  {
    try
    {
      base.Initialize (
          configurationFile, 
          schemaFile, 
          resolveTypes,
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
    ClassDefinitionLoader classDefinitionLoader = new ClassDefinitionLoader (Document, NamespaceManager, ResolveTypes);        
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
