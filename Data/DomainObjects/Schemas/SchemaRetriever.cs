using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Xml;
using Rubicon.Utilities;
using System.Xml.Schema;
using Rubicon.Data.DomainObjects.ConfigurationLoader;

namespace Rubicon.Data.DomainObjects.Schemas
{
  public class SchemaRetriever : SchemaBase
  {
    // types

    public enum SchemaType
    {
      Mapping,
      Queries,
      StorageProviders
    }

    // static members and constants

    // member fields

    private SchemaType _type;
    private string _schemaFile;
    private string _schemaUri;

    // construction and disposing

    public SchemaRetriever (SchemaType type)
    {
      ArgumentUtility.CheckValidEnumValue (type, "type");

      _type = type;
      switch (type)
      {
        case SchemaType.Mapping:
          _schemaFile = "mapping.xsd";
          _schemaUri =  PrefixNamespace.MappingNamespace.Uri;
          break;
        case SchemaType.Queries:
          _schemaFile = "queries.xsd";
          _schemaUri = PrefixNamespace.QueryConfigurationNamespace.Uri;
          break;
        case SchemaType.StorageProviders:
          _schemaFile = "storageProviders.xsd";
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

    public override XmlSchemaSet GetSchemaSet ()
    {
      XmlSchemaSet schemaSet = new XmlSchemaSet ();
      schemaSet.Add (GetSchema ("types.xsd"));
      schemaSet.Add (GetSchema (_schemaFile));
      return schemaSet;
    }

    protected override string SchemaFile
    {
      get { return _schemaFile; }
    }
  }
}
