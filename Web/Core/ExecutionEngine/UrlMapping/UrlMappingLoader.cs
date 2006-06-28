using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

using Rubicon.Utilities;
using Rubicon.Xml;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.ExecutionEngine.UrlMapping
{
public class UrlMappingLoader
{
  // types

  // static members and constants

  // member fields

  private string _configurationFile;
  private Type _type;
  private XmlSchemaSet _schemas;

  // construction and disposing

  public UrlMappingLoader (string configurationFile, Type type)
  {
    Initialize (configurationFile, type, new UrlMappingSchema());
  }

  // methods and properties

  public UrlMappingConfiguration CreateUrlMappingConfiguration()
  {
    return (UrlMappingConfiguration) LoadConfiguration (_configurationFile, _type, _schemas);
  }

  protected void Initialize (string configurationFile, Type type, params SchemaLoaderBase[] schemas)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("configurationFile", configurationFile);
    ArgumentUtility.CheckNotNull ("type", type);

    if (!File.Exists (configurationFile)) throw new FileNotFoundException (string.Format ("Configuration file '{0}' could not be found.", configurationFile) , configurationFile);

    _configurationFile = Path.GetFullPath (configurationFile);
    _type = type;
    _schemas = GetSchemas (schemas);
  }

  protected virtual object LoadConfiguration (string configurationFile, Type type, XmlSchemaSet schemas)
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

  protected virtual XmlSchemaSet GetSchemas (SchemaLoaderBase[] schemas)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("schemas", schemas);

    XmlSchemaSet schemaSet = new XmlSchemaSet();
    foreach (SchemaLoaderBase schema in schemas)
      schemaSet.Add (schema.LoadSchemaSet());
    return schemaSet;
  }

  public string ConfigurationFile
  {
    get { return _configurationFile; }
  }

  public Type Type
  {
    get { return _type; }
  }

  public XmlSchemaSet Schemas
  {
    get { return _schemas; }
  }
}

}
