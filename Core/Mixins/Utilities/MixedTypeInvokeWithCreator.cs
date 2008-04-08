using System;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Collections;
using Remotion.Mixins.Context;
using Remotion.Reflection;
using Remotion.Text;

namespace Remotion.Mixins.Utilities
{
  using CacheKey = Tuple<Type, Type>;
  using Remotion.Utilities; // target type, delegate type

  internal class MixedTypeInvokeWithCreator : TypesafeActivator.ConstructorLookupInfo
  {
    private static readonly ConstructorInfo s_scopeCtor = typeof (MixedTypeInstantiationScope).GetConstructor (new Type[] { typeof (object[]) });
    private static readonly MethodInfo s_scopeDisposeMethod = typeof (MixedTypeInstantiationScope).GetMethod ("Dispose");

    public static FuncInvokerWrapper<T> CreateConstructorInvoker<T> (Type baseTypeOrInterface, GenerationPolicy generationPolicy, bool allowNonPublic,
        params object[] preparedMixins)
    {
      Type resolvedTargetType = ResolveType (baseTypeOrInterface);
      Type concreteType;
      try
      {
        concreteType = TypeFactory.GetConcreteType (resolvedTargetType, generationPolicy);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("The given base type is invalid: " + ex.Message, "baseTypeOrInterface", ex);
      }

      if (!typeof (IMixinTarget).IsAssignableFrom (concreteType) && preparedMixins.Length > 0)
        throw new ArgumentException (string.Format ("There is no mixin configuration for type {0}, so no mixin instances must be specified.",
            baseTypeOrInterface.FullName), "preparedMixins");

      FuncInvoker<object[], T> constructorInvoker =
          new FuncInvoker<object[], T> (new MixedTypeInvokeWithCreator (concreteType, resolvedTargetType, allowNonPublic).GetDelegate, preparedMixins);
      return new FuncInvokerWrapper<T> (constructorInvoker);
    }

    private static Type ResolveType (Type baseType)
    {
      Type targetType;
      if (baseType.IsInterface)
      {
        ClassContext registeredContext = MixinConfiguration.ActiveConfiguration.ResolveInterface (baseType);
        if (registeredContext == null)
        {
          string message = string.Format ("The interface {0} has not been registered in the current configuration, no instances of the "
              + "type can be created.", baseType.FullName);
          throw new ArgumentException (message);
        }
        targetType = registeredContext.Type;
      }
      else
        targetType = baseType;
      return targetType;
    }

    private readonly Type _concreteType;
    private readonly Type _targetType;
    private readonly bool _allowNonPublic;

    private MixedTypeInvokeWithCreator (Type concreteType, Type targetType, bool allowNonPublic)
        : base (concreteType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
    {
      _concreteType = concreteType;
      _targetType = targetType;
      _allowNonPublic = allowNonPublic;
    }

    // This method reimplements parts of ConstructorWrapper because we need a delegate that interpretes the first argument passed to it
    // as the preallocated mixin instances.
    protected override Delegate CreateDelegate (Type delegateType)
    {
      Type[] argumentTypes = ConstructorWrapper.GetParameterTypes (delegateType);
      Type[] realArgumentTypes = new Type[argumentTypes.Length - 1];
      Array.Copy (argumentTypes, 1, realArgumentTypes, 0, realArgumentTypes.Length); // remove first argument, those are the mixin instances

      ConstructorInfo ctor = _concreteType.GetConstructor (BindingFlags, null, CallingConventions.Any, realArgumentTypes, null);
      ConstructorInfo targetTypeCtor = _targetType.GetConstructor (BindingFlags, null, CallingConventions.Any, realArgumentTypes, null);
      if (targetTypeCtor == null || (!targetTypeCtor.IsPublic && !_allowNonPublic))
      {
        string message = string.Format (
            "Type {0} does not contain a public constructor with signature ({1}).", _targetType.FullName,
            SeparatedStringBuilder.Build (",", realArgumentTypes, delegate (Type t) { return t.FullName; }));
        throw new MissingMethodException (message);
      }
      else
        Assertion.IsNotNull (ctor);

      return CreateConstructionDelegateWithpreparedMixins (ctor, delegateType);
    }

    private static Delegate CreateConstructionDelegateWithpreparedMixins (ConstructorInfo ctor, Type delegateType)
    {
      ParameterInfo[] parameters = ctor.GetParameters();
      Type[] parameterTypes = new Type[parameters.Length + 1];
      parameterTypes[0] = typeof (object[]); // mixin instances
      for (int i = 0; i < parameters.Length; ++i)
        parameterTypes[i + 1] = parameters[i].ParameterType;

      Type type = ctor.DeclaringType;
      DynamicMethod method = new DynamicMethod ("ConstructorWrapperWithpreparedMixins", type, parameterTypes, type);

      ILGenerator ilgen = method.GetILGenerator();
      Label endOfMethod = ilgen.DefineLabel();

      LocalBuilder newInstanceLocal = ilgen.DeclareLocal (type);
      LocalBuilder scopeLocal = ilgen.DeclareLocal (type);

      // using (new MixedTypeInstantiationScope (preparedMixins))

      ilgen.Emit (OpCodes.Ldarg_0); // load preparedMixins
      ilgen.Emit (OpCodes.Newobj, s_scopeCtor); // open up scope
      ilgen.Emit (OpCodes.Stloc, scopeLocal); // store scope for later

      ilgen.BeginExceptionBlock(); // try

      for (int i = 1; i < parameterTypes.Length; ++i) // load ctor arguments
        ilgen.Emit (OpCodes.Ldarg, i);

      ilgen.Emit (OpCodes.Newobj, ctor); // call ctor of mixed instance
      ilgen.Emit (OpCodes.Stloc, newInstanceLocal); // store for later
      ilgen.Emit (OpCodes.Leave, endOfMethod); // goto end of method (including execution of finally)

      ilgen.BeginFinallyBlock(); // finally

      ilgen.Emit (OpCodes.Ldloc, scopeLocal); // reload scope
      ilgen.Emit (OpCodes.Callvirt, s_scopeDisposeMethod); // dispose of scope
      ilgen.Emit (OpCodes.Endfinally); // end finally

      ilgen.EndExceptionBlock(); // end of exception block

      ilgen.MarkLabel (endOfMethod); // end of method is here

      ilgen.Emit (OpCodes.Ldloc, newInstanceLocal); // reload constructed instance
      ilgen.Emit (OpCodes.Ret); // and return

      return method.CreateDelegate (delegateType);
    }
  }
}
