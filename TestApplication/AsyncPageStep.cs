using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using Rubicon.Web.ExecutionEngine;

namespace TestApplication
{
  public class AsyncPageStep : WxeStep, IWxePageStep
  {
    private readonly string _url;
    private readonly AsyncFunction _callingFunction;
    private readonly AsyncExecutionIterator _executionIterator;
    private WxeFunction _function;
    private NameValueCollection _postBackCollection;

    public AsyncPageStep (string url, AsyncFunction callingFunction, AsyncExecutionIterator executionIterator)
    {
      _url = url;
      PageToken = Guid.NewGuid().ToString();
      _callingFunction = callingFunction;
      _executionIterator = executionIterator;
    }

    public AsyncExecutionIterator.Awaiter GetAwaiter()
    {
      return _executionIterator.CreateAwaiter(Execute, null);
    }

    public override void Execute (WxeContext context)
    {
      try
      {
        _executionIterator.SetReentryAction(Execute);

        _callingFunction.SetExecutingPageStep(this);

        ExecuteInternal (context);

        //pagestep complete
        //executeNextStep exception thrown

        //continue with original continuation
        _executionIterator.ResetReentryAction();
      }
      finally
      {
        _callingFunction.SetExecutingPageStep(null);
      }
    }

    private void ExecuteInternal(WxeContext context)
    {
      var httpContext = HttpContext.Current;

      if (_function != null)
      {
        _function.Execute(context);
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
        context.HttpContext.Server.Transfer(_url, context.IsPostBack);
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

    public void ExecuteFunction (IWxePage page, WxeFunction function)
    {
      if (!(function is AsyncFunction))
        throw new InvalidOperationException ("nur async functions erlaubt.");

      _postBackCollection = new NameValueCollection(page.GetPostBackCollection());
      InternalExecuteFunction(function);
    }

    private void InternalExecuteFunction(WxeFunction function)
    {
      if (_function != null)
        throw new InvalidOperationException("Cannot execute function while another function executes.");

      _function = function;
      _function.ParentStep = this;

      Execute();
    }

    public void ExecuteFunctionNoRepost (IWxePage page, WxeFunction function, Control sender, bool usesEventTarget)
    {
      throw new NotImplementedException();
    }

    public string PageToken { get; private set; }
  }
}