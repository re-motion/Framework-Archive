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

[XmlType (MappingConfiguration.ElementName, Namespace = MappingConfiguration.SchemaUri)]
public class MappingConfiguration: ConfigurationBase
{
  /// <summary> The name of the root element. </summary>
  /// <remarks> <c>mapping</c> </remarks>
  public const string ElementName = "mapping";

  /// <summary> The namespace of the mapping's schema. </summary>
  /// <remarks> <c>http://www.rubicon-it.com/Commons/Web/ExecutionEngine/Mapping/1.0</c> </remarks>
  public const string SchemaUri = "http://www.rubicon-it.com/Commons/Web/ExecutionEngine/Mapping/1.0";

  /// <summary> Gets an <see cref="XmlReader"/> reader for the schema embedded in the assembly. </summary>
  public static XmlReader GetSchemaReader ()
  {
    Stream schema = Assembly.GetExecutingAssembly().GetManifestResourceStream (typeof(MappingConfiguration), "WxeMapping.xsd");
    return new XmlTextReader (schema);
  }

  private static MappingConfiguration s_current = null;

  /// <summary> Gets the current <see cref="MappingConfiguration"/>. </summary>
  public static MappingConfiguration Current
  {
    get
    {
      if (s_current == null)
      {
        lock (typeof (MappingConfiguration))
        {
          if (s_current == null)
          {
            string mappingFile = WebConfiguration.Current.ExecutionEngine.MappingFile;
            if (StringUtility.IsNullOrEmpty (mappingFile))
              s_current = new MappingConfiguration();
            else
              s_current = LoadMappingFromFile (mappingFile);
          }
        }
      }
      return s_current;
    }
  }

  /// <summary> Sets the current <see cref="MappingConfiguration"/>. </summary>
  public static void SetCurrent (MappingConfiguration mapping)
  {
    lock (typeof (MappingConfiguration))
    {
      s_current = mapping;
    }
  }

  public static MappingConfiguration LoadMappingFromFile (string file)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("file", file);
    if (! File.Exists (file))
      throw new FileNotFoundException (string.Format ("Execution Engine mapping file '{0}' could not be found.", file), file);
    string fullPath = Path.GetFullPath (file);

    XmlTextReader reader = new XmlTextReader (fullPath);
    XmlSchemaCollection schemas = new XmlSchemaCollection();
    schemas.Add (SchemaUri, MappingConfiguration.GetSchemaReader());

    return (MappingConfiguration) XmlSerializationUtility.DeserializeUsingSchema (
        reader, file, typeof (MappingConfiguration), schemas);
  }
  
  private MappingRuleCollection _rules = new MappingRuleCollection();

  [XmlArray ("rules")]
  public MappingRuleCollection Rules
  {
    get { return _rules; }
  }
}

[XmlType ("rule", Namespace = MappingConfiguration.SchemaUri)]
public class MappingRule
{
  private string _functionType = null;
  private string _path = null;

  public MappingRule()
  {
  }

  public MappingRule (string functionType, string path)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("functionType", functionType);
    ArgumentUtility.CheckNotNullOrEmpty ("path", path);

    _functionType = functionType;
    _path = path;
  }

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

public class MappingRuleCollection: CollectionBase
{
  public MappingRuleCollection()
  {
  }

  public MappingRule this[int index]
  {
    get { return (MappingRule) List[index]; }
    set { List[index] = value; }
  }

  public int Add (MappingRule rule)
  {
    return List.Add (rule);
  }

  public void Remove (MappingRule rule)
  {
    List.Remove (rule);
  }

  protected override void OnValidate (object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (MappingRule));
    base.OnValidate (value);

  }

  public MappingRule Find (string path)
  {
    return null;
  }

  public MappingRule Find (Type functionType)
  {
    return null;
  }
}

}
