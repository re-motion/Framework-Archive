 using System;
 using System.Collections.Generic;
 using System.Diagnostics;
 using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public class RequirementsAnalyzer
  {
    private Type _filterAttribute;
    private BaseClassDefinition _baseClass;

    public RequirementsAnalyzer (BaseClassDefinition baseClass, Type filterAttribute)
    {
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);
      ArgumentUtility.CheckNotNull ("filterAttribute", filterAttribute);

      _baseClass = baseClass;
      _filterAttribute = filterAttribute;
    }

    public IEnumerable<Type> Analyze (MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      Dictionary<Type, Type> requirements = new Dictionary<Type, Type> ();

      Type mixinBase = GetMixinBase (mixin);
      if (mixinBase != null)
      {
        if (mixinBase.Equals (mixin.Type))
        {
          string message = string.Format ("The Mixin classes cannot be directly applied to a base class ({0}) as a mixin.", _baseClass.FullName);
          throw new ConfigurationException (message);
        }

        Debug.Assert (mixinBase.IsGenericType);

        foreach (Type genericArgument in GetFilteredGenericArguments (mixinBase))
        {
          AnalyzeRequirementsForMixinBaseArgument (genericArgument, requirements);
        }
      }
      return requirements.Keys;
    }

    private IEnumerable<Type> GetFilteredGenericArguments (Type mixinBase)
    {
      Debug.Assert (!mixinBase.IsGenericTypeDefinition); // the mixinBase is always a specialization of Mixin<,> or Mixin<>

      Type[] genericArguments = mixinBase.GetGenericArguments ();
      Type[] originalGenericParameters = mixinBase.GetGenericTypeDefinition ().GetGenericArguments ();

      for (int i = 0; i < genericArguments.Length; ++i)
      {
        if (originalGenericParameters[i].IsDefined (_filterAttribute, false))
        {
          yield return genericArguments[i];
        }
      }
    }

    private Type GetMixinBase (MixinDefinition mixin)
    {
      Type mixinBase = mixin.Type.BaseType;
      while (mixinBase != null && !(IsSpecializationOf (mixinBase, typeof (Mixin<,>)) || IsSpecializationOf (mixinBase, typeof (Mixin<>))))
      {
        mixinBase = mixinBase.BaseType;
      }
      return mixinBase;
    }

    private bool IsSpecializationOf (Type typeToCheck, Type requestedType)
    {
      if (requestedType.IsAssignableFrom (typeToCheck))
      {
        return true;
      }
      else if (typeToCheck.IsGenericType && !typeToCheck.IsGenericTypeDefinition)
      {
        Type typeDefinition = typeToCheck.GetGenericTypeDefinition ();
        return IsSpecializationOf (typeDefinition, requestedType);
      }
      else
      {
        return false;
      }
    }

    // The generic arguments used for MixinBase<,> are bound to either to real types or to new type parameters
    // The real types are directly taken as required interfaces; the type parameters have constraints which are taken as required interfaces
    private void AnalyzeRequirementsForMixinBaseArgument (Type genericArgument, Dictionary<Type, Type> requirements)
    {
      if (genericArgument.IsGenericParameter)
      {
        Type[] constraints = genericArgument.GetGenericParameterConstraints ();
        foreach (Type constraint in constraints)
        {
          AnalyzeRequirementForType (constraint, requirements);
        }
      }
      else
      {
        AnalyzeRequirementForType (genericArgument, requirements);
      }
    }

    private void AnalyzeRequirementForType (Type requiredType, Dictionary<Type, Type> requirements)
    {
      Debug.Assert (!requiredType.IsGenericParameter);
      if (!requirements.ContainsKey (requiredType))
      {
        requirements.Add (requiredType, requiredType);
      }
    }
  }
}
