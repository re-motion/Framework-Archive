using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  [Serializable]
  public abstract class AsyncFunction : WxeFunction
  {
    private AsyncPageStep _pageStep;
    private readonly AsyncExecutionIterator _executionIterator;

    protected AsyncFunction (ITransactionMode transactionMode, params object[] actualParameters)
        : base (transactionMode, actualParameters)
    {
      _executionIterator = new AsyncExecutionIterator (BeginExecute);
    }

    protected AsyncFunction (ITransactionMode transactionMode, WxeParameterDeclaration[] parameterDeclarations, object[] actualParameters)
        : base (transactionMode, parameterDeclarations, actualParameters)
    {
      _executionIterator = new AsyncExecutionIterator (BeginExecute);
    }

    protected abstract Task BeginExecute ();

    public AsyncExecutionIterator ExecutionIterator
    {
      get { return _executionIterator; }
    }

    public override sealed void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      Assertion.IsNotNull (ExecutionListener);

      if (!IsExecutionStarted)
      {
        VariablesContainer.EnsureParametersInitialized (null);
        ExecutionListener = new SecurityExecutionListener (this, ExecutionListener);

        TransactionStrategy = TransactionMode.CreateTransactionStrategy (this, context);
        Assertion.IsNotNull (TransactionStrategy);

        ExecutionListener = TransactionStrategy.CreateExecutionListener (ExecutionListener);
        Assertion.IsNotNull (ExecutionListener);
      }

      try
      {
        ExecutionListener.OnExecutionPlay (context);


        //var isPostBack = (0 == string.Compare (HttpContext.Current.Request.HttpMethod, "POST", false, CultureInfo.InvariantCulture));

        foreach (var continuation in _executionIterator)
        {
          //context.IsPostBack = isPostBack;

          WxeContext.SetCurrent (context);
          continuation();

          //isPostBack = false;
        }


        ExecutionListener.OnExecutionStop (context);
      }
      catch (WxeFatalExecutionException)
      {
        // bubble up
        throw;
      }
      catch (ThreadAbortException)
      {
        ExecutionListener.OnExecutionPause (context);
        throw;
      }
      catch (Exception stepException)
      {
        Exception unwrappedException = PageUtility.GetUnwrappedExceptionFromHttpException (stepException) ?? stepException;

        try
        {
          ExecutionListener.OnExecutionFail (context, unwrappedException);
        }
        catch (Exception listenerException)
        {
          throw new WxeFatalExecutionException (unwrappedException, listenerException);
        }

        if (!ExceptionHandler.Catch (unwrappedException))
        {
          if (unwrappedException is WxeUnhandledException)
            throw unwrappedException.PreserveStackTrace();

          throw new WxeUnhandledException (
              string.Format (
                  "An unhandled exception ocured while executing WxeFunction '{0}':\r\n{1}", GetType().FullName, unwrappedException.Message),
              unwrappedException);
        }
      }

      if (ExceptionHandler.Exception == null && ParentStep != null)
        VariablesContainer.ReturnParametersToCaller();
    }


    public override WxeStep ExecutingStep
    {
      get { return (WxeStep) _pageStep ?? this; }
    }

    public AsyncPageStep PageStep (string url)
    {
      return new AsyncPageStep (url, _executionIterator, this);
    }

    public void SetExecutingPageStep (AsyncPageStep pageStep)
    {
      _pageStep = pageStep;
    }
  }
}