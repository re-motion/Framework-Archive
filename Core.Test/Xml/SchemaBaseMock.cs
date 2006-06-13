using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Xml;
using System.Xml.Schema;
using Rubicon.Utilities;
using System.Xml;

namespace Rubicon.Core.UnitTests.Xml
{
  public class SchemaBaseMock : SchemaBase
  {
    // types

    // static members and constants

    // member fields

    private string _schemaUri;

    // construction and disposing

    public SchemaBaseMock (string schemaUri)
    {
      ArgumentUtility.CheckNotNull ("schemaUri", schemaUri);

      _schemaUri = schemaUri;
    }

    // methods and properties

    protected override string SchemaFile
    {
      get { return "SchemaBaseMock.xsd"; }
    }

    public override string SchemaUri
    {
      get { return _schemaUri; }
    }

    public new XmlSchema GetSchema (string schemaFileName)
    {
      return base.GetSchema (schemaFileName);
    }
  }
}
