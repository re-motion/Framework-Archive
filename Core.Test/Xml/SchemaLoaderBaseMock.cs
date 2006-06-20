using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Xml;
using System.Xml.Schema;
using Rubicon.Utilities;
using System.Xml;

namespace Rubicon.Core.UnitTests.Xml
{
  public class SchemaLoaderBaseMock : SchemaLoaderBase
  {
    // types

    // static members and constants

    // member fields

    private string _schemaUri;

    // construction and disposing

    public SchemaLoaderBaseMock (string schemaUri)
    {
      ArgumentUtility.CheckNotNull ("schemaUri", schemaUri);

      _schemaUri = schemaUri;
    }

    // methods and properties

    protected override string SchemaFile
    {
      get { return "SchemaLoaderBaseMock.xsd"; }
    }

    public override string SchemaUri
    {
      get { return _schemaUri; }
    }

    public new XmlSchema LoadSchema (string schemaFileName)
    {
      return base.LoadSchema (schemaFileName);
    }
  }
}
