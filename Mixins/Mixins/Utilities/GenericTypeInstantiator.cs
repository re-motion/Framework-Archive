using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Utilities
{
  public static class GenericTypeInstantiator
  {
    public static Type EnsureClosedType(Type type)
    {
      if (!type.ContainsGenericParameters)
        return type;
      else
      {
        try
        {
          return MakeClosedType (type);
        }
        catch (ArgumentException ex)
        {
          throw new ArgumentException (ex.Message, "type", ex);
        }
      }
    }

    private static Type MakeClosedType (Type typeDefinition)
    {
      Assertion.Assert (typeDefinition.IsGenericTypeDefinition);

      Type[] genericParameters = Array.FindAll (typeDefinition.GetGenericArguments (), delegate (Type t) { return t.IsGenericParameter; });
      Type[] arguments = new Type[genericParameters.Length];
      for (int i = 0; i < arguments.Length; ++i)
      {
        try
        {
          arguments[i] = GetGenericParameterInstantiation (genericParameters[i]);
        }
        catch (ArgumentException ex)
        {
          string message = string.Format ("Cannot make a closed type of {0}: {1}.", typeDefinition.FullName, ex.Message);
          throw new ArgumentException (message, "typeDefinition", ex);
        }
      }

      Type closedType = typeDefinition.MakeGenericType (arguments);
      Assertion.Assert (!closedType.ContainsGenericParameters);
      return closedType;
    }

    private static Type GetGenericParameterInstantiation (Type typeParameter)
    {
      Type candidate = null;
      if ((typeParameter.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint)
        candidate = typeof (ValueType);

      foreach (Type constraint in typeParameter.GetGenericParameterConstraints())
      {
        if (candidate == null)
          candidate = constraint;
        else if (candidate.IsAssignableFrom (constraint))
          candidate = constraint;
        else if (!constraint.IsAssignableFrom (candidate))
        {
          string message = string.Format ("The generic type parameter {0} has incompatible constraints {1} and {2}.", typeParameter.Name,
              candidate.FullName, constraint.FullName);
          throw new ArgumentException (message, "type");
        }
      }

      if (candidate == null)
        candidate = typeof (object);
      else if (candidate.Equals (typeof (ValueType)))
        candidate = typeof (int);
      return candidate;
    }
  }
}
