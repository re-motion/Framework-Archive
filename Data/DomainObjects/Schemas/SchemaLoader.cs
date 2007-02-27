using System;
using System.Xml.Schema;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Utilities;
using Rubicon.Xml;

namespace Rubicon.Data.DomainObjects.Schemas
{
  public class SchemaLoader : SchemaLoaderBase
  {
    // types

    // static members and constants

    // member fields

    private SchemaType _type;
    private string _schemaFile;
    private string _schemaUri;

    // construction and disposing

    public SchemaLoader (SchemaType type)
    {
      ArgumentUtility.CheckValidEnumValue ("type", type);

      _type = type;
      switch (type)
      {
        case SchemaType.Mapping:
          _schemaFile = "Mapping.xsd";
          _schemaUri =  PrefixNamespace.MappingNamespace.Uri;
          break;
        case SchemaType.Queries:
          _schemaFile = "Queries.xsd";
          _schemaUri = PrefixNamespace.QueryConfigurationNamespace.Uri;
          break;
        case SchemaType.StorageProviders:
          _schemaFile = "StorageProviders.xsd";
          _schemaUri = PrefixNamespace.StorageProviderConfigurationNamespace.Uri;
          break;
      }
    }

    // methods and properties

    public SchemaType Type
    {
      get { return _type; }
    }

    public override string SchemaUri
    {
      get { return _schemaUri; }
    }

    public override XmlSchemaSet LoadSchemaSet ()
    {
      XmlSchemaSet schemaSet = base.LoadSchemaSet ();
      schemaSet.Add (LoadSchema ("Types.xsd"));

      return schemaSet;
    }

    protected override string SchemaFile
    {
      get { return _schemaFile; }
    }
  }
}
