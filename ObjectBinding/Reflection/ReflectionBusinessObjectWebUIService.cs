using System;
using System.Web.UI.WebControls;
using Rubicon.ObjectBinding.Web;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectWebUIService: IBusinessObjectWebUIService
{ 
  public IconInfo GetIcon (IBusinessObject obj)
  {
    if (obj == null)
      return new IconInfo ("Images/NullIcon.gif", Unit.Pixel (16), Unit.Pixel (16));
    else
      return new IconInfo ("Images/" + obj.BusinessObjectClass.Identifier + ".gif", Unit.Pixel (16), Unit.Pixel (16));
  }
}

}
