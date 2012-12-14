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
    public class Awaiter : INotifyCompletion
    {
      private readonly AsyncPageStep _parentPageStep;
      private readonly AsyncFunction _callingFunction;

      public Awaiter (AsyncPageStep parentPageStep, AsyncFunction callingFunction)
      {
        _parentPageStep = parentPageStep;
        _callingFunction = callingFunction;
      }

      public bool IsCompleted
      {
        get { return false; }
      }

      public void GetResult ()
      {
      }

      public void OnCompleted (Action continuation)
      {
        var pageStepAction = new Action (_parentPageStep.Execute);

        _callingFunction.SetReentryAction (pageStepAction);
        _callingFunction.SetContinuation (continuation);
      }
    }

    private readonly string _url;
    private readonly AsyncFunction _callingFunction;
    private WxeFunction _function;
    private NameValueCollection _postBackCollection;

    public AsyncPageStep (string url, AsyncFunction callingFunction)
    {
      _url = url;
      PageToken = Guid.NewGuid().ToString();
      _callingFunction = callingFunction;
    }

    public Awaiter GetAwaiter()
    {
      return new Awaiter(this, _callingFunction);
    }

    public override void Execute (WxeContext context)
    {
      try
      {
        _callingFunction.SetReentryAction(Execute);

        _callingFunction.SetExecutingPageStep(this);

        ExecuteInternal (context);

        //pagestep complete
        //executeNextStep exception thrown

        //continue with original continuation
        _callingFunction.ResetReentryQueue();
      }
      finally
      {
        _callingFunction.SetExecutingPageStep(null);
      }
    }

    private void ExecuteInternal(WxeContext context)
    {
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