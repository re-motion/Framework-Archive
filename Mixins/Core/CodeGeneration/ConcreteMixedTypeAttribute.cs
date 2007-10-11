using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Utilities;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Rubicon.Mixins.CodeGeneration
{
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class ConcreteMixedTypeAttribute : Attribute
  {
    private static ConstructorInfo s_attributeCtor =
        typeof (ConcreteMixedTypeAttribute).GetConstructor (new Type[] {typeof (Type), typeof (Type[]), typeof (Type[]), typeof (Type[])});

    public static ConcreteMixedTypeAttribute FromClassContext (ClassContext context)
    {
      Type baseType = context.Type;
      List<Type> mixinTypes = new List<Type> (context.MixinCount);
      List<Type> completeInterfaces = new List<Type> (context.CompleteInterfaceCount);
      List<Type> explicitDependencyList = new List<Type> ();

      completeInterfaces.AddRange (context.CompleteInterfaces);

      foreach (MixinContext mixin in context.Mixins)
      {
        mixinTypes.Add (mixin.MixinType);
        if (mixin.ExplicitDependencyCount > 0)
        {
          if (explicitDependencyList.Count != 0)
            explicitDependencyList.Add (typeof (NextMixinDependency));

          explicitDependencyList.Add (mixin.MixinType);
          explicitDependencyList.AddRange (mixin.ExplicitDependencies);
        }
      }

      return new ConcreteMixedTypeAttribute (baseType, mixinTypes.ToArray(), completeInterfaces.ToArray(), explicitDependencyList.ToArray ());
    }

    internal static CustomAttributeBuilder BuilderFromClassContext (ClassContext context)
    {
      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixedTypeAttribute attribute = FromClassContext (context);
      CustomAttributeBuilder builder = new CustomAttributeBuilder (s_attributeCtor,
          new object[] { attribute.TargetType, attribute.MixinTypes, attribute.CompleteInterfaces, attribute.ExplicitDependenciesPerMixin });
      return builder;
    }

    internal static Expression NewAttributeExpressionFromClassContext (ClassContext context, AbstractCodeBuilder codeBuilder)
    {
      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixedTypeAttribute attribute = FromClassContext (context);

      LocalReference mixinTypesArray = CreateArrayLocal (codeBuilder, attribute.MixinTypes);
      LocalReference completeInterfacesArray = CreateArrayLocal (codeBuilder, attribute.CompleteInterfaces);
      LocalReference explicitDependenciesPerMixinArray = CreateArrayLocal (codeBuilder, attribute.ExplicitDependenciesPerMixin);

      return new NewInstanceExpression (s_attributeCtor,
          new TypeTokenExpression (attribute.TargetType),
          mixinTypesArray.ToExpression (),
          completeInterfacesArray.ToExpression (),
          explicitDependenciesPerMixinArray.ToExpression ());
    }

    private static LocalReference CreateArrayLocal (AbstractCodeBuilder codeBuilder, Type[] types)
    {
      LocalReference arrayLocal = codeBuilder.DeclareLocal (typeof (Type[]));
      codeBuilder.AddStatement (new AssignStatement (arrayLocal, new NewArrayExpression (types.Length, typeof (Type))));
      for (int i = 0; i < types.Length; ++i)
        codeBuilder.AddStatement (new AssignArrayStatement (arrayLocal, i, new TypeTokenExpression (types[i])));
      return arrayLocal;
    }

    private readonly Type _targetType;
    private readonly Type[] _mixinTypes;
    private readonly Type[] _completeInterfaces;
    private readonly Type[] _explicitDependenciesPerMixin;

    public ConcreteMixedTypeAttribute (Type baseType, Type[] mixinTypes, Type[] completeInterfaces, Type[] explicitDependenciesPerMixin)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);
      ArgumentUtility.CheckNotNull ("completeInterfaces", completeInterfaces);
      ArgumentUtility.CheckNotNull ("explicitDependenciesPerMixin", explicitDependenciesPerMixin);

      _targetType = baseType;
      _mixinTypes = mixinTypes;
      _completeInterfaces = completeInterfaces;
      _explicitDependenciesPerMixin = explicitDependenciesPerMixin;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public Type[] MixinTypes
    {
      get { return _mixinTypes; }
    }

    public Type[] CompleteInterfaces
    {
      get { return _completeInterfaces; }
    }

    public Type[] ExplicitDependenciesPerMixin
    {
      get { return _explicitDependenciesPerMixin; }
    }

    public ClassContext GetClassContext ()
    {
      ClassContext context = new ClassContext (TargetType, MixinTypes);
      foreach (Type completeInterface in CompleteInterfaces)
        context.AddCompleteInterface (completeInterface);

      Type currentMixin = null;
      foreach (Type type in ExplicitDependenciesPerMixin)
      {
        if (type == typeof (NextMixinDependency))
          currentMixin = null;
        else if (currentMixin == null)
          currentMixin = type;
        else
          context.GetOrAddMixinContext (currentMixin).AddExplicitDependency (type);
      }

      if (TargetType.IsGenericType)
      {
        Assertion.IsTrue (context.Type.IsGenericTypeDefinition);
        context = context.SpecializeWithTypeArguments (TargetType.GetGenericArguments ());
      }
      return context;
    }

    public TargetClassDefinition GetTargetClassDefinition ()
    {
      return TargetClassDefinitionCache.Current.GetTargetClassDefinition (GetClassContext ());
    }
  }

  // marker type used in ConcreteMixedTypeAttribute
  public class NextMixinDependency
  {
  }
}