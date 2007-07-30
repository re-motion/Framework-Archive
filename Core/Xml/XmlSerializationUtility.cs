using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Rubicon.Logging;
using Rubicon.Utilities;

namespace Rubicon.Xml
{
  [Serializable]
  [Obsolete ("try to get to original exception")]
  public class RubiconXmlSchemaValidationException: XmlException
  {
    private string _rawMessage;
    private string _fileName;
    //private int _lineNumber;
    //private int _linePosition;

    public RubiconXmlSchemaValidationException (string message, string fileName, int lineNumber, int linePosition, Exception innerException)
        : base (message, innerException, lineNumber, linePosition)
    {
      _rawMessage = message;
      _fileName = fileName;
      //_lineNumber = lineNumber;
      //_linePosition = linePosition;
    }

    protected RubiconXmlSchemaValidationException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
      _rawMessage = info.GetString ("_rawMessage");
      _fileName = info.GetString ("_fileName");
      //_lineNumber = info.GetInt32 ("_lineNumber");
      //_linePosition = info.GetInt32 ("_linePosition");
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("_rawMessage", _rawMessage);
      info.AddValue ("_fileName", _fileName);
      //info.AddValue ("_lineNumber", _lineNumber);
      //info.AddValue ("_linePosition", _linePosition);
    }

    public string RawMessage
    {
      get { return _rawMessage; }
    }

    public string FileName
    {
      get { return _fileName; }
    }

    //public int LineNumber
    //{
    //  get { return _lineNumber; }
    //}

    //public int LinePosition
    //{
    //  get { return _linePosition; }
    //}    
  }
  /// <summary>
  /// Use this class to easily serialize and deserialize objects to or from XML.
  /// </summary>
  public static class XmlSerializationUtility
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (XmlSerializationUtility));

    public static object DeserializeUsingSchema (XmlReader reader, Type type, string defaultNamespace, XmlReaderSettings settings)
    {
      ArgumentUtility.CheckNotNull ("reader", reader);
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("settings", settings);

      XmlSchemaValidationHandler validationHandler = new XmlSchemaValidationHandler (true);
      settings.ValidationEventHandler += validationHandler.Handler;
      settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

      XmlReader innerReader = XmlReader.Create (reader, settings);
      XmlSerializer serializer = new XmlSerializer (type, defaultNamespace);

      try
      {
        return serializer.Deserialize (innerReader);
      }
      catch (InvalidOperationException e)
      {
        // unwrap an inner XmlSchemaValidationException 
        XmlSchemaValidationException schemaException = e.InnerException as XmlSchemaValidationException;
        if (schemaException != null)
          throw schemaException;

        // wrap any other InvalidOperationException in an XmlException with line info
        IXmlLineInfo lineInfo = (IXmlLineInfo) innerReader;
        if (lineInfo != null)
        {
          string errorMessage = string.Format (
              "Error reading {0} ({1},{2}): {3}",
              innerReader.BaseURI,
              lineInfo.LineNumber,
              lineInfo.LinePosition,
              e.Message);
          throw new XmlException (errorMessage, e, lineInfo.LineNumber, lineInfo.LinePosition);
        }
        else
        {
          string errorMessage = string.Format (
              "Error reading {0}: {1}",
              innerReader.BaseURI,
              e.Message); 
          throw new XmlException (errorMessage, e);
        }
      }
    }

    public static object DeserializeUsingSchema (XmlReader reader, Type type, string defaultNamespace, XmlSchemaSet schemas)
    {
      ArgumentUtility.CheckNotNull ("reader", reader);

      XmlReaderSettings settings = new XmlReaderSettings();
      settings.Schemas = schemas;
      settings.ValidationType = ValidationType.Schema;

      return DeserializeUsingSchema (reader, type, defaultNamespace, settings);
    }

    public static object DeserializeUsingSchema (XmlReader reader, Type type, XmlSchemaSet schemas)
    {
      return DeserializeUsingSchema (reader, type, GetNamespace (type), schemas);
    }

    public static object DeserializeUsingSchema (XmlReader reader, Type type, string schemaUri, XmlReader schemaReader)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("schemaUri", schemaUri);
      ArgumentUtility.CheckNotNull ("schemaReader", schemaReader);

      XmlSchemaSet schemas = new XmlSchemaSet();
      schemas.Add (schemaUri, schemaReader);
      return DeserializeUsingSchema (reader, type, GetNamespace (type), schemas);
    }

    #region obsolete overloads with context argument
    [Obsolete ("Argument 'context' is no longer supported. Specify BaseURL for XmlReader instead.", true)]
    public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, string defaultNamespace, XmlReaderSettings settings)
    {
      throw new NotSupportedException ();
    }

    [Obsolete ("Argument 'context' is no longer supported. Specify BaseURL for XmlReader instead.", true)]
    public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, string defaultNamespace, XmlSchemaSet schemas)
    {
      throw new NotSupportedException ();
    }

    [Obsolete ("Argument 'context' is no longer supported. Specify BaseURL for XmlReader instead.", true)]
    public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, XmlSchemaSet schemas)
    {
      throw new NotSupportedException ();
    }

    [Obsolete ("Argument 'context' is no longer supported. Specify BaseURL for XmlReader instead.", true)]
    public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, string schemaUri, XmlReader schemaReader)
    {
      throw new NotSupportedException ();
    }
    #endregion
    
    /// <summary>
    /// Get the Namespace from a type's <see cref="XmlRootAttribute"/> (preferred) or <see cref="XmlTypeAttribute"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown if no namespace is specified through at least one of the possible attributes. </exception>
    public static string GetNamespace (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      XmlTypeAttribute xmlType = (XmlTypeAttribute) Attribute.GetCustomAttribute (type, typeof (XmlTypeAttribute), true);
      XmlRootAttribute xmlRoot = (XmlRootAttribute) Attribute.GetCustomAttribute (type, typeof (XmlRootAttribute), true);
      bool hasXmlType = xmlType != null;
      bool hasXmlRoot = xmlRoot != null;
      if (!hasXmlType && !hasXmlRoot)
      {
        throw new ArgumentException (
            string.Format (
                "Cannot determine the xml namespace of type '{0}' because no neither an XmlTypeAttribute nor an XmlRootAttribute has been provided.",
                type.FullName),
            "type");
      }

      bool hasXmlTypeNamespace = hasXmlType ? (! String.IsNullOrEmpty (xmlType.Namespace)) : false;
      bool hasXmlRootNamespace = hasXmlRoot ? (! String.IsNullOrEmpty (xmlRoot.Namespace)) : false;
      if (! hasXmlTypeNamespace && ! hasXmlRootNamespace)
      {
        throw new ArgumentException (
            string.Format (
                "Cannot determine the xml namespace of type '{0}' because neither an XmlTypeAttribute nor an XmlRootAttribute is used to define a namespace for the type.",
                type.FullName),
            "type");
      }

      if (hasXmlRootNamespace)
        return xmlRoot.Namespace;
      else
        return xmlType.Namespace;
    }
  }
}