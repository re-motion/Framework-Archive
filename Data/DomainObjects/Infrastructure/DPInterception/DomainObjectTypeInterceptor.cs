using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.DPInterception
{
  [Serializable]
  internal class DomainObjectTypeInterceptor : IInterceptor<DomainObject>
  {
    public readonly IInterceptorSelector<DomainObject> Selector;

    public DomainObjectTypeInterceptor ()
    {
      Selector = new DomainObjectTypeInterceptorSelector (this);
    }

    public void Intercept (IInvocation<DomainObject> invocation)
    {
      ArgumentUtility.CheckNotNull ("invocation", invocation);
      invocation.ReturnValue = invocation.TargetType;
    }
  }
}
