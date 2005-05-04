using System;
using System.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.Design
{

public class BocDataSourceDesigner: BocDesigner
{
  public override string GetDesignTimeHtml()
  {
    return base.CreatePlaceHolderDesignTimeHtml();
  }
}

}
