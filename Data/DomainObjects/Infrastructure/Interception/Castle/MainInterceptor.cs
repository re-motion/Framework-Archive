using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception.Castle
{
  /// <summary>Intercepts all methods and delegates to the respective specialized interceptor.</summary>
  // TODO: Instead of MainInterceptor use IInterceptorSelector as soon as implemented by DynamicProxy 2
	[Serializable]
  class MainInterceptor<TTarget> : global::Castle.Core.Interceptor.IInterceptor
  {
    private IInterceptorSelector<TTarget> _selector;

    public MainInterceptor (IInterceptorSelector<TTarget> selector)
    {
      _selector = selector;
    }

    public void Intercept (global::Castle.Core.Interceptor.IInvocation invocation)
    {
      _selector.SelectInterceptor (invocation.TargetType, invocation.Method).Intercept (new InvocationWrapper<TTarget> (invocation));
    }
  }
}
