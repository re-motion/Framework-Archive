using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Design;
using Remotion.Data.DomainObjects.Legacy.Design;
using Remotion.Data.DomainObjects.Legacy.Schemas;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader
{
  [DesignModeMappingLoader (typeof (DesignModeXmlBasedMappingLoader))]
  public class MappingLoader: BaseFileLoader, IMappingLoader
  {
    // types

    // static members and constants

    public const string ConfigurationAppSettingKey = "Remotion.Data.DomainObjects.Mapping.ConfigurationFile";
    public const string DefaultConfigurationFile = "Mapping.xml";

    public static MappingLoader Create()
    {
      return new MappingLoader (LoaderUtility.GetConfigurationFileName (ConfigurationAppSettingKey, DefaultConfigurationFile), true);
    }

    // member fields

    // construction and disposing

    public MappingLoader()
        : this (LoaderUtility.GetConfigurationFileName (ConfigurationAppSettingKey, DefaultConfigurationFile), true)
    {
    }

    public MappingLoader (string configurationFile, bool resolveTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("configurationFile", configurationFile);

      try
      {
        base.Initialize (
            configurationFile,
            LegacySchemaLoader.Mapping,
            resolveTypes,
            LegacyPrefixNamespace.MappingNamespace);
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

    public ClassDefinitionCollection GetClassDefinitions()
    {
      ClassDefinitionLoader classDefinitionLoader = new ClassDefinitionLoader (Document, NamespaceManager, ResolveTypes);

      ClassDefinitionCollection classDefinitions = classDefinitionLoader.GetClassDefinitions();
      classDefinitions.Validate();
      return classDefinitions;
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      RelationDefinitionLoader relationDefinitionLoader = new RelationDefinitionLoader (Document, NamespaceManager, classDefinitions);

      return relationDefinitionLoader.GetRelationDefinitions();
    }

    private MappingException CreateMappingException (Exception inner, string message, params object[] args)
    {
      return new MappingException (string.Format (message, args), inner);
    }
  }
}