using System;
using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Mixins.Utilities;

namespace Rubicon.Mixins.Context
{
  internal class InternalApplicationContextBuilder
  {
    private readonly ApplicationContext _parentContext;
    private readonly Set<Type> _extenders;
    private readonly Set<Type> _users;
    private readonly Set<Type> _potentialTargets;
    private readonly Set<Type> _completeInterfaces;
    private readonly Set<Tuple<ClassContext, Type>> _suppressedMixins;

    private ApplicationContext _builtContext;

    public InternalApplicationContextBuilder (ApplicationContext parentContext, Set<Type> extenders, Set<Type> users, Set<Type> potentialTargets,
                                              Set<Type> completeInterfaces)
    {
      _parentContext = parentContext;
      _extenders = extenders;
      _completeInterfaces = completeInterfaces;
      _potentialTargets = potentialTargets;
      _users = users;
      _suppressedMixins = new Set<Tuple<ClassContext, Type>> ();

      Analyze ();
    }

    public ApplicationContext BuiltContext
    {
      get { return _builtContext; }
    }

    public void Analyze ()
    {
      _builtContext = new ApplicationContext (_parentContext);
      foreach (Type extender in _extenders)
        AnalyzeExtender (extender);
      foreach (Type user in _users)
        AnalyzeUser (user);
      foreach (Type completeInterface in _completeInterfaces)
        AnalyzeCompleteInterface (completeInterface);
      foreach (Type type in _potentialTargets)
        AnalyzeInheritedMixins (type);
      foreach (Tuple<ClassContext, Type> suppressedMixin in _suppressedMixins)
        ApplySuppressedMixin (suppressedMixin.A, suppressedMixin.B);
    }

    private void AnalyzeExtender (Type extender)
    {
      foreach (ExtendsAttribute mixinAttribute in extender.GetCustomAttributes (typeof (ExtendsAttribute), false))
      {
        Type mixinType = extender;
        if (mixinAttribute.MixinTypeArguments.Length > 0)
        {
          try
          {
            mixinType = mixinType.MakeGenericType (mixinAttribute.MixinTypeArguments);
          }
          catch (Exception ex)
          {
            string message = string.Format ("The ExtendsAttribute for target class {0} applied to mixin type {1} specified invalid generic type "
              + "arguments.", mixinAttribute.TargetType.FullName, extender.FullName);
            throw new ConfigurationException (message, ex);
          }
        }
        ApplyMixinToClassContext (_builtContext.GetOrAddClassContext (mixinAttribute.TargetType), mixinType, mixinAttribute.AdditionalDependencies,
            mixinAttribute.SuppressedMixins);
      }
    }

    private void AnalyzeUser (Type user)
    {
      ClassContext classContext = _builtContext.GetOrAddClassContext (user);
      foreach (UsesAttribute usesAttribute in user.GetCustomAttributes (typeof (UsesAttribute), false))
        ApplyMixinToClassContext (classContext, usesAttribute.MixinType, usesAttribute.AdditionalDependencies, usesAttribute.SuppressedMixins);
    }

    private void ApplyMixinToClassContext (ClassContext classContext, Type mixinType, IEnumerable<Type> explicitDependencies,
        IEnumerable<Type> suppressedMixins)
    {
      if (AlreadyAppliedSame (mixinType, classContext))
      {
        Type typeForMessage = mixinType;
        if (typeForMessage.IsGenericType)
          typeForMessage = typeForMessage.GetGenericTypeDefinition();
        string message = string.Format ("Two instances of mixin {0} are configured for target type {1}.",
            typeForMessage.FullName, classContext.Type.FullName);
        throw new ConfigurationException (message);
      }
      else {
        MixinContext mixinContext = classContext.AddMixin (mixinType);
        foreach (Type additionalDependency in explicitDependencies)
          mixinContext.AddExplicitDependency (additionalDependency);

        foreach (Type suppressedMixinType in suppressedMixins)
        {
          if (Rubicon.Utilities.ReflectionUtility.CanAscribe (mixinType, suppressedMixinType))
          {
            string message = string.Format ("Mixin type {0} applied to target class {1} suppresses itself.", mixinType.FullName,
                classContext.Type.FullName);
            throw new ConfigurationException (message);
          }
          else
            _suppressedMixins.Add (Tuple.NewTuple (classContext, suppressedMixinType));
        }
      }
    }

    private void AnalyzeCompleteInterface (Type completeInterfaceType)
    {
      foreach (CompleteInterfaceAttribute ifaceAttribute in completeInterfaceType.GetCustomAttributes (typeof (CompleteInterfaceAttribute), false))
      {
        ClassContext classContext = _builtContext.GetOrAddClassContext (ifaceAttribute.TargetType);
        classContext.AddCompleteInterface (completeInterfaceType);
        _builtContext.RegisterInterface (completeInterfaceType, classContext);
      }
    }

    private void AnalyzeInheritedMixins (Type targetType)
    {
      Type currentBaseType = targetType.BaseType;
      while (currentBaseType != null)
      {
        if (currentBaseType.IsGenericType)
          currentBaseType = currentBaseType.GetGenericTypeDefinition ();
        AnalyzeInheritedMixins (targetType, currentBaseType);
        currentBaseType = currentBaseType.BaseType;
      }
    }

    private void AnalyzeInheritedMixins (Type targetType, Type baseType)
    {
#warning TODO: Use ClassContext.InheritFrom
      ClassContext baseTypeContext = _builtContext.GetClassContextNonRecursive (baseType);
      if (baseTypeContext != null)
      {
        foreach (MixinContext baseMixinContext in baseTypeContext.Mixins)
        {
          ClassContext targetContext = _builtContext.GetOrAddClassContext (targetType);
          if (!AlreadyAppliedSameOrDerived (baseMixinContext.MixinType, targetContext))
            baseMixinContext.CloneAndAddTo (targetContext);
        }
      }
    }

    private bool AlreadyAppliedSame (Type mixinType, ClassContext contextToCheck)
    {
      foreach (MixinContext mixin in contextToCheck.Mixins)
      {
        if (ReflectionUtility.IsSameTypeIgnoreGenerics (mixinType, mixin.MixinType))
          return true;
      }
      return false;
    }

    private bool AlreadyAppliedSameOrDerived (Type mixinType, ClassContext contextToCheck)
    {
      foreach (MixinContext mixin in contextToCheck.Mixins)
      {
        if (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (mixin.MixinType, mixinType))
          return true;
      }
      return false;
    }

    private void ApplySuppressedMixin (ClassContext classContext, Type suppressedType)
    {
      Set<Type> concreteTypes = new Set<Type> ();
      foreach (MixinContext mixin in classContext.Mixins)
      {
        if (Rubicon.Utilities.ReflectionUtility.CanAscribe (mixin.MixinType, suppressedType))
          concreteTypes.Add (mixin.MixinType);
      }
      foreach (Type concreteType in concreteTypes)
        classContext.RemoveMixin (concreteType);
    }
  }
}