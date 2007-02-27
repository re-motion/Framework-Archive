using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Schemas;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader
{
  public class MappingLoader : BaseFileLoader, IMappingLoader
  {
    // types

    // static members and constants

    [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
    public const string ConfigurationAppSettingKey = MappingConfiguration.ConfigurationAppSettingKey;
 
    [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
    public const string DefaultConfigurationFile = MappingConfiguration.DefaultConfigurationFile;

    [Obsolete ("Use MappingConfiguration.CreateConfigurationFromFileBasedLoader (). (Version 1.7.42", true)]
    public static MappingLoader Create ()
    {
      return new MappingLoader (LoaderUtility.GetConfigurationFileName (ConfigurationAppSettingKey, DefaultConfigurationFile), true);
    }

    // member fields

    // construction and disposing

    public MappingLoader (string configurationFile, bool resolveTypes)
    {
      try
      {
        base.Initialize (
            configurationFile,
            SchemaType.Mapping,
            resolveTypes,
            PrefixNamespace.MappingNamespace);
      }
      catch (ConfigurationException e)
      {
        throw CreateMappingException (e, "Error while reading mapping: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
      catch (XmlSchemaException e)
      {
        throw CreateMappingException (e, "Error while reading mapping: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
      catch (XmlException e)
      {
        throw CreateMappingException (e, "Error while reading mapping: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
    }

    // methods and properties

    public ClassDefinitionCollection GetClassDefinitions ()
    {
      ClassDefinitionLoader classDefinitionLoader = new ClassDefinitionLoader (Document, NamespaceManager, ResolveTypes);        

      ClassDefinitionCollection classDefinitions = classDefinitionLoader.GetClassDefinitions ();
      classDefinitions.Validate ();
      return classDefinitions;
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
