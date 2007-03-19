using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace Rubicon.Data.DomainObjects.Interception
{
  public class DomainObjectTypeInterceptor : IInterceptor<DomainObject>
  {
    class InterceptorSelector : IInterceptorSelector<DomainObject>
    {
      private DomainObjectTypeInterceptor _interceptor;

      public InterceptorSelector (DomainObjectTypeInterceptor interceptor)
      {
        _interceptor = interceptor;
      }

      public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
      {
        return memberInfo.Name == "GetPublicDomainObjectType";
      }

      public IInterceptor<DomainObject> SelectInterceptor (Type type, MethodInfo memberInfo)
      {
        Debug.Assert (ShouldInterceptMethod (type, memberInfo));
        return _interceptor;
      }
    }

    public readonly IInterceptorSelector<DomainObject> Selector;

    public DomainObjectTypeInterceptor ()
    {
      Selector = new InterceptorSelector (this);
    }

    public void Intercept (IInvocation<DomainObject> invocation)
    {
      invocation.ReturnValue = invocation.TargetType;
    }
  }
}
