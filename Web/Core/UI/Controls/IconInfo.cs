using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
[Serializable]
public sealed class IconInfo: ISerializable
{
  private string _url;
  private string _alternateText;
  private string _toolTip;
  private Unit _width;
  private Unit _height;

  public IconInfo (string url, string alternateText, string toolTip, Unit width, Unit height)
  {
    Url = url;
    AlternateText = alternateText;
    ToolTip = toolTip;
    _width = width;
    _height = height;
  }

  public IconInfo (string url, Unit width, Unit height)
    : this (url, null, null, width, height)
  {
  }

  public IconInfo (string url, string alternateText, string toolTip, string width, string height)
    : this (url, null, toolTip, new Unit (width), new Unit (height))
  {
  }

  public IconInfo (string url, string width, string height)
    : this (url, null, null, width, height)
  {
  }

  public IconInfo (string url)
    : this (url, null, null, Unit.Empty, Unit.Empty)
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
    set { _url = StringUtility.NullToEmpty (value); }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  [NotifyParentProperty (true)]
  public string AlternateText
  {
    get { return _alternateText; }
    set { _alternateText = StringUtility.NullToEmpty (value); }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  [NotifyParentProperty (true)]
  public string ToolTip
  {
    get { return  _toolTip; }
    set { _toolTip = StringUtility.NullToEmpty (value); }
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

  public void Render (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);

    writer.AddAttribute (HtmlTextWriterAttribute.Src, _url);
    if (! _width.IsEmpty && ! _height.IsEmpty)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Width, _width.ToString());
      writer.AddAttribute (HtmlTextWriterAttribute.Height, _height.ToString());
    }
    writer.AddStyleAttribute ("vertical-align", "middle");
    writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
    if (StringUtility.IsNullOrEmpty (_alternateText))
      writer.AddAttribute (HtmlTextWriterAttribute.Alt, string.Empty);
    else 
      writer.AddAttribute (HtmlTextWriterAttribute.Alt, _alternateText);
    if (! StringUtility.IsNullOrEmpty (_toolTip))
      writer.AddAttribute (HtmlTextWriterAttribute.Title, _toolTip);
    writer.RenderBeginTag (HtmlTextWriterTag.Img);
    writer.RenderEndTag();
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

  public static bool ShouldSerialize (IconInfo icon)
  {
    if (icon == null)
    {
      return false;
    }
    else if (   StringUtility.IsNullOrEmpty (icon.Url)
             && icon.Height.IsEmpty
             && icon.Width.IsEmpty)
    {
      return false;
    }
    else
    {
      return true;
    }
  }

  public void Reset()
  {
    _url = string.Empty;
    _alternateText = string.Empty;
    _width = Unit.Empty;
    _height = Unit.Empty;
  }

  public void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;

    string key;
    key = ResourceManagerUtility.GetGlobalResourceKey (Url);
    if (! StringUtility.IsNullOrEmpty (key))
      Url = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (AlternateText);
    if (! StringUtility.IsNullOrEmpty (key))
      AlternateText = resourceManager.GetString (key);
  }
}

}
