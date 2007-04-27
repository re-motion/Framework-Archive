using System;
using System.Reflection.Emit;
using Mixins.CodeGeneration;
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
    private TypeFactory _typeFactory;

    public ObjectFactory(TypeFactory typeFactory)
    {
      _typeFactory = typeFactory;
    }

    public InvokeWithWrapper<T> Create<T> ()
    {
      const BindingFlags bindingFlags  = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

      Type concreteType = _typeFactory.GetConcreteType (typeof (T));
      GetDelegateWith<T> constructionDelegateCreator = new CachedGetDelegateWith<T, Type> (
          concreteType,
          delegate (Type[] argumentTypes, Type delegateType)
          {
            ConstructorInfo ctor = concreteType.GetConstructor (bindingFlags, null, CallingConventions.Any, argumentTypes, null);
            if (ctor == null)
            {
              string message = string.Format ("Type {0} does not contain constructor with signature {1}.", typeof (T).FullName,
                SeparatedStringBuilder.Build (",", argumentTypes, delegate (Type t) { return t.FullName; }));
              throw new MissingMethodException (message);
            } 
            return CreateConstructionDelegate(ctor, delegateType);
          });
      return new InvokeWithWrapper<T>(new InvokeWith<T> (constructionDelegateCreator));
    }

    public static Delegate CreateConstructionDelegate (ConstructorInfo ctor, Type delegateType)
    {
      ParameterInfo[] parameters = ctor.GetParameters ();
      Type type = ctor.DeclaringType;
      DynamicMethod method = new DynamicMethod ("ConstructorWrapper", type, EmitUtility.GetParameterTypes (parameters), type);
      ILGenerator ilgen = method.GetILGenerator ();
      EmitUtility.PushParameters (ilgen, parameters.Length);
      ilgen.Emit (OpCodes.Newobj, ctor);
      ilgen.Emit (OpCodes.Dup);
      ilgen.EmitCall (OpCodes.Call, typeof (TypeFactory).GetMethod ("InitializeInstance"), null);
      ilgen.Emit (OpCodes.Ret);

      return method.CreateDelegate (delegateType);
    }

    public object Create (Type t, params object[] args)
    {
      Type concreteType = _typeFactory.GetConcreteType (t);
      object instance = Activator.CreateInstance (concreteType, args);
      TypeFactory.InitializeInstance (instance);
      return instance;
    }
  }
}
