using System;
using System.Web;
using System.Web.UI;

namespace Rubicon.ObjectBinding.Web.Controls
{

public abstract class BocItemCommand
{
  public abstract void RenderBegin (HtmlTextWriter writer, int index, string id);
  public abstract void RenderEnd (HtmlTextWriter writer);
}

public class BocHrefItemCommand: BocItemCommand
{
  private string _href;
  private string _target;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="label"></param>
  /// <param name="href">{0} index, {1} ID</param>
  /// <param name="target"></param>
  public BocHrefItemCommand (string href, string target)
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

}

public class BocEmptyItemCommand : BocItemCommand
{
  public BocEmptyItemCommand()
  {}

  public override void RenderBegin (HtmlTextWriter writer, int index, string id)
  {}

  public override void RenderEnd (HtmlTextWriter writer)
  {}
}

}
