using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Rubicon.Xml;

namespace Rubicon.Web.Configuration
{

/// <summary> Configuration section entry for configuring the <b>Rubicon.Web.ExecutionEngine</b>. </summary>
/// <include file='doc\include\Configuration\ExecutionEngineConfiguration.xml' path='ExecutionEngineConfiguration/Class/*' />
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
  /// <include file='doc\include\Configuration\ExecutionEngineConfiguration.xml' path='ExecutionEngineConfiguration/EnableSessionManagement/*' />
  [XmlAttribute ("enableSessionManagement")]
  public bool EnableSessionManagement
  {
    get { return _enableSessionManagement; }
    set { _enableSessionManagement = value; }
  }

  /// <summary> Gets or sets the default refresh intervall for a function. </summary>
  /// <include file='doc\include\Configuration\ExecutionEngineConfiguration.xml' path='ExecutionEngineConfiguration/EnableSessionManagement/*' />
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

}
