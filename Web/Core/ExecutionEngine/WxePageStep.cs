using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

public class WxePageStep: WxeStep
{
  private string _page;
  private WxeFunction _function;
  private NameValueCollection _postBackCollection;

  public WxePageStep (string page)
  {
    _page = page;
    _function = null;
  }

  public override void Execute (WxeContext context)
  {
    if (_function != null)
    {
      _function.Execute (context);
      _function = null;
      context.IsPostBack = true;
      context.PostBackCollection = _postBackCollection;
      _postBackCollection = null;
    }
    else
    {
      context.PostBackCollection = null;
    }

    context.HttpContext.Server.Transfer (_page, context.IsPostBack);
  }

  public override void ExecuteNextStep (WxeContext context)
  {
    if (_function != null)
    {
      _function.ExecuteNextStep (context);
      _function = null;
      context.IsPostBack = false;
    }

    base.ExecuteNextStep (context);
  }

  public override WxeStep ExecutingStep
  {
    get
    {
      if (_function != null)
        return _function.ExecutingStep;
      else
        return this;
    }
  }

  public void ExecuteFunction (object sender, IWxePage page, WxeFunction function)
  {
    _postBackCollection = new NameValueCollection (page.GetPostBackCollection());
    if (sender is Control)
      _postBackCollection.Remove (((Control)sender).ClientID);

    //_postBackCollection.Remove ("Sub");
    if (_function != null)
      throw new InvalidOperationException ("Cannot execute function while another function executes.");

    _function = function; 
    _function.ParentStep = this;

    Execute();
  }

  public override string ToString()
  {
    return _page;
  }

}

}
