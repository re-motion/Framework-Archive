using System;
using System.Web;
using System.Web.UI.WebControls;
using Rubicon.ObjectBinding.Web;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectWebUIService: IBusinessObjectWebUIService
{ 
  public IconInfo GetIcon (IBusinessObject obj)
  {
    if (obj == null)
    {
      string url = "~/Images/NullIcon.gif";
      return new IconInfo (url, Unit.Pixel (16), Unit.Pixel (16));
    }
    else
    {
      string url = "~/Images/" + obj.BusinessObjectClass.Identifier + ".gif";
      return new IconInfo (url, Unit.Pixel (16), Unit.Pixel (16));
    }
  }

  public string GetToolTip (IBusinessObject obj)
  {
    if (obj == null)
      return "No ToolTip";
    else
      return "ToolTip: " + obj.BusinessObjectClass.Identifier;
  }

}

}
