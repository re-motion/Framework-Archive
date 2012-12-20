using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;

namespace Remotion.Web.ExecutionEngine
{
  public class AsyncPageStep : WxePageStep
  {
    private readonly AsyncExecutionIterator _executionIterator;
    private readonly AsyncFunction _callingFunction;

    public AsyncPageStep (string page, AsyncExecutionIterator executionIterator, AsyncFunction callingFunction)
        : base(page)
    {
      _executionIterator = executionIterator;
      _callingFunction = callingFunction;
    }

    public AsyncPageStep (WxeVariableReference pageref, AsyncExecutionIterator executionIterator, AsyncFunction callingFunction)
        : base(pageref)
    {
      _executionIterator = executionIterator;
      _callingFunction = callingFunction;
    }

    protected AsyncPageStep (ResourceObjectBase page, AsyncExecutionIterator executionIterator, AsyncFunction callingFunction)
        : base(page)
    {
      _executionIterator = executionIterator;
      _callingFunction = callingFunction;
    }

    public AsyncExecutionIterator.Awaiter GetAwaiter()
    {
      return _executionIterator.CreateAwaiter(ExecuteAsync, null);
    }

    public void ExecuteAsync ()
    {
      ExecuteAsync (WxeContext.Current);
    }

    public void ExecuteAsync (WxeContext context)
    {
      try
      {
        _executionIterator.SetReentryAction(ExecuteAsync);
        _callingFunction.SetExecutingPageStep(this);

        Execute (context);

        _executionIterator.ResetReentryAction();
      }
      finally
      {
        _callingFunction.SetExecutingPageStep(null);
      }
    }
   
  }
}