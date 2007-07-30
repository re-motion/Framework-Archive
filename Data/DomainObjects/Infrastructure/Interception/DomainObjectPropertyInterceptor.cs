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
  internal class DomainObjectPropertyInterceptor : IInterceptor<DomainObject>
  {
    public static string GetIdentifierFromProperty (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      return ReflectionUtility.GetPropertyName (property);
    }

    public readonly IInterceptorSelector<DomainObject> Selector;

    public DomainObjectPropertyInterceptor()
    {
      Selector = new DomainObjectPropertyInterceptorSelector (this);
    }

    public void Intercept (IInvocation<DomainObject> invocation)
    {
      ArgumentUtility.CheckNotNull ("invocation", invocation);

      DomainObject target = invocation.InvocationTarget;
      Assertion.IsNotNull(target);

      Assertion.DebugAssert (Selector.ShouldInterceptMethod (target.GetPublicDomainObjectType (), invocation.Method));
      Assertion.DebugAssert (ReflectionUtility.IsPropertyAccessor (invocation.Method));

      PropertyInfo property = ReflectionUtility.GetPropertyForMethod (invocation.Method);
      Assertion.DebugAssert (property != null);

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
      Assertion.DebugAssert (target != null);
      Assertion.DebugAssert (id != null);
      Assertion.DebugAssert (invocation != null);

      if (ReflectionUtility.IsPropertyGetter (invocation.Method))
        invocation.ReturnValue = target.CurrentProperty.GetValueWithoutTypeCheck();
      else
      {
        Assertion.DebugAssert (ReflectionUtility.IsPropertySetter (invocation.Method));
        Assertion.DebugAssert (target.CurrentProperty.Kind != PropertyKind.RelatedObjectCollection);
        target.CurrentProperty.SetValueWithoutTypeCheck (invocation.Arguments[0]);
      }
    }
  }
}
