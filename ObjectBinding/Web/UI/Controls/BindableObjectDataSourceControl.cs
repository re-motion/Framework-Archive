using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.Design.BindableObject;
using Rubicon.ObjectBinding.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.UI.Controls
{
  [Designer (typeof (BindableObjectDataSourceDesigner))]
  public class BindableObjectDataSourceControl : BusinessObjectDataSourceControl
  {
    private readonly BindableObjectDataSource _dataSource = new BindableObjectDataSource();

    public BindableObjectDataSourceControl ()
    {
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [DefaultValue ("")]
    [Editor (typeof (BindableObjectTypePickerEditor), typeof (UITypeEditor))]
    public string TypeName
    {
      get { return _dataSource.TypeName; }
      set { _dataSource.TypeName = value; }
    }

    protected override IBusinessObjectDataSource GetDataSource ()
    {
      return _dataSource;
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      _dataSource.Site = Site;
    }
  }
}