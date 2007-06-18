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
      return CreateWithMixinInstances<T>();
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
      Type typeToBeCreated = GetTypeToBeCreated (typeof (T));
      Type concreteType;
      try
      {
        concreteType = TypeFactory.GetConcreteType (typeToBeCreated);
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
              string message = string.Format ("Type {0} does not contain a constructor with signature ({1}).", typeToBeCreated.FullName,
                  SeparatedStringBuilder.Build (",", realArgumentTypes, delegate (Type t) { return t.FullName; }));
              throw new MissingMethodException (message);
            }
            return CreateConstructionDelegateWithMixinInstances (ctor, delegateType);
          });
      return new InvokeWithWrapper<T> (new InvokeWithBoundFirst<T, object[]> (constructionDelegateCreator, mixinInstances));
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

      ILGenerator ilgen = method.GetILGenerator();
      LocalBuilder newInstanceLocal = ilgen.DeclareLocal (type);
      for (int i = 1; i < parameterTypes.Length; ++i)
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