using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Schemas;

namespace Rubicon.Data.DomainObjects.Legacy.Schemas
{
  public class LegacySchemaLoader: SchemaLoader
  {
    // types

    // static members and constants
    public static readonly SchemaLoader Mapping = new LegacySchemaLoader ("Mapping.xsd", PrefixNamespace.MappingNamespace.Uri);

    public static readonly SchemaLoader StorageProviders =
        new LegacySchemaLoader ("StorageProviders.xsd", PrefixNamespace.StorageProviderConfigurationNamespace.Uri);

    // member fields

    // construction and disposing


    protected LegacySchemaLoader (string schemaFile, string schemaUri)
        : base (schemaFile, schemaUri)
    {
    }

    // methods and properties
  }
}