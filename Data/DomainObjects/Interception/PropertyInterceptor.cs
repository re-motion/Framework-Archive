using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Interception
{
  /// <summary>
  /// Handles property accessor calls of domain objects and prepares the properties accordingly.
  /// </summary>
  class PropertyInterceptor : IInterceptor<DomainObject>
  {
    class InterceptorSelector : IInterceptorSelector<DomainObject>
    {
      private PropertyInterceptor _interceptor;

      public InterceptorSelector (PropertyInterceptor interceptor)
      {
        _interceptor = interceptor;
      }
      public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
      {
        if (ReflectionUtility.IsPropertyAccessor (memberInfo))
        {
          if (memberInfo.IsAbstract)
          {
            PropertyInfo property = ReflectionUtility.GetPropertyForMethod (memberInfo);
            Debug.Assert (property != null);
            if (!property.IsDefined (typeof (AutomaticPropertyAttribute), true))
            {
              throw new InvalidOperationException ("Cannot instantiate type " + type.FullName + ", the property " + property.Name
                  + " is abstract and not declared automatic.");
            }
          }
          return true;
        }
        else
        {
          return false;
        }
      }

      public IInterceptor<DomainObject> SelectInterceptor (Type type, MethodInfo memberInfo)
      {
        Debug.Assert (ShouldInterceptMethod(type, memberInfo));
        return _interceptor;
      }
    }

    public readonly IInterceptorSelector<DomainObject> Selector;

    public PropertyInterceptor ()
    {
      Selector = new InterceptorSelector (this);
    }

    public void Intercept (IInvocation<DomainObject> invocation)
    {
      DomainObject target = invocation.InvocationTarget;
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

    private void DefaultSetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Debug.Assert (invocation.Arguments.Length == 1);
      target.SetPropertyValue(invocation.Arguments[0]);
    }

    private void DefaultGetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Debug.Assert (invocation.Arguments.Length == 0);
      invocation.ReturnValue = target.GetPropertyValue<object> ();
    }
  }
}
