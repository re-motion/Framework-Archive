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

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web
{
public class DomainObjectDataSourceControl : BusinessObjectDataSourceControl
{
  private DomainObjectDataSource _dataSource = new DomainObjectDataSource();

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

    try
    {
      MappingLoader mappingLoader = new MappingLoader (GetMappingFilePath (projectPath), GetMappingSchemaPath (projectPath));
      MappingConfiguration.SetCurrent (new MappingConfiguration (mappingLoader));
    }
    catch (Exception e)
    {
      // MK: Exception.Message is shown when mapping in invalid. Please remove reference to System.Windows.Forms.dll when finished.
      // Many thanks!
      MessageBox.Show (e.Message, "Error while reading mapping configuration");
    }
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
