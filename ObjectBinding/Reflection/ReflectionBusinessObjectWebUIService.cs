using System;
using System.Web.UI.WebControls;
using Rubicon.ObjectBinding.Web;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectWebUIService: IBusinessObjectWebUIService
{ 
	public ReflectionBusinessObjectWebUIService()
	{
  }
  public IconInfo GetIcon (IBusinessObject obj)
  {
    return new IconInfo ("Images/RefelctionBusinessObjectIcon.gif", Unit.Pixel (16), Unit.Pixel (16));
  }

  public IconInfo GetNullValueIcon ()
  {
    return new IconInfo ("Images/RefelctionBusinessObjectIcon.gif", Unit.Pixel (16), Unit.Pixel (16));
  }
}

}
