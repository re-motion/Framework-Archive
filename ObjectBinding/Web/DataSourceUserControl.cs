using System;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding.Web
{

public interface IDataSourceContainer
{
  IBusinessObjectDataSource DataSource { get; }
}

[Obsolete ("Implement using IBusinessObjectDataSourceControl.")]
public class DataSourceUserControl: UserControl, IBusinessObjectDataSource, IGetComponentBindingExpression, IResolveComponentBindingExpression
{
  public UserControl UserControl
  {
    get { return this; }
  }

  protected virtual IBusinessObjectDataSource DataSource { get { throw new NotImplementedException ("This method must be overridden."); } }

  string IGetComponentBindingExpression.BindingExpression 
  {
    get { return "UserControl"; }
  }

  IComponent IResolveComponentBindingExpression.Resolve (string expression)
  {
    if (expression == "UserControl")
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

  bool IBusinessObjectDataSource.EditMode
  {
    get { return DataSource.EditMode; }
    set { DataSource.EditMode = value; }
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
}

}
