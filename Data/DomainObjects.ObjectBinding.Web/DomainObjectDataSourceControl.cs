using System;
using System.IO;
using System.Xml;
using System.Web.UI;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.ObjectBinding.Design;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ObjectBinding.Web.Design;

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
    string projectPath = (string) ControlHelper.GetDesignTimePropertyValue (this, "ActiveFileSharePath");
    if (projectPath == null)
      return;

    string mappingFile = string.Empty;
    try
    {
      mappingFile = GetMappingFilePath (projectPath);
      MappingLoader mappingLoader = new MappingLoader (mappingFile, GetMappingSchemaPath (projectPath));
      MappingConfiguration.SetCurrent (new MappingConfiguration (mappingLoader));
    }
    catch (Exception e)
    {
      string message = string.Format ("Error while reading mapping configuration from file '{0}'.", mappingFile);
      _designTimeMappingException = new ApplicationException (message, e);
    }
  }

  internal ApplicationException GetDesignTimeException ()
  {
    return _designTimeMappingException;
  }

  private string GetMappingFilePath (string projectPath)
  {
    return GetFilePathFromWebConfig (
        projectPath,
        MappingLoader.ConfigurationAppSettingKey,
        projectPath + @"\bin\mapping.xml");
  }

  private string GetMappingSchemaPath (string projectPath)
  {
    return GetFilePathFromWebConfig (
        projectPath,
        MappingLoader.SchemaAppSettingKey,
        projectPath + @"\bin\mapping.xsd");
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
