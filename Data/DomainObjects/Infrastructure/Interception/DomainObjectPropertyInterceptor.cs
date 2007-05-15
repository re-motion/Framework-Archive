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

    public readonly IInterceptorSelector<DomainObject> Selector;

    public DomainObjectPropertyInterceptor()
    {
      Selector = new DomainObjectPropertyInterceptorSelector (this);
    }

    public void Intercept (IInvocation<DomainObject> invocation)
    {
      DomainObject target = invocation.InvocationTarget;
      Assertion.Assert (target != null);

      Assertion.DebugAssert (Selector.ShouldInterceptMethod (target.GetPublicDomainObjectType (), invocation.Method));
      Assertion.DebugAssert (ReflectionUtility.IsPropertyAccessor (invocation.Method));

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
      PropertyAccessor<object> propertyAccessor = new PropertyAccessor<object> (target, id, false);
      if (ReflectionUtility.IsPropertyGetter (invocation.Method))
        invocation.ReturnValue = propertyAccessor.GetValue ();
      else
      {
        Assertion.DebugAssert (ReflectionUtility.IsPropertySetter (invocation.Method));
        Assertion.DebugAssert (propertyAccessor.Kind != PropertyKind.RelatedObjectCollection);
        propertyAccessor.SetValue (invocation.Arguments[0]);
      }
    }
  }
}
