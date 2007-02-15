using System;
using Rubicon.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.UI.Design
{

public class BocDataSourceDesigner: WebControlDesigner
{
  public override string GetDesignTimeHtml()
  {
    return base.CreatePlaceHolderDesignTimeHtml();
  }
}

}
