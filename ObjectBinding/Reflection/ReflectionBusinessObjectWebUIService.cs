using System;
using System.Web.UI.WebControls;
using Rubicon.ObjectBinding.Web;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectWebUIService: IBusinessObjectWebUIService
{ 
	public ReflectionBusinessObjectWebUIService()
	{
  }
  public IconInfo GetIcon (IBusinessObject obj)
  {
    return new IconInfo ("Images/" + obj.BusinessObjectClass.Identifier + ".gif", Unit.Pixel (16), Unit.Pixel (16));
  }

  public IconInfo GetNullValueIcon ()
  {
    return new IconInfo ("Images/NullIcon.gif", Unit.Pixel (16), Unit.Pixel (16));
  }
}

}
