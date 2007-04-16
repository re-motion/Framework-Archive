using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Schemas;

namespace Rubicon.Data.DomainObjects.Legacy.Schemas
{
  public class LegacySchemaLoader: SchemaLoader
  {
    // types

    // static members and constants
    public static readonly SchemaLoader Mapping = new LegacySchemaLoader ("Mapping.xsd", LegacyPrefixNamespace.MappingNamespace.Uri);

    // member fields

    // construction and disposing


    protected LegacySchemaLoader (string schemaFile, string schemaUri)
        : base (schemaFile, schemaUri)
    {
    }

    // methods and properties
  }
}