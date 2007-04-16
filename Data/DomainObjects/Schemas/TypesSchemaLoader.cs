using System;
using System.Xml.Schema;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Utilities;
using Rubicon.Xml;

namespace Rubicon.Data.DomainObjects.Schemas
{
  public sealed class TypesSchemaLoader : SchemaLoaderBase
  {
    // types

    // static members and constants

    public static readonly TypesSchemaLoader Instance = new TypesSchemaLoader();

    // member fields

    private readonly string _schemaFile = "Types.xsd";
    private readonly string _schemaUri = "http://www.rubicon-it.com/Data/DomainObjects/Types";

    // construction and disposing

    private TypesSchemaLoader ()
    {
    }

    // methods and properties

    public override string SchemaUri
    {
      get { return _schemaUri; }
    }

    protected override string SchemaFile
    {
      get { return _schemaFile; }
    }
  }
}
