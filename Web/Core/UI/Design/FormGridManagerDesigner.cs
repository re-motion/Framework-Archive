using System;
using System.Web.UI.Design;

namespace Rubicon.Web.UI.Design
{

public class FormGridManagerDesigner: ControlDesigner
{
  public override string GetDesignTimeHtml()
  {
    return base.CreatePlaceHolderDesignTimeHtml();
  }
}

}
