using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectDataSourceControl : BusinessObjectDataSourceControl
{
  private ReflectionBusinessObjectDataSource _dataSource = new ReflectionBusinessObjectDataSource();

	public ReflectionBusinessObjectDataSourceControl()
	{}

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [DefaultValue("")]
  public string TypeName
  {
    get { return _dataSource.TypeName; }
    set { _dataSource.TypeName = value; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Data")]
  public Type Type
  {
    get { return _dataSource.Type; }
  }

  protected override BusinessObjectDataSource DataSource
  {
    get { return _dataSource; }
  }
}

}
