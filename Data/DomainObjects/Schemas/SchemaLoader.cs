using System;
using System.Xml.Schema;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Utilities;
using Rubicon.Xml;

namespace Rubicon.Data.DomainObjects.Schemas
{
  public class SchemaLoader : SchemaLoaderBase
  {
    // types

    // static members and constants

    public readonly static SchemaLoader Queries = new SchemaLoader ("Queries.xsd", PrefixNamespace.QueryConfigurationNamespace.Uri);

    // member fields

    private readonly string _schemaFile;
    private readonly string _schemaUri;

    // construction and disposing

    protected SchemaLoader (string schemaFile, string schemaUri)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("schemaFile", schemaFile);
      ArgumentUtility.CheckNotNullOrEmpty ("schemaUri", schemaUri);

      _schemaFile = schemaFile;
      _schemaUri = schemaUri;
    }

    // methods and properties

    public override string SchemaUri
    {
      get { return _schemaUri; }
    }

    public override XmlSchemaSet LoadSchemaSet ()
    {
      XmlSchemaSet schemaSet = base.LoadSchemaSet ();
      schemaSet.Add (TypesSchemaLoader.Instance.LoadSchemaSet());

      return schemaSet;
    }

    protected override string SchemaFile
    {
      get { return _schemaFile; }
    }
  }
}
