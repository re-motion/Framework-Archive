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
      context.IsReturningPostBack = true;
    }
    else
    {
      context.PostBackCollection = null;
      context.IsReturningPostBack = false;
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
    InternalExecuteFunction (page, function);
  }

  public void ExecuteFunction (IWxePage page, WxeFunction function, Control control)
  {
    throw new NotImplementedException("ExecuteFunction (IWxePage page, WxeFunction function, Control control)");
  }

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page without causing the current event again.
  /// </summary>
  /// <remarks>
  ///   This overload assumes that the current event was caused by the __EVENTTARGET field. 
  ///   When in doubt, use <see cref="ExecuteFunctionNoRepost (IWxePage, WxeFunction, Control)"/>.
  /// </remarks>
  public void ExecuteFunctionNoRepost (IWxePage page, WxeFunction function)
  {
    ExecuteFunctionNoRepost (page, function, null, true);
  }

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page without causing the current event again.
  /// </summary>
  /// <remarks>
  ///   This overload tries to determine automatically whether the current event was caused by the __EVENTTARGET field.
  /// </remarks>
  public void ExecuteFunctionNoRepost (IWxePage page, WxeFunction function, Control sender)
  {
    bool usesEventTarget = true;
    foreach (string key in page.GetPostBackCollection().Keys)
    {
      // if a control's ID is in the forms collection and the control implements IPostBackEventHandler, but not IPostBackDataHandler, it
      // is assumed that the event was caused by the control's ID in the forms collection, not the __EVENTTARGET field. 
      // see also: System.Web.UI.Page.ProcessPostData
      if (key == sender.UniqueID && (sender is IPostBackEventHandler) && ! (sender is IPostBackDataHandler))
      {
        usesEventTarget = false;
        break;
      }
    }

    ExecuteFunctionNoRepost (page, function, sender, usesEventTarget);
  }

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page without causing the current event again.
  /// </summary>
  /// <remarks>
  ///   This overload allows you to specify whether the current event was caused by the __EVENTTARGET field.
  ///   When in doubt, use <see cref="ExecuteFunctionNoRepost (IWxePage, WxeFunction, Control)"/>.
  /// </remarks>
  public void ExecuteFunctionNoRepost (IWxePage page, WxeFunction function, Control sender, bool usesEventTarget)
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

    InternalExecuteFunction (page, function);
  }

  public void InternalExecuteFunction (IWxePage page, WxeFunction function)
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
