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

  private ExecutionEngineConfiguration _executionEngine = new ExecutionEngineConfiguration();
  private WcagConfiguration _wcag = new WcagConfiguration();
  private ResourceConfiguration _resources = new ResourceConfiguration();
  private SmartNavigationConfiguration _smartNavigation = new SmartNavigationConfiguration();

  /// <summary> Gets or sets the <see cref="ExecutionEngineConfiguration"/> entry. </summary>
  [XmlElement ("executionEngine")]
  public ExecutionEngineConfiguration ExecutionEngine
  {
    get { return _executionEngine; }
    set { _executionEngine = value; }
  }

  /// <summary> Gets or sets the <see cref="WcagConfiguration"/> entry. </summary>
  [XmlElement ("wcag")]
  public WcagConfiguration Wcag
  {
    get { return _wcag; }
    set { _wcag = value; }
  }

  /// <summary> Gets or sets the <see cref="ResourceConfiguration"/> entry. </summary>
  [XmlElement ("resources")]
  public ResourceConfiguration Resources
  {
    get { return _resources; }
    set { _resources = value; }
  }

  /// <summary> Gets or sets the <see cref="SmartNavigationConfiguration"/> entry. </summary>
  [XmlElement ("smartNavigation")]
  public SmartNavigationConfiguration SmartNavigation
  {
    get { return _smartNavigation; }
    set { _smartNavigation = value; }
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
  private bool _enableSessionManagement = true;
  private int _refreshInterval = 10;
  private bool _viewStateInSession = true;

  /// <summary> Gets or sets the default timeout for individual functions within one session. </summary>
  /// <value> The timeout in mintues. Defaults to 20 minutes. </value>
  [XmlAttribute ("functionTimeout")]
  public int FunctionTimeout
  {
    get { return _functionTimeout; }
    set { _functionTimeout = value; }
  }

  /// <summary> Gets or sets a flag that determines whether session management is employed. </summary>
  /// <value> <see langword="true"/> to enable session management. Defaults to <see langword="true"/>. </value>
  /// <remarks> 
  ///   Session management relies on javascript and is fully supported by browsers other than 
  ///   <list type="bullet">
  ///     <item> Internet Explorer 5.01+</item>
  ///     <item> Firefox 0.9+</item>
  ///     <item> Mozila 1.7+</item>
  ///   </list>
  ///   Other browsers present no confirmation dialog upon leaving the page.
  /// </remarks>
  [XmlAttribute ("enableSessionManagement")]
  public bool EnableSessionManagement
  {
    get { return _enableSessionManagement; }
    set { _enableSessionManagement = value; }
  }

  /// <summary> Gets or sets the default refresh intervall for a function. </summary>
  /// <value> The timeout in mintues. Use zero to disable refreshing. Defaults to 10 minutes. </value>
  /// <remarks> 
  ///   The value must be less than both the <see cref="FunctionTimeout"/> and the 
  ///   <see cref="System.Web.SessionState.HttpSessionState.Timeout"/>.
  /// </remarks>
  [XmlAttribute ("refreshInterval")]
  public int RefreshInterval
  {
    get { return _refreshInterval; }
    set { _refreshInterval = value; }
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
///   Enumeration listing the possible WCAG conformance levels.
/// </summary>
[Flags]
public enum WaiConformanceLevel
{
  /// <summary> The web application is not required to follow the WAI guidelines. </summary>
  Undefined = 0,
  /// <summary> WAI conformance level A, all Priority 1 checkpoints are satisfied. </summary>
  A = 1, 
  /// <summary> WAI conformance level Double-A, all Priority 1 and 2 checkpoints are satisfied. </summary>
  DoubleA = 3,
  /// <summary> WAI conformance level Triple-A, all Priority 1, 2, and 3 checkpoints are satisfied. </summary>
  TripleA = 7
}

/// <summary> Configuration section entry for specifying the application wide WAI level. </summary>
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class WcagConfiguration
{
  private WaiConformanceLevel _conformanceLevel = WaiConformanceLevel.Undefined;
  private bool _debug = false;

  /// <summary> Gets or sets the WCAG conformance level required in this web-application. </summary>
  /// <value> A value of the <see cref="WaiConformanceLevel"/> enumeration. Defaults to <see cref="WaiConformanceLevel.Undefined"/>. </value>
  [XmlAttribute ("conformanceLevel")]
  public WaiConformanceLevel ConformanceLevel
  {
    get { return _conformanceLevel; }
    set { _conformanceLevel = value; }
  }

  /// <summary>
  ///   Gets or sets a flag specifying whether the developer will be notified on WAI compliancy issues in the 
  ///   controls' configuration or if they will be corrected automatically.
  /// </summary>
  /// <value> <see langword="true"/> to enable debug mode. Defaults to <see langword="false"/>. </value>
  /// <remarks> 
  ///   Controls in violation of the required WAI level throw a <see cref="Rubicon.Web.UI.WcagException"/> in debug mode.
  /// </remarks>
  [XmlAttribute ("debug")]
  public bool Debug
  {
    get { return _debug; }
    set { _debug = value; }
  }
}

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
/// <seealso cref="Rubicon.Web.ResourceUrlResolver"/>
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class ResourceConfiguration
{
  private string _root = "res";
  private ResourceRootMode _rootMode = ResourceRootMode.AbsoluteWithApplicationRoot;

  /// <summary> Gets or sets the root folder for all resources. </summary>
  /// <value> 
  ///   A string specifying an absolute path or a path relative to the application root. Defaults to <c>res</c>.
  /// </value>
  /// <remarks> Leading and trailing slashes are removed. </remarks>
/// <seealso cref="Rubicon.Web.ResourceUrlResolver"/>
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
  /// <value> 
  ///   A value of the <see cref="ResourceRootMode"/> enumeration. 
  ///   Defaults to <see cref="ResourceRootMode.Relative"/>.
  /// </value>
  [XmlAttribute ("rootPath")]
  public ResourceRootMode RootMode
  {
    get { return _rootMode; }
    set { _rootMode = value; }
  }
}

/// <summary> Configuration section entry for configuring the <b>Rubicon.Web.ExecutionEngine</b>. </summary>
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class SmartNavigationConfiguration
{
  private bool _enableScrolling = true;
  private bool _enableFocusing = true;

  /// <summary> Gets or sets a flag that determines whether to enable smart scrolling. </summary>
  /// <value> <see langword="true"/> to enable smart scrolling. Defaults to <see langword="true"/>. </value>
  [XmlAttribute ("enableScrolling")]
  public bool EnableScrolling
  {
    get { return _enableScrolling; }
    set { _enableScrolling = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to enable smart focus. </summary>
  /// <value> <see langword="true"/> to enable smart focusing. Defaults to <see langword="true"/>. </value>
  [XmlAttribute ("enableFocusing")]
  public bool EnableFocusing
  {
    get { return _enableFocusing; }
    set { _enableFocusing = value; }
  }
}
}
