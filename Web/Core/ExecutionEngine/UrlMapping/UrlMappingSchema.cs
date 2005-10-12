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

namespace Rubicon.Web.ExecutionEngine.Mapping
{

public abstract class SchemaBase
{
  protected abstract string Schema { get; }
  public abstract string SchemaUri { get; }

  /// <summary> Gets an <see cref="XmlReader"/> reader for the schema embedded in the assembly. </summary>
  /// <exception cref="ApplicationException"> Thrown if the schema file could not be loaded. </exception>
  public XmlReader GetSchemaReader ()
  {
    Assembly assembly = Assembly.GetExecutingAssembly();
    Stream schema = assembly.GetManifestResourceStream (GetType(), Schema);
    if (schema == null)
    {
      throw new ApplicationException (string.Format (
          "Error loading schema resource '{0}' from assembly '{1}'.", Schema, assembly.FullName));
    }
    return new XmlTextReader (schema);
  }
}

public class MappingSchema: SchemaBase
{
  public MappingSchema()
  {
  }

  protected override string Schema
  {
    get { return "WxeMapping.xsd"; }
  }

  public override string SchemaUri
  {
    get { return MappingConfiguration.SchemaUri; }
  }
}

}