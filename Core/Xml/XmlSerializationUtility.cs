using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Rubicon.Logging;
using Rubicon.Utilities;

namespace Rubicon.Xml
{
  /// <summary>
  /// Use this class to easily serialize and deserialize objects to or from XML.
  /// </summary>
  public static class XmlSerializationUtility
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (XmlSerializationUtility));

    public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, string defaultNamespace, XmlReaderSettings settings)
    {
      ArgumentUtility.CheckNotNull ("reader", reader);
      ArgumentUtility.CheckNotNullOrEmpty ("context", context);
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("settings", settings);

      XmlSchemaValidationHandler validationHandler = new XmlSchemaValidationHandler (context);
      settings.ValidationEventHandler += validationHandler.Handler;
      settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

      XmlReader innerReader = XmlReader.Create (reader, settings);

      try
      {
        XmlSerializer serializer = new XmlSerializer (type, defaultNamespace);
        object result = serializer.Deserialize (innerReader);

        validationHandler.EnsureNoErrors();

        return result;
      }
      catch (InvalidOperationException e)
      {
        Exception actualException = e;
        while (actualException.InnerException != null)
          actualException = actualException.InnerException;

        string errorMessage = string.Format (
            "Error reading '{0}'. The value of {1} '{2}' could not be parsed: {3}",
            context,
            innerReader.NodeType.ToString().ToLower(),
            innerReader.Name,
            actualException.Message);
        s_log.Error (errorMessage, actualException);
            
        IXmlLineInfo lineInfo = (IXmlLineInfo) innerReader;
        if (lineInfo != null)
          throw new XmlException (errorMessage, e, lineInfo.LineNumber, lineInfo.LinePosition);
        else
          throw new XmlException (errorMessage, e);
      }
    }

    public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, string defaultNamespace, XmlSchemaSet schemas)
    {
      ArgumentUtility.CheckNotNull ("reader", reader);

      XmlReaderSettings settings = new XmlReaderSettings();
      settings.Schemas = schemas;
      settings.ValidationType = ValidationType.Schema;

      return DeserializeUsingSchema (reader, context, type, defaultNamespace, settings);
    }

    public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, XmlSchemaSet schemas)
    {
      return DeserializeUsingSchema (reader, context, type, GetNamespace (type), schemas);
    }

    public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, string schemaUri, XmlReader schemaReader)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("schemaUri", schemaUri);
      ArgumentUtility.CheckNotNull ("schemaReader", schemaReader);

      XmlSchemaSet schemas = new XmlSchemaSet();
      schemas.Add (schemaUri, schemaReader);
      return DeserializeUsingSchema (reader, context, type, GetNamespace (type), schemas);
    }

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