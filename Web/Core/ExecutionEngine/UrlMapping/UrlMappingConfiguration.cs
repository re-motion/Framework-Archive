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
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.ExecutionEngine.UrlMapping
{

/// <summary> Contains the configuration data for the URL mapping system of the execution engine. </summary>
/// <include file='doc\include\ExecutionEngine\UrlMapping\UrlMappingConfiguration.xml' path='UrlMappingConfiguration/Class/*' />
[XmlType (UrlMappingConfiguration.ElementName, Namespace = UrlMappingConfiguration.SchemaUri)]
public class UrlMappingConfiguration: ConfigurationBase
{
  /// <summary> The name of the root element. </summary>
  /// <remarks> <c>mapping</c> </remarks>
  public const string ElementName = "urlMapping";

  /// <summary> The namespace of the mapping's schema. </summary>
  /// <remarks> <c>http://www.rubicon-it.com/Commons/Web/ExecutionEngine/UrlMapping/1.0</c> </remarks>
  public const string SchemaUri = "http://www.rubicon-it.com/Commons/Web/ExecutionEngine/UrlMapping/1.0";

  private static UrlMappingConfiguration s_current = null;

  public static UrlMappingConfiguration CreateUrlMappingConfiguration (string configurationFile)
  {
    UrlMappingLoader loader = new UrlMappingLoader (configurationFile, typeof (UrlMappingConfiguration));
    return loader.CreateUrlMappingConfiguration();
  }

  /// <summary> Gets the current <see cref="UrlMappingConfiguration"/>. </summary>
  public static UrlMappingConfiguration Current
  {
    get
    {
      if (s_current == null)
      {
        lock (typeof (UrlMappingConfiguration))
        {
          if (s_current == null)
          {
            string mappingFile = WebConfiguration.Current.ExecutionEngine.UrlMappingFile;
            if (StringUtility.IsNullOrEmpty (mappingFile))
            {
              s_current = new UrlMappingConfiguration();
            }
            else
            {
              s_current = UrlMappingConfiguration.CreateUrlMappingConfiguration (
                  WebConfiguration.Current.ExecutionEngine.UrlMappingFile);
            }
          }
        }
      }
      return s_current;
    }
  }

  /// <summary> Sets the current <see cref="UrlMappingConfiguration"/>. </summary>
  public static void SetCurrent (UrlMappingConfiguration mappingConfiguration)
  {
    lock (typeof (UrlMappingConfiguration))
    {
      s_current = mappingConfiguration;
    }
  }

  private UrlMappingCollection _mappings = new UrlMappingCollection();

  public UrlMappingConfiguration()
  {
  }

  [XmlElement ("add")]
  public UrlMappingCollection Mappings
  {
    get { return _mappings; }
  }
}

[XmlType ("add", Namespace = UrlMappingConfiguration.SchemaUri)]
public class UrlMappingEntry
{
  private string _functionTypeName = null;
  private Type _functionType = null;
  private string _resource = null;

  public UrlMappingEntry()
  {
  }

  public UrlMappingEntry (Type functionType, string resource)
  {
    FunctionType = functionType;
    Resource = resource;
  }

  public UrlMappingEntry (string functionTypeName, string resource)
  {
    FunctionTypeName = functionTypeName;
    Resource = resource;
  }

  /// <summary>
  ///   The <see cref="Type"/> name of the <see cref="WxeFunction"/> identified by the <see cref="Resource"/>. 
  ///   Must not be <see langword="null"/> or empty. 
  /// </summary>
  /// <value> 
  ///   A valid type name as defined by the 
  ///   <see cref="TypeUtility.ParseAbbreviatedTypeName">TypeUtility.ParseAbbreviatedTypeName</see> method.
  /// </value>
  [XmlAttribute ("type")]
  public string FunctionTypeName
  {
    get 
    {
      return _functionTypeName; 
    }
    set
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      FunctionType = TypeUtility.GetType (value, true, true);
    }
  }

  /// <summary> 
  ///   The <see cref="Type"/> of the <see cref="WxeFunction"/> identified by the <see cref="Resource"/>. 
  ///   Must not be <see langword="null"/>. 
  /// </summary>
  /// <value> A <see cref="Type"/> derived from the <see cref="WxeFunction"/> type. </value>
  [XmlIgnore]
  public Type FunctionType
  {
    get
    {
      return _functionType; 
    }
    set
    {
      ArgumentUtility.CheckNotNull ("value", value);
      if (! typeof (WxeFunction).IsAssignableFrom (value))
        throw new ArgumentException (string.Format ("The FunctionType '{0}' must be derived from WxeFunction.", value), "value");
      _functionType = value;
      _functionTypeName = _functionType.AssemblyQualifiedName;
    }
  }

  /// <summary> 
  ///   The path associated with the <see cref="FunctionType"/>. Must not be <see langword="null"/> or empty. 
  /// </summary>
  /// <value> A virtual path, relative to the application root. Will always start with <c>~/</c>. </value>
  [XmlAttribute ("resource")]
  public string Resource
  {
    get 
    { 
      return _resource; 
    }
    set 
    {
      ArgumentUtility.CheckNotNull ("value", value);
      value = value.Trim();
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      if (value.StartsWith ("/") || value.IndexOf (":") != -1)
        throw new ArgumentException (string.Format ("No absolute paths are allowed. Resource: '{0}'", value), "value");
      if (! value.StartsWith ("~/"))
        value = "~/" + value;
      _resource = value; 
    }
  }
}

public class UrlMappingCollection: CollectionBase
{
  public UrlMappingCollection()
  {
  }

  public UrlMappingEntry this[int index]
  {
    get { return (UrlMappingEntry) List[index]; }
    set { List[index] = value; }
  }

  public UrlMappingEntry this[string path]
  {
    get { return Find (path); }
  }

  public UrlMappingEntry this[Type functionType]
  {
    get { return Find (functionType); }
  }

  public int Add (UrlMappingEntry entry)
  {
    return List.Add (entry);
  }

  public void Remove (UrlMappingEntry entry)
  {
    if (entry != null)
      List.Remove (entry);
  }

  protected virtual void ValidateNewValue (object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (UrlMappingEntry));
    base.OnValidate (value);
    UrlMappingEntry entry = (UrlMappingEntry) value;
    if (Find (entry.Resource) != null)
      throw new ArgumentException (string.Format ("The mapping already contains an entry for the following resource: '{0}'.", entry.Resource), "value");
  }

  protected override void OnInsert(int index, object value)
  {
    ValidateNewValue (value);
    base.OnInsert (index, value);
  }

  protected override void OnSet(int index, object oldValue, object newValue)
  {
    ValidateNewValue (newValue);
    base.OnSet (index, oldValue, newValue);
  }

  public Type FindType (string path)
  {
    UrlMappingEntry entry = Find (path);
    if (entry == null)
      return null;
    return entry.FunctionType;
  }

  public string FindResource (Type type)
  {
    UrlMappingEntry entry = Find (type);
    if (entry == null)
      return null;
    return entry.Resource;
  }

  public string FindResource (string typeName)
  {
    if (StringUtility.IsNullOrEmpty (typeName))
      return null;
    Type type = TypeUtility.GetType (typeName, true, true);
    return FindResource (type);
  }

  public UrlMappingEntry Find (string path)
  {
    if (StringUtility.IsNullOrEmpty (path))
      return null;
    for (int i = 0; i < Count; i++)
    {
      if (string.Compare (this[i].Resource, path, true) == 0)
        return this[i];
    }
    return null;
  }

  public UrlMappingEntry Find (Type functionType)
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
