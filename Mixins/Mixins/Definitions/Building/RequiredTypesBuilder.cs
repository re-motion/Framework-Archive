 using System;
using System.Collections.Generic;
 using System.Diagnostics;
 using System.Text;

namespace Mixins.Definitions.Building
{
  public class RequiredTypesBuilder<TValue>
      where TValue : IVisitableDefinition
  {
    private DefinitionItemCollection<Type, TValue> _requiredTypes;
    private Type _filterAttribute;
    private BaseClassDefinition _baseClass;
    private ValueCreator _valueCreator;

    public delegate TValue ValueCreator (BaseClassDefinition baseClass, Type requiredType);

    public RequiredTypesBuilder (BaseClassDefinition baseClass, DefinitionItemCollection<Type, TValue> requiredTypeCollection, Type filterAttribute,
        ValueCreator valueCreator)
    {
      _baseClass = baseClass;
      _valueCreator = valueCreator;
      _requiredTypes = requiredTypeCollection;
      _filterAttribute = filterAttribute;
    }

    public void Apply (MixinDefinition mixin)
    {
      Type mixinBase = GetMixinBase (mixin);
      if (mixinBase != null)
      {
        if (mixinBase.Equals (mixin.Type))
        {
          string message = string.Format ("MixinBase<,> cannot be directly applied to a base class ({0}) as a mixin.", _baseClass.FullName);
          throw new ConfigurationException (message);
        }

        Debug.Assert (mixinBase.IsGenericType);

        foreach (Type genericArgument in GetFilteredGenericArguments (mixinBase))
        {
          ApplyGenericArgumentRequirements (genericArgument);
        }
      }
    }

    private IEnumerable<Type> GetFilteredGenericArguments (Type mixinBase)
    {
      Debug.Assert (!mixinBase.IsGenericTypeDefinition); // the mixinBase is always a specialization of Mixin<,>

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
      while (mixinBase != null && !IsSpecializationOf (mixinBase, typeof (Mixin<,>)))
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

    // Since mixinBase is not a generic type definition, all of its arguments are bound, either to real types or to new type parameters
    // The real types are directly taken as required interfaces; the type parameters have constraints which are taken as required interfaces
    private void ApplyGenericArgumentRequirements (Type genericArgument)
    {
      if (genericArgument.IsGenericParameter)
      {
        Type[] constraints = genericArgument.GetGenericParameterConstraints ();
        foreach (Type constraint in constraints)
        {
          ApplyRequiredType (constraint);
        }
      }
      else
      {
        ApplyRequiredType (genericArgument);
      }
    }

    private void ApplyRequiredType (Type requiredType)
    {
      Debug.Assert (!requiredType.IsGenericParameter);
      if (!_requiredTypes.HasItem (requiredType) && !requiredType.Equals(typeof(INull)))
      {
        _requiredTypes.Add (_valueCreator(_baseClass, requiredType));
      }
    }
  }
}
