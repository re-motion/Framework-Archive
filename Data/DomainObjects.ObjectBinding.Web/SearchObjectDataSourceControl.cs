using System;
using System.ComponentModel;
using System.Web.UI;
using Remotion.Data.DomainObjects.ObjectBinding.Infrastructure;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.Data.DomainObjects.ObjectBinding.Web
{
public class SearchObjectDataSourceControl : BusinessObjectDataSourceControl, IForceLegacyReferenceMarker
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
