using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Rubicon.Web.ExecutionEngine;

namespace TestApplication
{
  public abstract class AsyncFunction : WxeFunction
  {
    private readonly Queue<Action> _continuationQueue = new Queue<Action>();
    private readonly Queue<Action> _reentryQueue = new Queue<Action>();
    private WxePageStep _pageStep;

    public AsyncFunction ()
    {
      SetContinuation (() => BeginExecute());
    }

    protected abstract Task BeginExecute ();

    public override sealed void Execute (WxeContext context)
    {
      if (!ExecutionStarted)
      {
        var parentVariables = (ParentStep != null) ? ParentStep.Variables : null;
        EnsureParametersInitialized (null);
      }

      var synchronizationContext = SynchronizationContext.Current;
      var method = synchronizationContext.GetType().GetMethod ("AllowVoidAsyncOperations", BindingFlags.Instance | BindingFlags.NonPublic);
      method.Invoke (synchronizationContext, null);

      var isPostBack = (0 == string.Compare (HttpContext.Current.Request.HttpMethod, "POST", false, CultureInfo.InvariantCulture));

      while (_reentryQueue.Count > 0 || _continuationQueue.Count > 0)
      {
        context.IsPostBack = isPostBack;
        var continuation = _reentryQueue.Count > 0 ? _reentryQueue.Dequeue() : _continuationQueue.Dequeue();

        continuation();

        isPostBack = false;
      }

      if (ParentStep != null)
        ReturnParametersToCaller();
    }

    public override WxeStep ExecutingStep
    {
      get { return (WxeStep) _pageStep ?? this; }
    }

    public PageStepAwaitable PageStep (string url)
    {
      return new PageStepAwaitable (url, this);
    }

    public void SetExecutingPageStep (WxePageStep pageStep)
    {
      _pageStep = pageStep;
    }

    public void SetContinuation (Action continuation)
    {
      _continuationQueue.Clear();
      _continuationQueue.Enqueue (continuation);
    }

    public void SetReentryAction (Action executePageStep)
    {
      _reentryQueue.Clear();
      _reentryQueue.Enqueue (executePageStep);
    }

    public void ResetReentryQueue ()
    {
      _reentryQueue.Clear();
    }
  }
}