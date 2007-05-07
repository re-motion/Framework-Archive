using System;
using System.Reflection.Emit;
using Mixins.CodeGeneration;
using Mixins.UnitTests.SampleTypes;
using Rubicon.Reflection;
using Mixins.Definitions;
using System.Reflection;
using Rubicon.Text;

namespace Mixins.UnitTests.Mixins
{
  public struct InvokeWithWrapper<T>
  {
    private readonly IInvokeWith<T> _invokeWith;

    public InvokeWithWrapper(IInvokeWith<T> invokeWith)
    {
      _invokeWith = invokeWith;
    }

    public T With()
    {
      return _invokeWith.With();
    }

    public T With<A1> (A1 a1)
    {
      return _invokeWith.With (a1);
    }

    public T With<A1, A2> (A1 a1, A2 a2)
    {
      return _invokeWith.With (a1, a2);
    }
  }

  public class ObjectFactory
  {
    public static InvokeWithWrapper<T> Create<T> ()
    {
      Type concreteType = TypeFactory.Current.GetConcreteType (typeof (T));
      GetDelegateWith<T> constructionDelegateCreator = new CachedGetDelegateWith<T, Type> (
          concreteType,
          delegate (Type[] argumentTypes, Type delegateType)
          {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            ConstructorInfo ctor = concreteType.GetConstructor (bindingFlags, null, CallingConventions.Any, argumentTypes, null);
            if (ctor == null)
            {
              string message = string.Format ("Type {0} does not contain constructor with signature {1}.", typeof (T).FullName,
                SeparatedStringBuilder.Build (",", argumentTypes, delegate (Type t) { return t.FullName; }));
              throw new MissingMethodException (message);
            } 
            return CreateConstructionAndDelegate(ctor, delegateType);
          });
      return new InvokeWithWrapper<T>(new InvokeWith<T> (constructionDelegateCreator));
    }

    private static Delegate CreateConstructionAndDelegate (ConstructorInfo ctor, Type delegateType)
    {
      ParameterInfo[] parameters = ctor.GetParameters ();
      Type type = ctor.DeclaringType;
      DynamicMethod method = new DynamicMethod ("ConstructorWrapper", type, EmitUtility.GetParameterTypes (parameters), type);
      ILGenerator ilgen = method.GetILGenerator ();
      EmitUtility.PushParameters (ilgen, parameters.Length);
      ilgen.Emit (OpCodes.Newobj, ctor);
      ilgen.Emit (OpCodes.Dup);
      ilgen.EmitCall (OpCodes.Call, typeof (TypeFactory).GetMethod ("InitializeMixedInstance"), null);
      ilgen.Emit (OpCodes.Ret);

      return method.CreateDelegate (delegateType);
    }

    /*public object Create (Type t, params object[] args)
    {
      Type concreteType = TypeFactory.Current.GetConcreteType (t);
      object instance = Activator.CreateInstance (concreteType, args);
      TypeFactory.InitializeMixedInstance (instance);
      return instance;
    }*/

    public static InvokeWithWrapper<T> CreateWithMixinInstances<T> (params object[] mixinInstances)
    {
      Type concreteType = TypeFactory.Current.GetConcreteType (typeof (T));
      GetDelegateWith<T> constructionDelegateCreator = new CachedGetDelegateWith<T, Type> (
          concreteType,
          delegate (Type[] argumentTypes, Type delegateType)
          {
            Type[] realArgumentTypes = new Type[argumentTypes.Length - 1];
            Array.Copy (argumentTypes, 1, realArgumentTypes, 0, realArgumentTypes.Length);

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            ConstructorInfo ctor = concreteType.GetConstructor (bindingFlags, null, CallingConventions.Any, realArgumentTypes, null);
            if (ctor == null)
            {
              string message = string.Format ("Type {0} does not contain constructor with signature {1}.", typeof (T).FullName,
                SeparatedStringBuilder.Build (",", realArgumentTypes, delegate (Type t) { return t.FullName; }));
              throw new MissingMethodException (message);
            }
            return CreateConstructionDelegateWithMixinInstances (ctor, delegateType);
          });
      return new InvokeWithWrapper<T> (new InvokeWithBoundFirst<T, object[]> (constructionDelegateCreator, mixinInstances));
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

      ILGenerator ilgen = method.GetILGenerator();
      LocalBuilder newInstanceLocal = ilgen.DeclareLocal (type);
      for (int i = 1; i < parameters.Length; ++i)
      {
        ilgen.Emit (OpCodes.Ldarg, i);
      }
      ilgen.Emit (OpCodes.Newobj, ctor);
      ilgen.Emit (OpCodes.Stloc_0);
      ilgen.Emit (OpCodes.Ldloc_0);
      ilgen.Emit (OpCodes.Ldarg_0);
      ilgen.EmitCall (OpCodes.Call, typeof (TypeFactory).GetMethod ("InitializeMixedInstanceWithMixins"), null);
      ilgen.Emit (OpCodes.Ldloc_0);
      ilgen.Emit (OpCodes.Ret);

      return method.CreateDelegate (delegateType);
    }
  }
}
