using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Rubicon.Utilities;
using log4net;

namespace Rubicon.Xml
{

public class XmlSerializationUtility
{
  private static readonly ILog s_log = LogManager.GetLogger (typeof (XmlSerializationUtility));

  public static object DeserializeUsingSchema (
      XmlTextReader reader, string context, Type type, string defaultNamespace, XmlSchemaCollection schemas)
  {
    ArgumentUtility.CheckNotNull ("reader", reader);
    ArgumentUtility.CheckNotNullOrEmpty ("context", context);
    ArgumentUtility.CheckNotNull ("type", type);
    ArgumentUtility.CheckNotNull ("schemas", schemas);

    XmlValidatingReader validatingReader = null;
    try
    {
      XmlSerializer serializer = new XmlSerializer (type, defaultNamespace);
      validatingReader = new XmlValidatingReader (reader);
      validatingReader.Schemas.Add (schemas);
      XmlSchemaValidationHandler validationHandler = new XmlSchemaValidationHandler (context);
      validatingReader.ValidationEventHandler += validationHandler.Handler;
      object result = serializer.Deserialize (validatingReader);
      validationHandler.EnsureNoErrors();
      return result;
    }
    catch (XmlSchemaException e)
    {
      s_log.Error (string.Format ("Error reading \"{0}\": {1}", context, e.Message));
      throw;
    }
    catch (Exception e)
    {
      while (e.InnerException != null)
        e = e.InnerException;
     
      string errorMessage = null;
      if (validatingReader != null)
      {
        IXmlLineInfo lineInfo = (IXmlLineInfo) validatingReader;
        errorMessage = string.Format ("Error reading '{0}', ({1}, {2}). "
            + "The value of {3} '{4}' could not be parsed: {5}", 
            context, 
            lineInfo.LineNumber, lineInfo.LinePosition, 
            validatingReader.NodeType.ToString().ToLower(), validatingReader.Name,
            e.Message);
      }
      else
      {
        errorMessage = string.Format ("Error reading \"{0}\": {1}", context, e.Message);
      }
      s_log.Error (errorMessage, e);
      throw new Exception (errorMessage);
    }
  }

  public static object DeserializeUsingSchema (
      XmlTextReader reader, string context, Type type, XmlSchemaCollection schemas)
  {
    return XmlSerializationUtility.DeserializeUsingSchema (
        reader, context, type, XmlSerializationUtility.GetNamespace (type), schemas);
  }

  public static object DeserializeUsingSchema (
      XmlTextReader reader, string context, Type type, string schemaUri, XmlReader schemaReader)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("schemaUri", schemaUri);
    ArgumentUtility.CheckNotNull ("schemaReader", schemaReader);

    XmlSchemaCollection schemas = new XmlSchemaCollection();
    schemas.Add (schemaUri, schemaReader);
    return XmlSerializationUtility.DeserializeUsingSchema (
        reader, context, type, XmlSerializationUtility.GetNamespace (type), schemas);
  }
  
  public static object DeserializeUsingSchema (
      XmlReader reader, string context, Type type, string defaultNamespace, XmlSchemaCollection schemas)
  { 
    ArgumentUtility.CheckNotNull ("reader", reader);

    XmlDocument document = new XmlDocument ();
    document.Load (reader);

    MemoryStream memoryStream = new MemoryStream ();
    document.Save (new StreamWriter (memoryStream, Encoding.Unicode));
    memoryStream.Seek (0, SeekOrigin.Begin);

    XmlTextReader textReader = new XmlTextReader (context, memoryStream, new NameTable());
    return XmlSerializationUtility.DeserializeUsingSchema (textReader, context, type, defaultNamespace, schemas);
  }

  public static object DeserializeUsingSchema (
      XmlReader reader, string context, Type type, XmlSchemaCollection schemas)
  { 
    return XmlSerializationUtility.DeserializeUsingSchema (
        reader, context, type, XmlSerializationUtility.GetNamespace (type), schemas);
  }

  public static object DeserializeUsingSchema (
      XmlReader reader, string context, Type type, string schemaUri, XmlReader schemaReader)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("schemaUri", schemaUri);
    ArgumentUtility.CheckNotNull ("schemaReader", schemaReader);

    XmlSchemaCollection schemas = new XmlSchemaCollection();
    schemas.Add (schemaUri, schemaReader);
    return XmlSerializationUtility.DeserializeUsingSchema (
        reader, context, type, XmlSerializationUtility.GetNamespace (type), schemas);
  }

  public static string GetNamespace (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    XmlTypeAttribute[] xmlTypes = (XmlTypeAttribute[]) type.GetCustomAttributes (typeof (XmlTypeAttribute), true);
    XmlRootAttribute[] xmlRoots = (XmlRootAttribute[]) type.GetCustomAttributes (typeof (XmlRootAttribute), true);
    
    bool hasXmlType = xmlTypes.Length == 1;
    bool hasXmlRoot = xmlRoots.Length == 1;
    if (! hasXmlType && ! hasXmlRoot) 
    {
      throw new ArgumentException (string.Format (
              "Cannot determine the xml namespace of type '{0}' because no neither an XmlTypeAttribute nor an "
              + "XmlRootAttribute has been provided.", type),
          "type");
    }

    XmlTypeAttribute xmlType = hasXmlType ? xmlTypes[0] : null;
    XmlRootAttribute xmlRoot = hasXmlRoot ? xmlRoots[0] : null;

    bool hasXmlTypeNamespace = hasXmlType ? (! StringUtility.IsNullOrEmpty (xmlType.Namespace)) : false;
    bool hasXmlRootNamespace = hasXmlRoot ? (! StringUtility.IsNullOrEmpty (xmlRoot.Namespace)) : false;
    if (! hasXmlTypeNamespace && ! hasXmlRootNamespace)
    {
      throw new ArgumentException (string.Format (
              "Cannot determine the xml namespace of type '{0}' because neither an XmlTypeAttribute nor an "
              + "XmlRootAttribute is used to define a namespace for the type.", type),
          "type");
    }

    if (hasXmlRootNamespace)
      return xmlRoot.Namespace;
    else
      return xmlType.Namespace;
  }

  /// <exclude />
  private XmlSerializationUtility()
  {
  }
}

}
