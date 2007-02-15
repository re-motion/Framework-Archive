using System;
using System.ComponentModel;
using System.Web.UI;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding.Web
{

[Obsolete ("Implement using IBusinessObjectDataSourceControl.")]
public class DataSourcePage: Page, IBusinessObjectDataSource, IGetComponentBindingExpression, IResolveComponentBindingExpression
{
  protected virtual IBusinessObjectDataSource DataSource { get { throw new NotImplementedException ("This method must be overridden."); } }

  string IGetComponentBindingExpression.BindingExpression 
  {
    get { return "Page"; }
  }

  IComponent IResolveComponentBindingExpression.Resolve (string expression)
  {
    if (expression == "Page")
      return this;
    else if (this.Site != null && this.Site.Container != null && this.Site.Container.Components != null)
      return this.Site.Container.Components[expression];
    else
      return null;
  }

  void IBusinessObjectDataSource.LoadValues (bool interim)
  {
    DataSource.LoadValues (interim);
  }

  void IBusinessObjectDataSource.Register (IBusinessObjectBoundControl control)
  {
    DataSource.Register (control);
  }

  IBusinessObject IBusinessObjectDataSource.BusinessObject
  {
    get { return DataSource.BusinessObject; }
    set { DataSource.BusinessObject = value; }
  }

  IBusinessObjectProvider IBusinessObjectDataSource.BusinessObjectProvider
  {
    get { return DataSource.BusinessObjectProvider; }
  }

  DataSourceMode IBusinessObjectDataSource.Mode
  {
    get { return DataSource.Mode; }
    set { DataSource.Mode = value; }
  }

  void IBusinessObjectDataSource.Unregister (IBusinessObjectBoundControl control)
  {
    DataSource.Unregister (control);
  }

  IBusinessObjectClass IBusinessObjectDataSource.BusinessObjectClass
  {
    get { return DataSource.BusinessObjectClass; }
  }

  void IBusinessObjectDataSource.SaveValues (bool interim)
  {
    DataSource.SaveValues (interim);
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectBoundControl[] BoundControls
  {
    get { return DataSource.BoundControls; }
  }
}

}
