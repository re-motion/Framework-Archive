using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;

namespace Rubicon.Web.UI.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
[Serializable]
public sealed class IconInfo: ISerializable
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

  public IconInfo (string url, string width, string height)
    : this (url, new Unit (width), new Unit (height))
  {
  }

  public IconInfo (string url)
    : this (url, Unit.Empty, Unit.Empty)
  {
  }

  public IconInfo ()
    : this (string.Empty)
  {
  }

  //[Editor(typeof(ImageUrlEditor), typeof(UITypeEditor))]
  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  [NotifyParentProperty (true)]
  public string Url
  {
    get { return _url; }
    set { _url = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue (typeof (Unit), "")]
  [NotifyParentProperty (true)]
  public Unit Width
  {
    get { return _width; }
    set { _width = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue (typeof (Unit), "")]
  [NotifyParentProperty (true)]
  public Unit Height
  {
    get { return _height; }
    set { _height = value; }
  }

  public override string ToString()
  {
    return _url;
  }

  private IconInfo (SerializationInfo info, StreamingContext context)
  {
    string width = (string) info.GetValue ("_width", typeof (string));
    string height = (string) info.GetValue ("_height", typeof (string));

    _url = (string) info.GetValue ("_url", typeof (string));
    _width = new Unit (width);
    _height = new Unit (height);
  }

  void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("_url", _url);
    info.AddValue ("_width", _width.ToString());
    info.AddValue ("_height", _height.ToString());
  }
}

}
