using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Web.UI;
using System.Xml;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.ObjectBinding.Design;
using Rubicon.Data.DomainObjects.ObjectBinding.Web.Design;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web
{
[Designer (typeof (DomainObjectDataSourceDesigner))]
public class DomainObjectDataSourceControl : BusinessObjectDataSourceControl
{
  private DomainObjectDataSource _dataSource = new DomainObjectDataSource();
  private ApplicationException _designTimeMappingException = null;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    SetDesignTimeMapping ();
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [DefaultValue("")]
  [Editor (typeof (ClassPickerEditor), typeof (UITypeEditor))]
  public string TypeName
  {
    get 
    { 
      return _dataSource.TypeName; 
    }
    set
    { 
      _dataSource.TypeName = value; 
    }
  }

  protected override IBusinessObjectDataSource GetDataSource()
  {
    return _dataSource;
  }

  private void SetDesignTimeMapping ()
  {
    string projectPath = GetProjectPath();
    if (projectPath == null)
      return;

    string mappingFile = string.Empty;
    try
    {
      mappingFile = GetMappingFilePath (projectPath);
      MappingLoader mappingLoader = new MappingLoader (mappingFile, true);
      MappingConfiguration.SetCurrent (new MappingConfiguration (mappingLoader));
    }
    catch (Exception e)
    {
      string message = string.Format ("Error while reading mapping configuration from file '{0}'.", mappingFile);
      _designTimeMappingException = new ApplicationException (message, e);
    }
  }

  private string GetProjectPath ()
  {
    string projectPath = (string)ControlHelper.GetDesignTimePropertyValue (this, "ActiveFileSharePath");

#if !NET11
    if (projectPath == null)
      projectPath = (string)ControlHelper.GetDesignTimePropertyValue (this, "FullPath");
#endif

    return projectPath;
  }

  internal ApplicationException GetDesignTimeException ()
  {
    return _designTimeMappingException;
  }

  private string GetMappingFilePath (string projectPath)
  {
    //TODO: use MappingLoader.DefaultConfigurationFile here
    return GetFilePathFromWebConfig (
        projectPath,
        MappingLoader.ConfigurationAppSettingKey,
        projectPath + @"\bin\" + MappingLoader.DefaultConfigurationFile);
  }

  private string GetFilePathFromWebConfig (string projectPath, string configurationKey, string defaultPath)
  {
    string webConfigFilePath = projectPath + "\\web.config";
    string filePath = GetValueFromWebConfig (webConfigFilePath, configurationKey);
    if (filePath == null || filePath == string.Empty)
      return defaultPath;

    if (!File.Exists (filePath))
      return defaultPath;

    return filePath;
  }

  private string GetValueFromWebConfig (string webConfigFilePath, string configurationKey)
  {
    XmlDocument webConfigDocument = new XmlDocument();
    webConfigDocument.Load (webConfigFilePath);
    string xPath = string.Format ("/configuration/appSettings/add/@value[../@key='{0}']", configurationKey);
    XmlNode valueNode = webConfigDocument.SelectSingleNode (xPath);
    if (valueNode == null)
      return null;

    return valueNode.InnerText;
  }
}
}
