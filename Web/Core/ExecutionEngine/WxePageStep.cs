using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

public class WxeExecuteNextStepException: Exception
{
}

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
      context.ReturningFunction = _function;
      _function = null;
      context.IsPostBack = true;
      context.PostBackCollection = _postBackCollection;
      _postBackCollection = null;
      context.IsReturningPostBack = true;
    }
    else
    {
      context.PostBackCollection = null;
      context.IsReturningPostBack = false;
    }

    try 
    {
      context.HttpContext.Server.Transfer (_page, context.IsPostBack);
    }
    catch (HttpException e)
    {
      if (e.InnerException is WxeExecuteNextStepException)
        return;
      if (e.InnerException is HttpUnhandledException && e.InnerException.InnerException is WxeExecuteNextStepException)
        return;
      throw;
    }
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

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page.
  /// </summary>
  /// <remarks>
  ///   Note that if you call this method from a postback event handler, the postback event will be raised again when the user
  ///   returns to this page. You can either manually check whether the event was re-posted using 
  ///   <see cref="WxeContext.IsReturningPostBack"/> or suppress the re-post by calling <see cref="ExecuteFunctionNoRepost"/>.
  /// </remarks>
  public void ExecuteFunction (IWxePage page, WxeFunction function)
  {
    _postBackCollection = new NameValueCollection (page.GetPostBackCollection());
    InternalExecuteFunction (function);
  }

  internal void ExecuteFunctionNoRepost (IWxePage page, WxeFunction function, Control sender, bool usesEventTarget)
  {
    _postBackCollection = new NameValueCollection (page.GetPostBackCollection());

    if (usesEventTarget)
    {
      _postBackCollection.Remove ("__EVENTTARGET");
      _postBackCollection.Remove ("__EVENTARGUMENT");
    }
    else
    {
      ArgumentUtility.CheckNotNull ("sender", sender);
      _postBackCollection.Remove (sender.UniqueID);
    }
    InternalExecuteFunction (function);
  }

  private void InternalExecuteFunction (WxeFunction function)
  {
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
