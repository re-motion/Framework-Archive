using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Interception.Castle
{
  class GenerationHook<TTarget> : IProxyGenerationHook
  {
    private IInterceptorSelector<TTarget> _selector;

    public GenerationHook (IInterceptorSelector<TTarget> selector)
    {
      _selector = selector;
    }

    public void MethodsInspected ()
    {
      // nothing to do
    }

    public void NonVirtualMemberNotification (Type type, MemberInfo memberInfo)
    {
      // nothing to do
    }

    public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
    {
      return _selector.ShouldInterceptMethod (type, memberInfo);
    }
  }
}
