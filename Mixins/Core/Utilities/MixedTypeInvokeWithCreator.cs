using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators;
using Rubicon.Collections;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Reflection;
using Rubicon.Text;

namespace Rubicon.Mixins.Utilities
{
  using CacheKey = Tuple<Type, Type>; // target type, delegate type

  public static class MixedTypeInvokeWithCreator
  {
    private static ConstructorInfo s_scopeCtor = typeof (MixedTypeInstantiationScope).GetConstructor (new Type[] { typeof (object[]) });
    private static MethodInfo s_scopeDisposeMethod = typeof (MixedTypeInstantiationScope).GetMethod ("Dispose");
    private static ICache<CacheKey, Delegate> s_delegateCache = new InterlockedCache<CacheKey, Delegate> ();

    public static FuncInvokerWrapper<T> CreateInvokeWithWrapper<T> (Type baseTypeOrInterface, params object[] mixinInstances)
    {
      Type typeToBeCreated = GetTypeToBeCreated (baseTypeOrInterface);
      Type concreteType;
      try
      {
        concreteType = TypeFactory.GetConcreteType (typeToBeCreated);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("The given base type is invalid: " + ex.Message, "T");
      }

      return new FuncInvokerWrapper<T> (new FuncInvoker<object[], T> (
          delegate (Type delegateType)
          {
            Delegate result;
            CacheKey key = new CacheKey (concreteType, delegateType);
            if (!s_delegateCache.TryGetValue (key, out result))
            {
              result = s_delegateCache.GetOrCreateValue (
                  key,
                  delegate
                  {
                    Type[] argumentTypes = ConstructorWrapper.GetParameterTypes (delegateType);
                    Type[] realArgumentTypes = new Type[argumentTypes.Length - 1];
                    Array.Copy (argumentTypes, 1, realArgumentTypes, 0, realArgumentTypes.Length);

                    const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    ConstructorInfo ctor = concreteType.GetConstructor (bindingFlags, null, CallingConventions.Any, realArgumentTypes, null);
                    if (ctor == null)
                    {
                      string message = string.Format (
                          "Type {0} does not contain a constructor with signature ({1}).",
                          typeToBeCreated.FullName,
                          SeparatedStringBuilder.Build (",", realArgumentTypes, delegate (Type t) { return t.FullName; }));
                      throw new MissingMethodException (message);
                    }
                    return CreateConstructionDelegateWithMixinInstances (ctor, delegateType);
                  });
            }
            return result;
          },
          mixinInstances));
    }

    private static Type GetTypeToBeCreated (Type baseType)
    {
      Type targetType;
      if (baseType.IsInterface)
      {
        ClassContext registeredContext = MixinConfiguration.ActiveContext.ResolveInterface (baseType);
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

    private static Delegate CreateConstructionDelegateWithMixinInstances (ConstructorInfo ctor, Type delegateType)
    {
      ParameterInfo[] parameters = ctor.GetParameters ();
      Type[] parameterTypes = new Type[parameters.Length + 1];
      parameterTypes[0] = typeof (object[]); // mixin instances
      for (int i = 0; i < parameters.Length; ++i)
        parameterTypes[i + 1] = parameters[i].ParameterType;

      Type type = ctor.DeclaringType;
      DynamicMethod method = new DynamicMethod ("ConstructorWrapperWithMixinInstances", type, parameterTypes, type);

      ILGenerator ilgen = method.GetILGenerator ();
      Label endOfMethod = ilgen.DefineLabel ();

      LocalBuilder newInstanceLocal = ilgen.DeclareLocal (type);
      LocalBuilder scopeLocal = ilgen.DeclareLocal (type);

      // using (new MixedTypeInstantiationScope (mixinInstances))

      ilgen.Emit (OpCodes.Ldarg_0); // load mixinInstances
      ilgen.Emit (OpCodes.Newobj, s_scopeCtor); // open up scope
      ilgen.Emit (OpCodes.Stloc, scopeLocal); // store scope for later

      ilgen.BeginExceptionBlock (); // try

      for (int i = 1; i < parameterTypes.Length; ++i) // load ctor arguments
        ilgen.Emit (OpCodes.Ldarg, i);

      ilgen.Emit (OpCodes.Newobj, ctor); // call ctor of mixed instance
      ilgen.Emit (OpCodes.Stloc, newInstanceLocal); // store for later
      ilgen.Emit (OpCodes.Leave, endOfMethod); // goto end of method (including execution of finally)

      ilgen.BeginFinallyBlock (); // finally

      ilgen.Emit (OpCodes.Ldloc, scopeLocal); // reload scope
      ilgen.Emit (OpCodes.Callvirt, s_scopeDisposeMethod); // dispose of scope
      ilgen.Emit (OpCodes.Endfinally); // end finally

      ilgen.EndExceptionBlock (); // end of exception block

      ilgen.MarkLabel (endOfMethod); // end of method is here

      ilgen.Emit (OpCodes.Ldloc, newInstanceLocal); // reload constructed instance
      ilgen.Emit (OpCodes.Ret); // and return

      return method.CreateDelegate (delegateType);
    }
 
  }
}
