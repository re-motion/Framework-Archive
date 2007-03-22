using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  public interface IInvocation<TTarget>
  {
    TTarget This { get; }
    TTarget InvocationTarget { get; }

    Type TargetType { get; }
    MethodInfo Method { get; }

    object[] Arguments { get; }
    object ReturnValue { get; set; }

    void Proceed ();
  }
}
