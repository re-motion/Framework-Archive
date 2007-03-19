using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Interception
{
  /// <summary>
  /// Handles property accessor calls of domain objects and prepares the properties accordingly.
  /// </summary>
  public class DomainObjectPropertyInterceptor : IInterceptor<DomainObject>
  {
    class InterceptorSelector : IInterceptorSelector<DomainObject>
    {
      private DomainObjectPropertyInterceptor _interceptor;

      public InterceptorSelector (DomainObjectPropertyInterceptor interceptor)
      {
        _interceptor = interceptor;
      }

      public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
      {
        if (ReflectionUtility.IsPropertyAccessor (memberInfo))
        {
          PropertyInfo property = ReflectionUtility.GetPropertyForMethod (memberInfo);
          if (memberInfo.IsAbstract && !Attribute.IsDefined(property, typeof (AutomaticPropertyAttribute), true))
          {
            return false;
          }

          // this interceptor only intercepts properties which are defined in the mapping either as a property definition or as a related object
          string id = DomainObjectPropertyInterceptor.GetIdentifierFromPropertyName (property.Name);
          ClassDefinition classDefinition = Mapping.MappingConfiguration.Current.ClassDefinitions[type];
          return DomainObjectPropertyInterceptor.IsPropertyValue (type, id) || DomainObjectPropertyInterceptor.IsRelatedObject (type, id);
        }
        else
        {
          return false;
        }
      }

      public IInterceptor<DomainObject> SelectInterceptor (Type type, MethodInfo memberInfo)
      {
        Assertion.DebugAssert (ShouldInterceptMethod (type, memberInfo));
        return _interceptor;
      }
    }

    private static string GetIdentifierFromPropertyName (string propertyName)
    {
      return propertyName;
    }

    public static bool IsRelatedObject (Type type, string propertyID)
    {
      ClassDefinition classDefinition = Mapping.MappingConfiguration.Current.ClassDefinitions[type];
      return classDefinition.GetRelationEndPointDefinition (propertyID) != null;
    }

    public static bool IsPropertyValue (Type type, string propertyID)
    {
      ClassDefinition classDefinition = Mapping.MappingConfiguration.Current.ClassDefinitions[type];
      return classDefinition.GetPropertyDefinition (propertyID) != null && !IsRelatedObject (type, propertyID);
    }

    public readonly IInterceptorSelector<DomainObject> Selector;

    public DomainObjectPropertyInterceptor ()
    {
      Selector = new InterceptorSelector (this);
    }

    public void Intercept (IInvocation<DomainObject> invocation)
    {
      DomainObject target = invocation.InvocationTarget;

      Assertion.DebugAssert (ReflectionUtility.IsPropertyAccessor (invocation.Method));
      Assertion.Assert (target != null);

      string propertyName = ReflectionUtility.GetPropertyNameForMethodName (invocation.Method.Name);
      string id = DomainObjectPropertyInterceptor.GetIdentifierFromPropertyName (propertyName);
      
      target.PreparePropertyAccess (id);
      try
      {
        if (invocation.Method.IsAbstract)
        {
          HandleAutomaticProperty (invocation, target, id);
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

    private void HandleAutomaticProperty (IInvocation<DomainObject> invocation, DomainObject target, string id)
    {
      if (ReflectionUtility.IsPropertyGetter (invocation.Method))
      {
        HandleAutomaticGetter (invocation, target, id);
      }
      else
      {
        Assertion.DebugAssert (ReflectionUtility.IsPropertySetter (invocation.Method));
        HandleAutomaticSetter (invocation, target, id);
      }
    }

    private void HandleAutomaticSetter (IInvocation<DomainObject> invocation, DomainObject target, string id)
    {
      if (DomainObjectPropertyInterceptor.IsRelatedObject (invocation.TargetType, id))
      {
        DefaultRelatedSetterImplementation (target, invocation);
      }
      else
      {
        Assertion.DebugAssert(DomainObjectPropertyInterceptor.IsPropertyValue (invocation.TargetType, id));
        DefaultPropertySetterImplementation (target, invocation);
      }
    }

    private void HandleAutomaticGetter (IInvocation<DomainObject> invocation, DomainObject target, string id)
    {
      if (DomainObjectPropertyInterceptor.IsRelatedObject (invocation.TargetType, id))
      {
        DefaultRelatedGetterImplementation (target, invocation);
      }
      else
      {
        Assertion.DebugAssert (DomainObjectPropertyInterceptor.IsPropertyValue (invocation.TargetType, id));
        DefaultPropertyGetterImplementation (target, invocation);
      }
    }

    private void DefaultPropertySetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Assertion.Assert (invocation.Arguments.Length == 1);
      target.SetPropertyValue(invocation.Arguments[0]);
    }

    private void DefaultPropertyGetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Assertion.Assert (invocation.Arguments.Length == 0);
      invocation.ReturnValue = target.GetPropertyValue<object> ();
    }

    private void DefaultRelatedSetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Assertion.DebugAssert (invocation.Arguments.Length == 1 && invocation.Arguments[0] is DomainObject);
      target.SetRelatedObject ((DomainObject) invocation.Arguments[0]);
    }

    private void DefaultRelatedGetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Assertion.Assert (invocation.Arguments.Length == 0);
      invocation.ReturnValue = target.GetRelatedObject ();
    }
  }
}
