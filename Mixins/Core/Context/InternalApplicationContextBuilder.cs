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
    private readonly Set<Type> _completeInterfaces;
    private readonly Set<Tuple<ClassContext, Type>> _suppressedMixins;

    private readonly Set<Type> _targets;
    private readonly Set<Type> _finishedTargets;

    private ApplicationContext _builtContext;

    public InternalApplicationContextBuilder (ApplicationContext parentContext, Set<Type> extenders, Set<Type> users, Set<Type> completeInterfaces)
    {
      _parentContext = parentContext;
      _extenders = extenders;
      _completeInterfaces = completeInterfaces;
      _users = users;

      _targets = new Set<Type> ();
      _finishedTargets = new Set<Type> ();
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
      
      // At this point of time, every type in _targets has exactly those mixins configured espectially for it

      foreach (Type target in _targets)
        AnalyzeInheritedMixins (target);

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
      else
      {
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

        _targets.Add (classContext.Type);
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
      if (_finishedTargets.Contains (targetType))
        return;

      ClassContext targetContext = _builtContext.GetClassContextNonRecursive (targetType); // null if no specific mixins are configured for this type

      Type baseType = targetType.BaseType;
      if (baseType != null)
        AnalyzeBaseAndInherit(targetContext, baseType);

      Type genericTypeDefinition = targetType.IsGenericType && !targetType.IsGenericTypeDefinition ? targetType.GetGenericTypeDefinition() : null;
      if (genericTypeDefinition != null)
        AnalyzeBaseAndInherit (targetContext, genericTypeDefinition);

      _finishedTargets.Add (targetType);
    }

    private void AnalyzeBaseAndInherit (ClassContext targetContext, Type baseType)
    {
      AnalyzeInheritedMixins (baseType);
      if (targetContext != null)
      {
        ClassContext baseContext = _builtContext.GetClassContext (baseType); // this will include the base type's inherited stuff
        if (baseContext != null)
          targetContext.InheritFrom (baseContext);
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