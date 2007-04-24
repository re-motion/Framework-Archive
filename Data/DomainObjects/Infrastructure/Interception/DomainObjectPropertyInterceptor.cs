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

    //TODO: Test for GetMandatory
    public static bool IsPropertyValue (Type type, string propertyID)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (type);
      return classDefinition.GetPropertyDefinition (propertyID) != null && classDefinition.GetRelationEndPointDefinition (propertyID) == null;
    }

    //TODO: Test for GetMandatory
    public static bool IsRelatedObject (Type type, string propertyID)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (type);
      IRelationEndPointDefinition endPoint = classDefinition.GetRelationEndPointDefinition (propertyID);
      return endPoint != null && endPoint.Cardinality == CardinalityType.One;
    }

    //TODO: Test for GetMandatory
    public static bool IsRelatedObjectCollection (Type type, string propertyID)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (type);
      IRelationEndPointDefinition endPoint = classDefinition.GetRelationEndPointDefinition (propertyID);
      return endPoint != null && endPoint.Cardinality == CardinalityType.Many;
    }

    public static bool IsInterceptable (Type type, string propertyID)
    {
      return IsPropertyValue (type, propertyID) || IsRelatedObject (type, propertyID) || IsRelatedObjectCollection (type, propertyID);
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
      Assertion.DebugAssert (IsInterceptable (invocation.TargetType, id));

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
      if (IsRelatedObjectCollection (invocation.TargetType, id))
      {
        string message = string.Format ("There is no automatic implementation for properties that set related object collections (type {0}, "
          + "property ID {1}).", invocation.TargetType, id);
        throw new NotSupportedException (message);
      }
      else if (IsRelatedObject (invocation.TargetType, id))
        DefaultRelatedSetterImplementation (target, invocation);
      else
      {
        Assertion.DebugAssert (IsPropertyValue (invocation.TargetType, id));
        DefaultPropertySetterImplementation (target, invocation);
      }
    }

    private void HandleAutomaticGetter (IInvocation<DomainObject> invocation, DomainObject target, string id)
    {
      if (IsRelatedObjectCollection (invocation.TargetType, id))
        DefaultRelatedCollectionGetterImplementation (target, invocation);
      else if (IsRelatedObject (invocation.TargetType, id))
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

    private void DefaultRelatedCollectionGetterImplementation (DomainObject target, IInvocation<DomainObject> invocation)
    {
      Assertion.Assert (invocation.Arguments.Length == 0);
      invocation.ReturnValue = target.GetRelatedObjects ();
    }
  }
}
