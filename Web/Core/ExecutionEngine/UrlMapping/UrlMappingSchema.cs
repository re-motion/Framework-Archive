using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Reflection;
using Rubicon.Utilities;
using Rubicon.Xml;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.ExecutionEngine.UrlMapping
{

public class UrlMappingSchema: SchemaBase
{
  public UrlMappingSchema()
  {
  }

  protected override string SchemaFile
  {
    get { return "UrlMapping.xsd"; }
  }

  public override string SchemaUri
  {
    get { return UrlMappingConfiguration.SchemaUri; }
  }
}

}