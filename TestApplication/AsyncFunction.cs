using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using Rubicon.Web.ExecutionEngine;

namespace TestApplication
{
  public abstract class AsyncFunction : WxeFunction
  {
    private AsyncPageStep _pageStep;
    private readonly AsyncExecutionIterator _executionIterator;

    public AsyncFunction ()
    {
      _executionIterator = new AsyncExecutionIterator(BeginExecute);
    }

    protected abstract Task BeginExecute ();

    public AsyncExecutionIterator ExecutionIterator
    {
      get { return _executionIterator; }
    }

    public override sealed void Execute (WxeContext context)
    {
      if (!ExecutionStarted)
      {
        var parentVariables = (ParentStep != null) ? ParentStep.Variables : null;
        EnsureParametersInitialized (null);
      }

      var isPostBack = (0 == string.Compare (HttpContext.Current.Request.HttpMethod, "POST", false, CultureInfo.InvariantCulture));

      foreach (var continuation in _executionIterator)
      {
        context.IsPostBack = isPostBack;

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

    public AsyncPageStep PageStep (string url)
    {
      return new AsyncPageStep (url, this, _executionIterator);
    }

    public void SetExecutingPageStep (AsyncPageStep pageStep)
    {
      _pageStep = pageStep;
    }
  }
}