using System;
using System.Web.UI.Design;

using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Design
{

public class DomainObjectDataSourceDesigner: BocDataSourceDesigner
{
  public override string GetDesignTimeHtml()
  {
    DomainObjectDataSourceControl dataSourceControl = (DomainObjectDataSourceControl) Component;
    
    ApplicationException designTimeException = dataSourceControl.GetDesignTimeException();
    if (designTimeException != null)
      return CreateErrorDesignTimeHtml (designTimeException.Message, designTimeException.InnerException, Component);

    return CreatePlaceHolderDesignTimeHtml();
  }
}

}
