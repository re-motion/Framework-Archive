using System;
using System.Drawing;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Web
{
/// <summary>
///   Provides services for business object bound web applications
/// </summary>
public interface IBusinessObjectWebUIService: IBusinessObjectService
{
  IconPrototype GetIcon (IBusinessObjectWithIdentity obj);
}

public sealed class IconPrototype
{
  private string _url;

  private Size _size;

  public IconPrototype (string url, Size size)
  {
    _url = url;
    _size = size;
  }

  public string Url
  {
    get { return _url; }
    set { _url = value; }
  }

  public Size Size
  {
    get { return _size; }
    set { _size = value; }
  }
}

}
