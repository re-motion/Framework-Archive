using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure.DPInterception
{
  public interface IInterceptorSelector<TTarget>
  {
    bool ShouldInterceptMethod (Type type, MethodInfo memberInfo);
    IInterceptor<TTarget> SelectInterceptor (Type type, MethodInfo memberInfo);
  }
}
