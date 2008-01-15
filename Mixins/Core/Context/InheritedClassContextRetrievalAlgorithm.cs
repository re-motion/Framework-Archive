using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context
{
  public class InheritedClassContextRetrievalAlgorithm
  {
    private readonly Func<Type, ClassContext> _exactGetter;
    private readonly Func<Type, ClassContext> _inheritanceAwareGetter;

    public InheritedClassContextRetrievalAlgorithm (Func<Type, ClassContext> exactGetter, Func<Type, ClassContext> inheritanceAwareGetter)
    {
      ArgumentUtility.CheckNotNull ("exactGetter", exactGetter);
      ArgumentUtility.CheckNotNull ("inheritanceAwareGetter", inheritanceAwareGetter);

      _exactGetter = exactGetter;
      _inheritanceAwareGetter = inheritanceAwareGetter;
    }

    public ClassContext GetWithInheritance (Type type)
    {
      ClassContext exactValue = _exactGetter (type);
      if (exactValue != null)
        return exactValue;

      ClassContext definitionValue = null;
      if (type.IsGenericType && !type.IsGenericTypeDefinition)
        definitionValue = _inheritanceAwareGetter (type.GetGenericTypeDefinition ());

      ClassContext baseValue = null;
      if (type.BaseType != null)
        baseValue = _inheritanceAwareGetter (type.BaseType);

      if (definitionValue != null && baseValue != null)
        return CombineContexts (type, baseValue, definitionValue);
      else if (definitionValue != null)
        return AdjustContext (type, definitionValue);
      else if (baseValue != null)
        return AdjustContext (type, baseValue);
      else
        return null;
    }

    private ClassContext CombineContexts (Type type, ClassContext inheritedOne, ClassContext inheritedTwo)
    {
      return new ClassContext (type).InheritFrom (new ClassContext[] {inheritedOne, inheritedTwo});
    }

    private ClassContext AdjustContext (Type type, ClassContext inherited)
    {
      return inherited.CloneForSpecificType (type);
    }
  }
}