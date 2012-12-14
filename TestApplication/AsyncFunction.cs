using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Rubicon.Collections;
using Rubicon.Web.ExecutionEngine;

namespace TestApplication
{
    public abstract class AsyncFunction : WxeFunction
    {
        private readonly Queue<Action> _continuationQueue = new Queue<Action>();
        private readonly Queue<Action> _reentryQueue = new Queue<Action>();
        private WxePageStep _pageStep;
        private bool _abort;
        private object _abortState;
        private Exception _exception;
        private ExecutionContext _executionContext;

        public AsyncFunction ()
        {
            SetContinuation(() =>
                            {
                                Task beginExecute = BeginExecute();
                                TaskStatus taskStatus = beginExecute.Status;
                            });
        }

        protected abstract Task BeginExecute ();

        public override sealed void Execute(WxeContext context)
        {
            if (!ExecutionStarted)
            {
                var parentVariables = (ParentStep != null) ? ParentStep.Variables : null;
                EnsureParametersInitialized(null);
            }

            var synchronizationContext = SynchronizationContext.Current;
            var method = synchronizationContext.GetType().GetMethod ("AllowVoidAsyncOperations", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke (synchronizationContext, null);

            var isPostBack = (0 == string.Compare (HttpContext.Current.Request.HttpMethod, "POST", false, CultureInfo.InvariantCulture));

            while (_reentryQueue.Count > 0 || _continuationQueue.Count > 0)
            {
                context.IsPostBack = isPostBack;
                Action continuation;
                if (_reentryQueue.Count > 0)
                    continuation = _reentryQueue.Dequeue();
                else
                    continuation = _continuationQueue.Dequeue();

                _executionContext = ExecutionContext.Capture();
                
                continuation();

                isPostBack = false;
                //if (_abort)
                //{
                //    _abort = false;
                //    Thread.CurrentThread.Abort(_abortState);
                //}
            }

            if (ParentStep != null)
                ReturnParametersToCaller();
        }

        public override WxeStep ExecutingStep
        {
            get { return (WxeStep)_pageStep ?? this; }
        }

        public PageStepAwaitable PageStep (string url)
        {
            return new PageStepAwaitable (url, this, () => _executionContext);
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

        public void SetAbort ( object state, Exception  ex)
        {
            _exception = ex;
            _abortState = state;
            _abort = true;
        }

        public void SetReentryAction (Action executePageStep)
        {
            _reentryQueue.Clear();
            _reentryQueue.Enqueue(executePageStep);
        }

        public void ResetReentryQueue()
        {
            _reentryQueue.Clear();
        }
    }
}