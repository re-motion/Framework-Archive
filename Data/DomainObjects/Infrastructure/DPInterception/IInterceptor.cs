using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.Infrastructure.DPInterception
{
  public interface IInterceptor<TTarget>
  {
    void Intercept (IInvocation<TTarget> invocation);
  }
}
