using System;
using System.Reflection.Emit;
using Mixins.CodeGeneration;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.Validation;
using Rubicon.Reflection;
using System.Reflection;
using Rubicon.Text;

namespace Mixins
{
  /// <summary>
  /// Provides support for instantiating the combinations of types with their mixins.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a target class should be combined with mixins, the target class (and sometimes also the mixin types) cannot be instantiated as
  /// is. Instead, a concrete type has to be created which incorporates the necessary delegation code, and this type is then instantiated.
  /// The <see cref="ObjectFactory"/> class provides a simple API to creating and instantiating these mixed types. If a mixed type should be
  /// created without being instantiated, refer to the <see cref="TypeFactory"/> class.
  /// </para>
  /// <para>
  /// The <see cref="ObjectFactory"/> class uses the mixin configuration defined by <see cref="MixinConfiguration.ActiveContext"/>. Use the 
  /// <see cref="MixinConfiguration"/> class if the configuration needs to be adapted.
  /// </para>
  /// </remarks>
  public class ObjectFactory
  {
    /// <summary>
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IInvokeWith{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <typeparamref name="T"/> contains severe problems that
    /// make generation of a <see cref="BaseClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <typeparamref name="T"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentException">The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.</exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType"/>. Note that this means that mixed types are created even for instances
    /// which do not have an active mixin configuration. Thus, all objects created via this method can be treated in the same way,
    /// however it might be inefficient to create arbitrary non-mixed objects using this method.
    /// </para>
    /// <para>
    /// The <see cref="Create{T}"/> method supports the creation of instances from their complete interfaces: <typeparamref name="T"/> can be an
    /// interface with exactly one <see cref="CompleteInterfaceAttribute"/> applied. For more information, see <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static InvokeWithWrapper<T> Create<T> ()
    {
      Type concreteType;
      try
      {
        concreteType = GetConcreteType (typeof (T));
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("The given base type is invalid: " + ex.Message, "T");
      }
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

    private static Type GetConcreteType (Type baseType)
    {
      if (baseType.IsInterface)
      {
        CompleteInterfaceAttribute[] completeInterfaceAttributes =
            (CompleteInterfaceAttribute[]) baseType.GetCustomAttributes (typeof (CompleteInterfaceAttribute), false);
        if (completeInterfaceAttributes.Length == 0)
        {
          string message = string.Format ("The interface {0} does not have the CompleteInterfaceAttribute attached, no instances of the "
              + "type can be created.", baseType.FullName);
          throw new ArgumentException (message);
        }
        else if (completeInterfaceAttributes.Length > 1)
        {
          string message = string.Format ("The interface {0} has multiple CompleteInterfaceAttributes attached, it is ambiguous which type "
              + "should be instantiated.", baseType.FullName);
          throw new ArgumentException (message);
        }
        else
          baseType = completeInterfaceAttributes[0].TargetType;
      }
      return TypeFactory.GetConcreteType (baseType);
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

    /// <summary>
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/> and supplies a number of prepared mixin instances.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IInvokeWith{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="ConfigurationException">The current mixin configuration for type <typeparamref name="T"/> contains severe problems that
    /// make generation of a <see cref="BaseClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for type <typeparamref name="T"/> violates at least one validation
    /// rule, which makes code generation impossible.</exception>
    /// <exception cref="ArgumentException">
    /// The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType"/>. Note that this means that mixed types are created even for instances
    /// which do not have an active mixin configuration. Thus, all objects created via this method can be treated in the same way,
    /// however it might be inefficient to create arbitrary non-mixed objects using this method.
    /// </para>
    /// <para>
    /// Usually, the mixin types configured in the <see cref="ClassContext"/> of a target class are simply instantiated when the mixed
    /// instance is initialized. Use this method instead of <see cref="Create"/> to supply pre-instantiated mixins instead.
    /// </para>
    /// <para>
    /// The <see cref="CreateWithMixinInstances{T}"/> method supports the creation of instances from their complete interfaces: <typeparamref name="T"/> can be an
    /// interface with exactly one <see cref="CompleteInterfaceAttribute"/> applied. For more information, see <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static InvokeWithWrapper<T> CreateWithMixinInstances<T> (params object[] mixinInstances)
    {
      Type concreteType;
      try
      {
        concreteType = GetConcreteType (typeof (T));
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("The given base type is invalid: " + ex.Message, "T");
      }

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