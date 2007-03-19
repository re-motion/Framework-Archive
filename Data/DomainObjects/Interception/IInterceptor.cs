using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.Interception
{
  public interface IInterceptor<TTarget>
  {
    void Intercept (IInvocation<TTarget> invocation);
  }
}
