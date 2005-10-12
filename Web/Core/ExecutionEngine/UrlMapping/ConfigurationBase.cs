using System;
using System.Xml.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine.Mapping
{
/// <summary> Represents the common information all configuration classes provide. </summary>
public class ConfigurationBase
{
  // types

  // static members and constants

  // member fields

  private string _applicationName;
  private string _configurationFile;

  // construction and disposing
  protected ConfigurationBase (): this ("app", "file")
  {
  }

  /// <summary> Initializes a new instance of the <b>ConfigurationBase</b> type. </summary>
  protected ConfigurationBase (string applicationName, string configurationFile)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("applicationName", applicationName);
    ArgumentUtility.CheckNotNullOrEmpty ("configurationFile", configurationFile);

    _applicationName = applicationName;
    _configurationFile = configurationFile;
  }

  // methods and properties

  /// <summary> Gets the application name that is specified in the XML configuration file.  </summary>
  [XmlAttribute ("applicationName")]
  public string ApplicationName
  {
    get { return _applicationName; }
  }

  /// <summary> Gets the XML configuration file. </summary>
  [XmlIgnore]
  public string ConfigurationFile
  {
    get { return _configurationFile; }
  }
}
}
