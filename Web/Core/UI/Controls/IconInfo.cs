using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rubicon.Web.UI.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
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

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  public string Url
  {
    get { return _url; }
    set { _url = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue (typeof (Unit), "")]
  public Unit Width
  {
    get { return _width; }
    set { _width = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue (typeof (Unit), "")]
  public Unit Height
  {
    get { return _height; }
    set { _height = value; }
  }

  public override string ToString()
  {
    return _url;
  }

}

}
