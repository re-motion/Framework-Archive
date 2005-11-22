using System;
using System.Web.UI.Design;
using Rubicon.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.Design
{

public class BocDataSourceDesigner: WebControlDesigner
{
  public override string GetDesignTimeHtml()
  {
    return base.CreatePlaceHolderDesignTimeHtml();
  }
}

}
