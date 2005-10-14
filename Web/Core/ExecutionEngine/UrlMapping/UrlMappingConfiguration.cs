using System;
using System.ComponentModel;
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

  private static MappingConfiguration s_current = null;

  public static MappingConfiguration CreateMappingConfiguration (string configurationFile)
  {
    MappingLoader loader = new MappingLoader (configurationFile, typeof (MappingConfiguration));
    return loader.CreateMappingConfiguration();
  }

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
            {
              s_current = new MappingConfiguration();
            }
            else
            {
              s_current = MappingConfiguration.CreateMappingConfiguration (
                  WebConfiguration.Current.ExecutionEngine.MappingFile);
            }
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

  private MappingRuleCollection _rules = new MappingRuleCollection();

  public MappingConfiguration()
  {
  }

  [XmlArray ("rules")]
  public MappingRuleCollection Rules
  {
    get { return _rules; }
  }
}

[XmlType ("rule", Namespace = MappingConfiguration.SchemaUri)]
public class MappingRule
{
  private string _functionTypeName = null;
  private Type _functionType = null;
  private string _path = null;

  public MappingRule()
  {
  }

  public MappingRule (Type functionType, string path)
  {
    ArgumentUtility.CheckNotNull ("functionType", functionType);

    _functionType = functionType;
    _functionTypeName = _functionType.AssemblyQualifiedName;
    Path = path;
  }

  public MappingRule (string functionTypeName, string path)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("functionTypeName", functionTypeName);
    ArgumentUtility.CheckNotNullOrEmpty ("path", path);

    _functionTypeName = functionTypeName;
    Path = path;
  }

  [XmlAttribute ("functionType")]
  public string FunctionTypeName
  {
    get 
    {
      return _functionTypeName; 
    }
    set
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      _functionTypeName = value; 
      _functionType = null;
    }
  }

  [XmlIgnore]
  public Type FunctionType
  {
    get
    {
      if (_functionType == null)
        _functionType = TypeUtility.GetType (_functionTypeName, true, true);
      return _functionType; 
    }
  }

  /// <summary> A path relative to the application root. Must not be <see langword-"null"/> or empty. </summary>
  /// <value> A virtual path, relative to the application root. Will always start with <c>~/</c>. </value>
  [XmlAttribute ("path")]
  public string Path
  {
    get 
    { 
      return _path; 
    }
    set 
    {
      ArgumentUtility.CheckNotNull ("value", value);
      value = value.Trim();
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      if (value.StartsWith ("/") || value.IndexOf (":") != -1)
        throw new ArgumentException (string.Format ("No absolute paths are allowed. Path: '{0}'", value), "value");
      if (! value.StartsWith ("~/"))
        value = "~/" + value;
      _path = value; 
    }
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

  public MappingRule this[string path]
  {
    get { return Find (path); }
  }

  public MappingRule this[Type functionType]
  {
    get { return Find (functionType); }
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
    MappingRule rule = (MappingRule) value;
    if (Find (rule.Path) != null)
      throw new ArgumentException (string.Format ("The mapping already contains a rule with a Path of '{0}'.", rule.Path), "value");
    if (Find (rule.FunctionType) != null)
      throw new ArgumentException (string.Format ("The mapping already contains a rule with a FunctionType of '{0}'.", rule.FunctionType), "value");
  }

  public Type FindType (string path)
  {
    MappingRule rule = Find (path);
    if (rule == null)
      return null;
    return rule.FunctionType;
  }

  public string FindPath (Type type)
  {
    MappingRule rule = Find (type);
    if (rule == null)
      return null;
    return rule.Path;
  }

  public string FindPath (string typeName)
  {
    if (StringUtility.IsNullOrEmpty (typeName))
      return null;
    Type type = TypeUtility.GetType (typeName, true, true);
    return FindPath (type);
  }

  public MappingRule Find (string path)
  {
    if (StringUtility.IsNullOrEmpty (path))
      return null;
    for (int i = 0; i < Count; i++)
    {
      if (string.Compare (this[i].Path, path, true) == 0)
        return this[i];
    }
    return null;
  }

  public MappingRule Find (Type functionType)
  {
    if (functionType == null)
      return null;
    for (int i = 0; i < Count; i++)
    {
      if (this[i].FunctionType == functionType)
        return this[i];
    }
    return null;
  }
}

}
