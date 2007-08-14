using System;
using System.Collections.Generic;
using System.Text;
using CastleInvocation = Castle.Core.Interceptor.IInvocation;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure.DPInterception.Castle
{
  class InvocationWrapper<T> : IInvocation<T>
  {
    private CastleInvocation _invocation;

    public InvocationWrapper (CastleInvocation invocation)
    {
      _invocation = invocation;
    }

    public T This
    {
      get { return (T) _invocation.Proxy; }
    }

    public T InvocationTarget
    {
      get { return (T) _invocation.InvocationTarget; }
    }

    public Type TargetType
    {
      get { return _invocation.TargetType; }
    }

    public MethodInfo Method
    {
      get { return _invocation.Method; }
    }

    public object[] Arguments
    {
      get { return _invocation.Arguments; }
    }

    public object ReturnValue
    {
      get { return _invocation.ReturnValue; }
      set { _invocation.ReturnValue = value; }
    }

    public void Proceed ()
    {
      _invocation.Proceed ();
    }
  }
}
