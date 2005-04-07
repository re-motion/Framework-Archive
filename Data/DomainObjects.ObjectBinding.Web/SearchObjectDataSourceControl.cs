using System;
using System.ComponentModel;
using System.Web.UI;

using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web
{
public class SearchObjectDataSourceControl : BusinessObjectDataSourceControl
{
  private SearchObjectDataSource _dataSource = new SearchObjectDataSource ();

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [DefaultValue("")]
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
}
}
