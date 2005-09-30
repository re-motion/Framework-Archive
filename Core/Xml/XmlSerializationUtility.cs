using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using log4net;

namespace Rubicon.Xml
{

public class XmlSerializationUtility
{
  private static readonly ILog s_log = LogManager.GetLogger (typeof (XmlSerializationUtility));

  public static object DeserializeUsingSchema (XmlTextReader reader, string context, Type type, string schemaUri, XmlReader schemaReader)
  {
    XmlValidatingReader validatingReader = null;
    try
    {
      XmlSerializer serializer = new XmlSerializer (type, schemaUri);
      validatingReader = new XmlValidatingReader (reader);
      validatingReader.Schemas.Add (schemaUri, schemaReader);
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

  public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, string schemaUri, XmlReader schemaReader)
  {
    XmlDocument document = new XmlDocument ();
    document.Load (reader);

    MemoryStream memoryStream = new MemoryStream ();
    document.Save (new StreamWriter (memoryStream, Encoding.Unicode));
    memoryStream.Seek (0, SeekOrigin.Begin);

    XmlTextReader textReader = new XmlTextReader (context, memoryStream, new NameTable());
    return DeserializeUsingSchema (textReader, context, type, schemaUri, schemaReader);
  }
}

}
