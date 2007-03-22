using System;
using System.Collections.Generic;
using System.Text;
using CastleInterceptor = Castle.Core.Interceptor.IInterceptor;
using CastleInvocation = Castle.Core.Interceptor.IInvocation;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception.Castle
{
  class InterceptorWrapper<TTarget> : CastleInterceptor
  {
    private IInterceptor<TTarget> _interceptor;

    public InterceptorWrapper (IInterceptor<TTarget> interceptor)
    {
      _interceptor = interceptor;
    }

    public void Intercept (CastleInvocation invocation)
    {
      _interceptor.Intercept (new InvocationWrapper<TTarget> (invocation));
    }
  }
}
