using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

public abstract class BocItemCommand
{
  public abstract void RenderBegin (HtmlTextWriter writer, int index, string id);
  public abstract void RenderEnd (HtmlTextWriter writer);
}

[TypeConverter (typeof (BocHrefItemCommandConverter))]
public class BocHrefItemCommand: BocItemCommand
{
  private string _href;
  private string _target;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="href">{0}: index, {1}: ID</param>
  /// <param name="target"></param>
  public BocHrefItemCommand (string href, string target)
  {
    _href = href;
    _target = target;
  }

  public BocHrefItemCommand()
    : this (".aspx?{0}", null)
  {}

  [Description ("The hyperlink reference of the command.")]
  [DefaultValue(".aspx?{0}")]
  public string Href 
  {
    get { return _href; }
    set { _href = value; }
  }

  [Description ("The target frame of the command. Leave it blank for no target.")]
  [DefaultValue("")]
  public string Target 
  { 
    get { return _target; }
    set { _target = value; }
  }

  public override void RenderBegin (HtmlTextWriter writer, int index, string id)
  {
    string href = string.Format (_href, index, id);
    writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
    if (_target != null) 
      writer.AddAttribute (HtmlTextWriterAttribute.Target, _target);
    writer.RenderBeginTag (HtmlTextWriterTag.A);    
  }

  public override void RenderEnd (HtmlTextWriter writer)
  {
    writer.RenderEndTag();
  }

  public override string ToString()
  {
    if (StringUtility.IsNullOrEmpty (Href))
      return string.Empty;
    else if (StringUtility.IsNullOrEmpty (Target))
      return Href;
    else
      return string.Format ("{0}, {1}", Href, Target);
  }

}

}
