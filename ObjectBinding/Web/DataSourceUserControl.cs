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

  void IObjectBindingDataSource.LoadValues (bool interim)
  {
    DataSource.LoadValues (interim);
  }

  void IObjectBindingDataSource.Register (IBusinessObjectBoundControl control)
  {
    DataSource.Register (control);
  }

  IBusinessObject IBusinessObjectDataSource.BusinessObject
  {
    get { return DataSource.BusinessObject; }
    set { DataSource.BusinessObject = value; }
  }

  IBusinessObjectProvider IObjectBindingDataSource.BusinessObjectProvider
  {
    get { return DataSource.BusinessObjectProvider; }
  }

  bool IObjectBindingDataSource.IsWritable
  {
    get { return DataSource.IsWritable; }
  }

  void IObjectBindingDataSource.Unregister (IBusinessObjectBoundControl control)
  {
    DataSource.Unregister (control);
  }

  IBusinessObjectClass IObjectBindingDataSource.BusinessObjectClass
  {
    get { return DataSource.BusinessObjectClass; }
  }

  void IObjectBindingDataSource.SaveValues (bool interim)
  {
    DataSource.SaveValues (interim);
  }
}

}
