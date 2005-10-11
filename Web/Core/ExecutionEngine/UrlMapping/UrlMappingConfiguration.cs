using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Rubicon.Xml;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.ExecutionEngine
{

[XmlType (WxeMapping.ElementName, Namespace = WxeMapping.SchemaUri)]
public class WxeMapping
{
  /// <summary> The name of the configuration section in the configuration file. </summary>
  /// <remarks> <c>rubicon.web</c> </remarks>
  public const string ElementName = "mapping";

  /// <summary> The namespace of the configuration section schema. </summary>
  /// <remarks> <c>http://www.rubicon-it.com/Commons/Web/ExecutionEngine/Mapping/1.0</c> </remarks>
  public const string SchemaUri = "http://www.rubicon-it.com/Commons/Web/ExecutionEngine/Mapping/1.0";

  /// <summary> Gets an <see cref="XmlReader"/> reader for the schema embedded in the assembly. </summary>
  public static XmlReader GetSchemaReader ()
  {
    return new XmlTextReader (Assembly.GetExecutingAssembly().GetManifestResourceStream (typeof(WxeMapping), "WxeMapping.xsd"));
  }

  private static WxeMapping s_current = null;

  /// <summary> Gets the <see cref="WebConfiguration"/> for the current thread. </summary>
  public static WxeMapping Current
  {
    get
    {
      if (s_current == null)
      {
        lock (typeof (WxeMapping))
        {
          if (s_current == null)
          {
            if (!File.Exists (WebConfiguration.Current.ExecutionEngine.MappingFile)) throw new FileNotFoundException (string.Format ("Mapping file '{0}' could not be found.", WebConfiguration.Current.ExecutionEngine.MappingFile), WebConfiguration.Current.ExecutionEngine.MappingFile);

            string mappingFile = Path.GetFullPath (WebConfiguration.Current.ExecutionEngine.MappingFile);

            XmlTextReader textReader = new XmlTextReader (mappingFile);
            if (textReader != null)
            {
              s_current = (WxeMapping) XmlSerializationUtility.DeserializeUsingSchema (
                  textReader,
                  mappingFile,
                  typeof (WxeMapping),
                  SchemaUri, 
                  GetSchemaReader());
            }
            else
            {
              s_current = new WxeMapping();
            }
          }
        }
      }
      return s_current;
    }
  }

  private WxeMappingRule[] _rules;
  private WxeMappingRule _singleRule;

  [XmlElement ("singleRule")]
  public WxeMappingRule SingleRule
  {
    get { return _singleRule; }
    set { _singleRule = value; }
  }

  [XmlArray ("rules")]
  public WxeMappingRule[] Rules
  {
    get { return _rules; }
    set { _rules = value; }
  }
}

[XmlType ("rule", Namespace = WxeMapping.SchemaUri)]
public class WxeMappingRule
{
  private string _functionType = null;
  private string _path = null;

  [XmlAttribute ("functionType")]
  public string FunctionType
  {
    get { return _functionType; }
    set { _functionType = value; }
  }

  [XmlAttribute ("path")]
  public string Path
  {
    get { return _path; }
    set { _path = value; }
  }
}

}
