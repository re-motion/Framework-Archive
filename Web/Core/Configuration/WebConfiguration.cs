using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Rubicon.Xml;

namespace Rubicon.Web.Configuration
{

[XmlType (WebConfiguration.ElementName, Namespace = WebConfiguration.SchemaUri)]
public class WebConfiguration: IConfigurationSectionHandler
{
  public const string ElementName = "rubicon.web";
  public const string SchemaUri = "http://www.rubicon-it.com/commons/web/configuration";

  public static XmlReader GetSchemaReader ()
  {
    return new XmlTextReader (Assembly.GetExecutingAssembly().GetManifestResourceStream (typeof(WebConfiguration), "WebConfiguration.xsd"));
  }

  private static WebConfiguration s_current = null;

  public static WebConfiguration Current
  {
    get
    {
      if (s_current == null)
      {
        lock (typeof (WebConfiguration))
        {
          if (s_current == null)
          {
            XmlNode section = (XmlNode) ConfigurationSettings.GetConfig (ElementName);
            if (section != null)
            {
              s_current = (WebConfiguration) XmlSerializationUtility.DeserializeUsingSchema (
                  new XmlNodeReader (section),
                  ElementName,
                  typeof (WebConfiguration),
                  SchemaUri, 
                  GetSchemaReader());
            }
            else
            {
              s_current = new WebConfiguration();
            }
          }
        }
      }
      return s_current;
    }
  }

  ExecutionEngineConfiguration _executionEngine = new ExecutionEngineConfiguration();
  WaiConfiguration _wai = new WaiConfiguration();
  ResourceConfiguration _resources = new ResourceConfiguration();

  [XmlElement ("executionEngine")]
  public ExecutionEngineConfiguration ExecutionEngine
  {
    get { return _executionEngine; }
    set { _executionEngine = value; }
  }

  [XmlElement ("wai")]
  public WaiConfiguration Wai
  {
    get { return _wai; }
    set { _wai = value; }
  }

  [XmlElement ("resources")]
  public ResourceConfiguration Resources
  {
    get { return _resources; }
    set { _resources = value; }
  }

  object IConfigurationSectionHandler.Create (object parent, object configContext, XmlNode section)
  {
    // instead of the CooNetConfiguration instance, the xml node is returned. this prevents version 
    // conflicts when multiple versions of this assembly are loaded within one AppDomain.
    return section;
  }
}

[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class ExecutionEngineConfiguration
{
  private int _functionTimeout = 20;
  private bool _viewStateInSession = true;

  /// <summary>
  /// Specifies the default timeout for individual functions within one session.
  /// </summary>
  [XmlAttribute ("functionTimeout")]
  public int FunctionTimeout
  {
    get { return _functionTimeout; }
    set { _functionTimeout = value; }
  }

  /// <summary>
  ///   Specifies whether the page view state should be stored in the session.
  /// </summary>
  [XmlAttribute ("viewStateInSession")]
  public bool ViewStateInSession
  {
    get { return _viewStateInSession; }
    set { _viewStateInSession = value; }
  }
}

[Flags]
public enum WaiLevel
{
  Undefined = 0,
  A = 1, 
  /// <summary> WAI level AA includes all requirements of level A. </summary>
  AA = 3,
  /// <summary> WAI level AAA includes all requirements of levels A and AA. </summary>
  AAA = 7
}

[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class WaiConfiguration
{
  private WaiLevel _level = WaiLevel.Undefined;
  private bool _debug = false;

  /// <summary>
  ///   Specifies the WAI level required in this web-application.
  /// </summary>
  [XmlAttribute ("level")]
  public WaiLevel Level
  {
    get { return _level; }
    set { _level = value; }
  }

  /// <summary>
  ///   Specifies whether the developer will be notified on WAI compliancy issues in the controls' configuration
  ///   or if they will be corrected automatically.
  /// </summary>
  [XmlAttribute ("debug")]
  public bool Debug
  {
    get { return _debug; }
    set { _debug = value; }
  }
}

[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class ResourceConfiguration
{
  private string _root = "res";
  private bool _relativeToApplicationRoot = true;

  /// <summary>
  /// Specifies the root folder for all resources.
  /// </summary>
  [XmlAttribute ("root")]
  public string Root
  {
    get { return _root; }
    set { _root = value; }
  }

  /// <summary>
  ///   Specifies whether the <see cref="Root"/> folder is relative to the application root.
  /// </summary>
  /// <value> 
  ///   If <see langword="true"/> the <see cref="Root"/> is prepended with the application root,
  ///   thereby transforming the resource path into an absolute path.
  /// </value>
  [XmlAttribute ("relativeToApplicationRoot")]
  public bool RelativeToApplicationRoot
  {
    get { return _relativeToApplicationRoot; }
    set { _relativeToApplicationRoot = value; }
  }
}

}
