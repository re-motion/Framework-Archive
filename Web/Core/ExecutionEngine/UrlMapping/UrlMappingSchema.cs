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

public abstract class SchemaBase
{
  protected abstract string SchemaFile { get; }
  public abstract string SchemaUri { get; }

  /// <summary> Gets an <see cref="XmlReader"/> reader for the schema embedded in the assembly. </summary>
  /// <exception cref="ApplicationException"> Thrown if the schema file could not be loaded. </exception>
  public XmlReader GetSchemaReader ()
  {
    Assembly assembly = Assembly.GetExecutingAssembly();
    Stream schemaStream = assembly.GetManifestResourceStream (GetType(), SchemaFile);
    if (schemaStream == null)
    {
      throw new ApplicationException (string.Format (
          "Error loading schema resource '{0}' from assembly '{1}'.", SchemaFile, assembly.FullName));
    }
    return new XmlTextReader (schemaStream);
  }
}

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