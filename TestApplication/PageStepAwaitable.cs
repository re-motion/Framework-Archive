using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using Rubicon.Web.ExecutionEngine;

namespace TestApplication
{
    public class PageStepAwaitable : INotifyCompletion
    { 
        private readonly string _url;
        private readonly AsyncFunction _callingFunction;
        private readonly Func<ExecutionContext> _executionContextAccessor;

        public PageStepAwaitable (string url, AsyncFunction callingFunction, Func<ExecutionContext> executionContextAccessor)
        {
            _url = url;
            _callingFunction = callingFunction;
            _executionContextAccessor = executionContextAccessor;
        }

        public PageStepAwaitable GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted
        {
            get { return false; }
        }

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
        {
            var pageStepAction = new Action(ExecutePageStep);

            _callingFunction.SetReentryAction(pageStepAction);
            _callingFunction.SetContinuation (continuation);
        }

        private void ExecutePageStep ()
        {
            var wxePageStep = new WxePageStep (_url);

            try
            {

                _callingFunction.SetReentryAction(ExecutePageStep);

                _callingFunction.SetExecutingPageStep (wxePageStep);
                var executionContext = _executionContextAccessor();
                wxePageStep.Execute();

                //pagestep complete
                //executeNextStep exception thrown

                //continue with original continuation
                _callingFunction.ResetReentryQueue();

                //ExecutionContext.Run (executionContext, state => wxePageStep.Execute(), null);
            }
                    //catch (ThreadAbortException ex)
                    //{
                    //    var state = ex.ExceptionState;
                    //    Thread.ResetAbort();
                    //    _callingFunction.SetAbort(state, ex);
                    //    return;
                    //}
            finally
            {
                _callingFunction.SetExecutingPageStep (null);
            }
        }
    }
}