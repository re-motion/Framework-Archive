using System;

using Rubicon.ObjectBinding.Web;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{
public class EditableRowDataSourceFactory
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public EditableRowDataSourceFactory ()
  {
  }

  // methods and properties

  public virtual IBusinessObjectReferenceDataSource Create (IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);

    BusinessObjectReferenceDataSource dataSource = new BusinessObjectReferenceDataSource();
    dataSource.BusinessObject = businessObject;
    
    return dataSource;
  }
}

}
