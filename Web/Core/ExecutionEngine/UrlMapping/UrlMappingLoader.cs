using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

using Rubicon.Utilities;
using Rubicon.Xml;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.ExecutionEngine.Mapping
{
public class MappingLoader
{
  // types

  // static members and constants

  // member fields

  private string _configurationFile;
  private Type _type;
  private XmlSchemaCollection _schemas;

  // construction and disposing

  public MappingLoader (string configurationFile, Type type)
  {
    Initialize (configurationFile, type, new MappingSchema());
  }

  // methods and properties

  public MappingConfiguration CreateMappingConfiguration()
  {
    return (MappingConfiguration) LoadConfiguration (_configurationFile, _type, _schemas);
  }

  protected void Initialize (string configurationFile, Type type, params SchemaBase[] schemas)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("configurationFile", configurationFile);
    ArgumentUtility.CheckNotNull ("type", type);

    if (!File.Exists (configurationFile)) throw new FileNotFoundException (string.Format ("Configuration file '{0}' could not be found.", configurationFile) , configurationFile);

    _configurationFile = Path.GetFullPath (configurationFile);
    _type = type;
    _schemas = GetSchemas (schemas);
  }

  protected virtual object LoadConfiguration (string configurationFile, Type type, XmlSchemaCollection schemas)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("configurationFile", configurationFile);
    ArgumentUtility.CheckNotNull ("type", type);
    ArgumentUtility.CheckNotNull ("schemas", schemas);

    XmlTextReader reader = new XmlTextReader (_configurationFile);
    object configuration = null;
    try
    {
      configuration = XmlSerializationUtility.DeserializeUsingSchema (reader, _configurationFile, _type, _schemas);
    }
    finally
    {
      reader.Close();
    }
    return configuration;
//    try
//    {
//    return XmlSerializationUtility.DeserializeUsingSchema (reader, _configurationFile, _type, _schemas);
//    }
//    catch (XmlSchemaException e)
//    {
//      throw CreateMappingException (e, "Error while reading mapping: {0}", e.Message);
//    }
//    catch (XmlException e)
//    {
//      throw CreateMappingException (e, "Error while reading mapping: {0}", e.Message);
//    }
  }

  protected virtual XmlSchemaCollection GetSchemas (SchemaBase[] schemas)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("schemas", schemas);

    XmlSchemaCollection schemaCollection = new XmlSchemaCollection();
    foreach (SchemaBase schema in schemas)
      schemaCollection.Add (schema.SchemaUri, schema.GetSchemaReader());
    return schemaCollection;
  }

  public string ConfigurationFile
  {
    get { return _configurationFile; }
  }

  public Type Type
  {
    get { return _type; }
  }

  public XmlSchemaCollection Schemas
  {
    get { return _schemas; }
  }
}

}
