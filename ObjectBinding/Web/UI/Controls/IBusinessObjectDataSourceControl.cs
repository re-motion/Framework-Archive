using System;
using System.ComponentModel;
using System.Web.UI;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Web.Controls
{

public interface IBusinessObjectDataSourceControl: IBusinessObjectDataSource, IControl
{
}

public abstract class BusinessObjectDataSourceControl: Control, IBusinessObjectDataSourceControl
{
  protected override void Render (HtmlTextWriter writer)
  {
    //  No output, control is invisible
  }

  public virtual void LoadValues(bool interim)
  {
    GetDataSource().LoadValues (interim);
  }

  public virtual void SaveValues (bool interim)
  {
    GetDataSource().SaveValues (interim);
  }

  public virtual void Register (IBusinessObjectBoundControl control)
  {
    GetDataSource().Register (control);
  }

  public virtual void Unregister (IBusinessObjectBoundControl control)
  {
    GetDataSource().Unregister (control);
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [DefaultValue(true)]
  public virtual bool EditMode
  {
    get { return GetDataSource().EditMode; }
    set { GetDataSource().EditMode = value; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObject BusinessObject
  {
    get { return GetDataSource().BusinessObject; }
    set { GetDataSource().BusinessObject = value; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObjectClass BusinessObjectClass
  {
    get { return GetDataSource().BusinessObjectClass; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObjectProvider BusinessObjectProvider
  {
    get { return GetDataSource().BusinessObjectProvider; }
  }

  protected abstract IBusinessObjectDataSource GetDataSource();
}

}
