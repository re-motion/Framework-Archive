using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using System.Reflection;

namespace Rubicon.Web.ExecutionEngine
{

public class WxeExecuteNextStepException: Exception
{
  public WxeExecuteNextStepException()
    : base ("This exception does not indicate an error. It is used to roll back the call stack. It is recommended to disable breaking on this exeption type while debugging.")
  {
  }
}

public class WxeUserCancelException: Exception
{
  public WxeUserCancelException()
    : this ("User cancelled this step.")
  {
  }
  public WxeUserCancelException(string message)
    : base (message)
  {
  }
  public WxeUserCancelException(string message, Exception innerException)
    : base (message, innerException)
  {
  }
}

public class WxePageStep: WxeStep
{
  private string _page = null;
  private string _pageref = null;
  private Assembly _resourceAssembly = null;
  private string _resourceAssemblyName = null;
  private string _pageToken;
  private WxeFunction _function;
  private NameValueCollection _postBackCollection;

  public WxePageStep (string page)
  {
    _page = page;
    init();
  }

  protected WxePageStep (Assembly resourceAssembly, string page)
  {
    _resourceAssembly = resourceAssembly;
    _page = page;
    init();
  }
  public WxePageStep (WxeVariableReference page)
  {
    _pageref = page.Name;
    init();
  }
  protected WxePageStep (Assembly resourceAssembly, WxeVariableReference page)
  {
    _resourceAssembly = resourceAssembly;
    _pageref = page.Name;
    init();
  }

  private void init()
  {
    _pageToken = Guid.NewGuid().ToString();
    _function = null;
    if (_resourceAssembly != null)
    {
       _resourceAssemblyName = _resourceAssembly.FullName;
      int comma = _resourceAssemblyName.IndexOf (',');
      if (comma >= 0)
        _resourceAssemblyName = _resourceAssemblyName.Substring (0, comma);
    }
  }

  protected string Page
  { 
    get
    {
      string name;
      if (_page != null)
        name = _page;
      else if (_pageref != null && Variables[_pageref] != null)
        name = (string) Variables[_pageref];
      else
        throw new ApplicationException ("No Page specified for " + this.GetType().FullName + ".");
      if (_resourceAssemblyName != null)
        name = "res/" + _resourceAssemblyName + "/" + name;
      return name;
    }
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
      context.HttpContext.Server.Transfer (Page, context.IsPostBack);
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
      _postBackCollection.Remove (ControlHelper.EventTarget);
      _postBackCollection.Remove (ControlHelper.EventArgument);
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

  public string PageToken
  {
    get { return _pageToken; }
  }

  public override string ToString()
  {
    return Page;
  }
}

}
