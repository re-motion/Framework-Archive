using System;
using System.Xml;
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
    try
    {
      XmlSerializer serializer = new XmlSerializer (type, schemaUri);
      XmlValidatingReader validatingReader = new XmlValidatingReader (reader);
      validatingReader.Schemas.Add (schemaUri, schemaReader);
      XmlSchemaValidationHandler validationHandler = new XmlSchemaValidationHandler (context);
      validatingReader.ValidationEventHandler += validationHandler.Handler;
      object result = serializer.Deserialize (validatingReader);
      validationHandler.EnsureNoErrors();
      return result;
    }
    catch (Exception e)
    {
      s_log.Error (string.Format ("Error reading {0}: {1}", context, e.Message), e);
      throw;
    }
  }

  public static object DeserializeUsingSchema (XmlReader reader, string context, Type type, string schemaUri, XmlReader schemaReader)
  {
    XmlDocument document = new XmlDocument ();
    document.Load (reader);

    MemoryStream memoryStream = new MemoryStream ();
    document.Save (new StreamWriter (memoryStream, Encoding.Unicode));
    memoryStream.Seek (0, SeekOrigin.Begin);

    XmlTextReader textReader = new XmlTextReader (memoryStream);
    return DeserializeUsingSchema (textReader, context, type, schemaUri, schemaReader);
  }
}

}
