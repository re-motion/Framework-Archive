using System;
using System.Web;
using System.Web.UI;

namespace Rubicon.ObjectBinding.Web.Controls
{

public abstract class ItemCommand
{
  private object _label;
  private string _iconPath;

  public ItemCommand (object label)
  {
    _label = label;
    _iconPath = null;
  }

  public ItemCommand (string iconPath)
  {
    _iconPath = iconPath;
    _label = null;
  }

  public string Label
  {
    get { return _label.ToString(); }
  }

  public string IconPath 
  {
    get { return _iconPath; }
  }

  protected void RenderLabel (HtmlTextWriter writer)
  {
    if (_label != null)
    {
      HttpUtility.HtmlEncode (Label, writer);
    }
    else
    {
      writer.WriteFullBeginTag ("img");
      writer.WriteAttribute ("href", _iconPath);
      writer.Write (HtmlTextWriter.TagRightChar);
    }
  }

  public abstract void RenderBegin (HtmlTextWriter writer);
  public abstract void RenderEnd (HtmlTextWriter writer);
}

public class HrefItemCommand: ItemCommand
{
  private string _href;
  private string _target;

  public HrefItemCommand (object label, string href, string target)
    : base (label)
  {
    _href = href;
    _target = target;
  }

  public string Href 
  {
    get { return _href; }
  }

  public string Target 
  { 
    get { return _target; }
  }

  public override void RenderBegin (HtmlTextWriter writer)
  {
    writer.WriteBeginTag ("a");                             // <a
    writer.WriteAttribute ("href", _href);                  //    href=
    if (_target != null) 
      writer.WriteAttribute ("target", _target);            //    target=
    writer.Write (HtmlTextWriter.TagRightChar);             // >

    RenderLabel (writer);                                   // label/icon
  }

  public override void RenderEnd (HtmlTextWriter writer)
  {
    writer.WriteEndTag ("a");                               // </a>
  }

}

}
