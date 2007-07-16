using System;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.UI.Design
{
  public class BindableObjectDataSourceDesigner: BocDataSourceDesigner
  {
    public override string GetDesignTimeHtml()
    {
      BindableObjectDataSourceControl dataSourceControl = (BindableObjectDataSourceControl) Component;

      //Exception designTimeException = dataSourceControl.GetDesignTimeException();
      //if (designTimeException != null)
      //  return CreateErrorDesignTimeHtml (designTimeException.Message, designTimeException.InnerException);
      return CreatePlaceHolderDesignTimeHtml();
    }
  }
}