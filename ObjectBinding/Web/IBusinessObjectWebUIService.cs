using System;
using System.Drawing;
using Rubicon.ObjectBinding;
using System.Web.UI.WebControls;

namespace Rubicon.ObjectBinding.Web
{

/// <summary>
///   Provides services for business object bound web applications
/// </summary>
public interface IBusinessObjectWebUIService: IBusinessObjectService
{
  IconInfo GetIcon (IBusinessObject obj);
  IconInfo GetNullValueIcon ();
}

public sealed class IconInfo
{
  private string _url;
  private Unit _width;
  private Unit _height;

  public IconInfo (string url, Unit width, Unit height)
  {
    _url = url;
    _width = width;
    _height = height;
  }

  public string Url
  {
    get { return _url; }
    set { _url = value; }
  }

  public Unit Width
  {
    get { return _width; }
    set { _width = value; }
  }

  public Unit Height
  {
    get { return _height; }
    set { _height = value; }
  }
}

}
