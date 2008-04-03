using System;
using Remotion.ObjectBinding.Web.UI.Design;

namespace Remotion.Data.DomainObjects.ObjectBinding.Web.Design
{
  public class DomainObjectDataSourceDesigner: BocDataSourceDesigner
  {
    public override string GetDesignTimeHtml()
    {
      DomainObjectDataSourceControl dataSourceControl = (DomainObjectDataSourceControl) Component;

      Exception designTimeException = dataSourceControl.GetDesignTimeException();
      if (designTimeException != null)
        return CreateErrorDesignTimeHtml (designTimeException.Message, designTimeException.InnerException);
      return CreatePlaceHolderDesignTimeHtml();
    }
  }
}