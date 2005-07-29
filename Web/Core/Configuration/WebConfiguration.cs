using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Rubicon.Xml;

namespace Rubicon.Web.Configuration
{

/// <summary> The configuration section for <b>Rubicon.Web</b>. </summary>
/// <remarks> Use this classes <see cref="Current"/> property to read the configuration settings from your code. </remarks>
/// <seealso cref="IConfigurationSectionHandler"/>
/// <example>
///   Register the configuration section in the configuration file within the <c>configSections</c> element.
///   The <c>configSections</c> element must be precede all configuration sections.
/// <code>
/// &lt;?xml version="1.0" encoding="utf-8" ?&gt;
/// &lt;configuration&gt;
///   &lt;configSections&gt;
///     &lt;section name="rubicon.web" type="Rubicon.Web.Configuration.WebConfiguration, Rubicon.Web" /&gt;
///     &lt;!-- Other configuration section registrations. --&gt;
///   &lt;/configSections&gt;
///   &lt;!-- The configuration sections. --&gt;
/// &lt;/configuration&gt;
/// </code>
///   Create the configuration section for <c>rubicon.web</c>.
/// <code>
/// &lt;rubicon.web xmlns="http://www.rubicon-it.com/commons/web/configuration"&gt; 
///   &lt;!-- The configuration section entries. --&gt;
/// &lt;/rubicon.web&gt;
/// </code>
/// </example>
[XmlType (WebConfiguration.ElementName, Namespace = WebConfiguration.SchemaUri)]
public class WebConfiguration: IConfigurationSectionHandler
{
  /// <summary> The name of the configuration section in the configuration file. </summary>
  /// <remarks> <c>rubicon.web</c> </remarks>
  public const string ElementName = "rubicon.web";

  /// <summary> The namespace of the configuration section schema. </summary>
  /// <remarks> <c>http://www.rubicon-it.com/commons/web/configuration</c> </remarks>
  public const string SchemaUri = "http://www.rubicon-it.com/commons/web/configuration";

  /// <summary> Gets an <see cref="XmlReader"/> reader for the schema embedded in the assembly. </summary>
  public static XmlReader GetSchemaReader ()
  {
    return new XmlTextReader (Assembly.GetExecutingAssembly().GetManifestResourceStream (typeof(WebConfiguration), "WebConfiguration.xsd"));
  }

  private static WebConfiguration s_current = null;

  /// <summary> Gets the <see cref="WebConfiguration"/> for the current thread. </summary>
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

  /// <summary> Gets or sets the <see cref="ExecutionEngineConfiguration"/> entry. </summary>
  [XmlElement ("executionEngine")]
  public ExecutionEngineConfiguration ExecutionEngine
  {
    get { return _executionEngine; }
    set { _executionEngine = value; }
  }

  /// <summary> Gets or sets the <see cref="WaiConfiguration"/> entry. </summary>
  [XmlElement ("wai")]
  public WaiConfiguration Wai
  {
    get { return _wai; }
    set { _wai = value; }
  }

  /// <summary> Gets or sets the <see cref="ResourceConfiguration"/> entry. </summary>
  [XmlElement ("resources")]
  public ResourceConfiguration Resources
  {
    get { return _resources; }
    set { _resources = value; }
  }

  object IConfigurationSectionHandler.Create (object parent, object configContext, XmlNode section)
  {
    // instead of the WebConfiguration instance, the xml node is returned. this prevents version 
    // conflicts when multiple versions of this assembly are loaded within one AppDomain.
    return section;
  }
}

/// <summary> Configuration section entry for configuring the <b>Rubicon.Web.ExecutionEngine</b>. </summary>
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class ExecutionEngineConfiguration
{
  private int _functionTimeout = 20;
  private bool _viewStateInSession = true;

  /// <summary> Gets or sets the default timeout for individual functions within one session. </summary>
  /// <value> The timeout in mintues. Defaults to 20 minutes. </value>
  [XmlAttribute ("functionTimeout")]
  public int FunctionTimeout
  {
    get { return _functionTimeout; }
    set { _functionTimeout = value; }
  }

  /// <summary> Gets or sets a flag specifying whether the page view state should be stored in the session. </summary>
  /// <value> <see langword="true"/> to use the session. Defaults to <see langword="true"/>. </value>
  [XmlAttribute ("viewStateInSession")]
  public bool ViewStateInSession
  {
    get { return _viewStateInSession; }
    set { _viewStateInSession = value; }
  }
}

/// <summary>
///   Enumeration listing the possible WAI levels.
/// </summary>
[Flags]
public enum WaiLevel
{
  /// <summary> The application is not required to follow the WAI guidelines. </summary>
  Undefined = 0,
  /// <summary> WAI level A. </summary>
  A = 1, 
  /// <summary> WAI level AA, includes all requirements of level A. </summary>
  AA = 3,
  /// <summary> WAI level AAA, includes all requirements of levels A and AA. </summary>
  AAA = 7
}

/// <summary> Configuration section entry for specifying the application wide WAI level. </summary>
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class WaiConfiguration
{
  private WaiLevel _level = WaiLevel.Undefined;
  private bool _debug = false;

  /// <summary> Gets or sets the WAI level required in this web-application. </summary>
  /// <value> A value of the <see cref="WaiLevel"/> enumeration. Defaults to <see cref="WaiLevel.Undefined"/>. </value>
  [XmlAttribute ("level")]
  public WaiLevel Level
  {
    get { return _level; }
    set { _level = value; }
  }

  /// <summary>
  ///   Gets or sets a flag specifying whether the developer will be notified on WAI compliancy issues in the 
  ///   controls' configuration or if they will be corrected automatically.
  /// </summary>
  /// <value> <see langword="true"/> to enable debug mode. Defaults to <see langword="false"/>. </value>
  /// <remarks> 
  ///   Controls in violation of the required WAI level throw a <see cref="Rubicon.Web.UI.WaiException"/> in debug mode.
  /// </remarks>
  [XmlAttribute ("debug")]
  public bool Debug
  {
    get { return _debug; }
    set { _debug = value; }
  }
}

/// <summary> Configuration section entry for specifying the resources root. </summary>
/// <seealso cref="Rubicon.Web.ResourceUrlResolver"/>
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class ResourceConfiguration
{
  private string _root = "res";
  private bool _relativeToApplicationRoot = true;

  /// <summary> Gets or sets the root folder for all resources. </summary>
  /// <value> 
  ///   A string specifying an absolute path or a path relative to the application root. Defaults to <c>res</c>.
  /// </value>
  /// <remarks> Trailing slashes are removed. </remarks>
/// <seealso cref="Rubicon.Web.ResourceUrlResolver"/>
  [XmlAttribute ("root")]
  public string Root
  {
    get { return _root; }
    set { _root = Rubicon.Utilities.StringUtility.NullToEmpty(value).TrimEnd ('/'); }
  }

  /// <summary> 
  ///   Gets or sets a flag Specifying whether the <see cref="Root"/> folder is relative to the application root. 
  /// </summary>
  /// <value> 
  ///   If <see langword="true"/>, the <see cref="Root"/> is prepended with the application root,
  ///   thereby transforming the resource path into an absolute path. Defaults to <see langword="true"/>.
  /// </value>
  [XmlAttribute ("relativeToApplicationRoot")]
  public bool RelativeToApplicationRoot
  {
    get { return _relativeToApplicationRoot; }
    set { _relativeToApplicationRoot = value; }
  }
}

}
