using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  /// <summary>
  /// Handles property accessor calls of domain objects and prepares the properties accordingly.
  /// </summary>
  [Serializable]
  public class DomainObjectPropertyInterceptor : IInterceptor<DomainObject>
  {
    public static string GetIdentifierFromProperty (PropertyInfo property)
    {
      return ReflectionUtility.GetPropertyName (property);
    }

    public static bool IsRelatedObject (Type type, string propertyID)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
      return classDefinition.GetRelationEndPointDefinition (propertyID) != null;
    }

    public static bool IsPropertyValue (Type type, string propertyID)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
      return classDefinition.GetPropertyDefinition (propertyID) != null && !IsRelatedObject (type, propertyID);
    }

    public readonly IInterceptorSelector<DomainObject> Selector;

    public DomainObjectPropertyInterceptor()
    {
      Selector = new DomainObjectPropertyInterceptorSelector (this);
    }

    public void Intercept (IInvocation<DomainObject> invocation)
    {
      DomainObject target = invocation.InvocationTarget;

      Assertion.DebugAssert (ReflectionUtility.IsPropertyAccessor (invocation.Method));
      Assertion.Assert (target != null);

      PropertyInfo property = ReflectionUtility.GetPropertyForMethod (invocation.Method);
      string id = GetIdentifierFromProperty (property);

      target.PreparePropertyAccess (id);
      try
      {
        if (invocation.Method.IsAbstract)
          HandleAutomaticProperty (invocation, target, id);
        else
          invocation.Proceed();
      }
      finally
      {
        target.PropertyAccessFinished();
      }
    }

    private void HandleAutomaticProperty (IInvocation<DomainObject> invocation, DomainObject target, string id)
    {
      if (ReflectionUtility.IsPropertyGetter (invocation.Method))
        HandleAutomaticGetter (invocation, target, id);
      else
      {
        Assertion.DebugAssert (ReflectionUtility.IsPropertySetter (invocation.Method));
        HandleAutomaticSetter (invocation, target, id);
      }
    }

    private void HandleAutomaticSetter (IInvocation<DomainObject> invocation, DomainObject target, string id)
    {
      if (IsRelatedObject (invocation.TargetType, id))
        DefaultRelatedSetterImplementation (target, invocation);
      else
      {
        Assertion.DebugAssert (IsPropertyValue (invocation.TargetType, id));
        DefaultPropertySetterImplementation (target, invocation);
      }
    }

    private void HandleAutomaticGetter (IInvocation<DomainObject> invocation, DomainObject target, string id)
    {
      if (IsRelatedObject (invocation.TargetType, id))
        DefaultRelatedGetterImplementation (target, invocation);
      else
      {
        Assertion.DebugAssert (IsPropertyValue (invocation.TargetType, id));
        DefaultPropertyGetterImplementation (target, invocation);
      }
    }

    private void DefaultPropertySetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Assertion.Assert (invocation.Arguments.Length == 1);
      target.SetPropertyValue (invocation.Arguments[0]);
    }

    private void DefaultPropertyGetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Assertion.Assert (invocation.Arguments.Length == 0);
      invocation.ReturnValue = target.GetPropertyValue<object>();
    }

    private void DefaultRelatedSetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Assertion.DebugAssert (invocation.Arguments.Length == 1 && (invocation.Arguments[0] == null || invocation.Arguments[0] is DomainObject));
      target.SetRelatedObject ((DomainObject) invocation.Arguments[0]);
    }

    private void DefaultRelatedGetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Assertion.Assert (invocation.Arguments.Length == 0);
      invocation.ReturnValue = target.GetRelatedObject();
    }
  }
}
