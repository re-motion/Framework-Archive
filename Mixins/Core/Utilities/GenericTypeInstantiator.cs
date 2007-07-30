using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Utilities
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
      Assertion.IsTrue (typeDefinition.IsGenericTypeDefinition);

      List<Type> instantiations = new List<Type> ();

      foreach (Type genericArgument in typeDefinition.GetGenericArguments ())
      {
        if (genericArgument.IsGenericParameter)
        {
          Type instantiation;
          try
          {
            instantiation = GetGenericParameterInstantiation (genericArgument, null);
          }
          catch (NotSupportedException ex)
          {
            string message = string.Format ("Cannot make a closed type of {0}: {1}", typeDefinition.FullName, ex.Message);
            throw new NotSupportedException (message, ex);
          }
          instantiations.Add (instantiation);
        }
      }

      Type closedType = typeDefinition.MakeGenericType (instantiations.ToArray ());
      Assertion.IsFalse (closedType.ContainsGenericParameters);
      return closedType;
    }

    public static Type GetGenericParameterInstantiation (Type typeParameter, Type suggestedInstantiation)
    {
      ArgumentUtility.CheckNotNull ("typeParameter", typeParameter);

      if (!typeParameter.IsGenericParameter)
        throw new ArgumentException ("Type must be a generic parameter.", "typeParameter");

      Type candidate = null;
      if ((typeParameter.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint)
        candidate = typeof (ValueType);

      foreach (Type constraint in typeParameter.GetGenericParameterConstraints())
      {
        if (constraint.ContainsGenericParameters)
        {
          string message = string.Format ("The generic type parameter {0} has a constraint {1} which itself contains generic parameters.",
              typeParameter.Name, constraint.Name);
          throw new NotSupportedException (message);
        }

        if (candidate == null)
          candidate = constraint;
        else if (candidate.IsAssignableFrom (constraint))
          candidate = constraint;
        else if (!constraint.IsAssignableFrom (candidate))
        {
          if (constraint.IsAssignableFrom (suggestedInstantiation) && candidate.IsAssignableFrom (suggestedInstantiation))
            candidate = suggestedInstantiation;
          else
          {
            string message = string.Format (
                "The generic type parameter {0} has incompatible constraints {1} and {2}.",
                typeParameter.Name,
                candidate.FullName,
                constraint.FullName);
            throw new NotSupportedException (message);
          }
        }
      }

      if (candidate == null)
        candidate = typeof (object);
      else if (candidate.Equals (typeof (ValueType)))
        candidate = typeof (int);

      if (suggestedInstantiation != null && candidate.IsAssignableFrom (suggestedInstantiation))
        candidate = suggestedInstantiation;

      return candidate;
    }
  }
}
