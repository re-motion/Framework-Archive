using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ObjectBinding.Design;

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

    MappingLoader mappingLoader = new MappingLoader (projectPath + @"\bin\mapping.xml", projectPath + @"\bin\mapping.xsd");
    MappingConfiguration.SetCurrent (new MappingConfiguration (mappingLoader));
  }
}
}
