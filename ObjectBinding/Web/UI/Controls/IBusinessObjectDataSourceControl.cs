using System;
using System.ComponentModel;
using System.Web.UI;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Web.Controls
{

public interface IBusinessObjectDataSourceControl : IBusinessObjectDataSource, IControl
{
}

public abstract class BusinessObjectDataSourceControl : Control, IBusinessObjectDataSourceControl
{
  protected override void Render (HtmlTextWriter writer)
  {
    //  No output, control is invisible
  }

  public virtual void LoadValues(bool interim)
  {
    DataSource.LoadValues (interim);
  }

  public virtual void SaveValues (bool interim)
  {
    DataSource.SaveValues (interim);
  }

  public virtual void Register (IBusinessObjectBoundControl control)
  {
    DataSource.Register (control);
  }

  public virtual void Unregister (IBusinessObjectBoundControl control)
  {
    DataSource.Unregister (control);
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [DefaultValue(true)]
  public virtual bool EditMode
  {
    get { return DataSource.EditMode; }
    set { DataSource.EditMode = value; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObject BusinessObject
  {
    get { return DataSource.BusinessObject; }
    set { DataSource.BusinessObject = value; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObjectClass BusinessObjectClass
  {
    get { return DataSource.BusinessObjectClass; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObjectProvider BusinessObjectProvider
  {
    get { return DataSource.BusinessObjectProvider; }
  }

  protected abstract BusinessObjectDataSource DataSource { get; }
}

}
