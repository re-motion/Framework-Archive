using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Rubicon.Xml;

namespace Rubicon.Web.Configuration
{

/// <summary>
///   Enumeration listing the options for interpreting the <see cref="ResourceConfiguration.Root"/> folder setting.
/// </summary>
public enum ResourceRootMode
{
  /// <summary> The <see cref="ResourceConfiguration.Root"/> folder specifies a relative path. </summary>
  Relative,
  /// <summary> The <see cref="ResourceConfiguration.Root"/> folder is transformed into an absolute path.  </summary>
  Absolute, 
  /// <summary> 
  ///   The <see cref="ResourceConfiguration.Root"/> folder is prepended with the application root and 
  ///   transformed into an absolute path.
  /// </summary>
  AbsoluteWithApplicationRoot
}

/// <summary> Configuration section entry for specifying the resources root. </summary>
/// <include file='doc\include\Configuration\ResourceConfiguration.xml' path='ResourceConfiguration/Class/*' />
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class ResourceConfiguration
{
  private string _root = "res";

  /// <summary> Gets or sets the root folder for all resources. </summary>
  /// <include file='doc\include\Configuration\ResourceConfiguration.xml' path='ResourceConfiguration/Root/*' />
  [XmlAttribute ("root")]
  public string Root
  {
    get { return _root; }
    set { _root = Rubicon.Utilities.StringUtility.NullToEmpty(value).Trim ('/'); }
  }

  /// <summary> 
  ///   Gets or sets a value specifying whether the <see cref="Root"/> folder is relative, absolute, or 
  ///   absolute and prepended with the the application root. 
  /// </summary>
  /// <include file='doc\include\Configuration\ResourceConfiguration.xml' path='ResourceConfiguration/RootMode/*' />
  public ResourceRootMode RootMode
  {
    get { return ResourceRootMode.AbsoluteWithApplicationRoot; }
  }
}

}
