using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  public class DomainObjectTypeInterceptor : IInterceptor<DomainObject>
  {
    public readonly IInterceptorSelector<DomainObject> Selector;

    public DomainObjectTypeInterceptor ()
    {
      Selector = new DomainObjectTypeInterceptorSelector (this);
    }

    public void Intercept (IInvocation<DomainObject> invocation)
    {
      invocation.ReturnValue = invocation.TargetType;
    }
  }
}
