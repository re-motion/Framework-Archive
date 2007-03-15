using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Interceptor;
using System.Diagnostics;
using System.Reflection;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Handles property accessor calls of domain objects and prepares the properties accordingly.
  /// </summary>
  class PropertyInterceptor : IInterceptor
  {
    public void Intercept (IInvocation invocation)
    {
      DomainObject target = invocation.InvocationTarget as DomainObject;
      Debug.Assert (ReflectionUtility.IsPropertyAccessor (invocation.Method));
      Debug.Assert (target != null);

      string propertyName = ReflectionUtility.GetPropertyNameForMethodName (invocation.Method.Name);
      
      target.PreparePropertyAccess (propertyName);
      try
      {
        if (invocation.Method.IsAbstract)
        {
          if (ReflectionUtility.IsPropertyGetter (invocation.Method))
          {
            DefaultGetterImplementation (target, invocation);
          }
          else
          {
            Debug.Assert (ReflectionUtility.IsPropertySetter (invocation.Method));
            DefaultSetterImplementation (target, invocation);
          }
        }
        else
        {
          invocation.Proceed ();
        }
      }
      finally
      {
        target.PropertyAccessFinished ();
      }
    }

    private void DefaultSetterImplementation (DomainObject target, IInvocation invocation)
    {
      Debug.Assert (invocation.Arguments.Length == 1);
      target.SetPropertyValue(invocation.Arguments[0]);
    }

    private void DefaultGetterImplementation (DomainObject target, IInvocation invocation)
    {
      Debug.Assert (invocation.Arguments.Length == 0);
      invocation.ReturnValue = target.GetPropertyValue<object> ();
    }
  }
}
