using System;
using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Mixins;
using Rubicon.Mixins.Utilities;
using Rubicon.Mixins.Utilities.Serialization;
using Rubicon.Utilities;
using System.Runtime.Serialization;
using System.Text;
using ReflectionUtility=Rubicon.Utilities.ReflectionUtility;

namespace Rubicon.Mixins.Context
{
  /// <summary>
  /// Holds the mixin configuration information for a single mixin target class.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  /// <remarks>
  /// Instances of this class are immutable.
  /// </remarks>
  [Serializable]
  public sealed class ClassContext : ISerializable
  {
    private static bool ContainsOverrideForMixin (IEnumerable<MixinContext> mixinContexts, Type mixinType)
    {
      return GetOverrideForMixin (mixinContexts, mixinType) != null;
    }

    private static MixinContext GetOverrideForMixin (IEnumerable<MixinContext> mixinContexts, Type mixinType)
    {
      Type typeToSearchFor = mixinType.IsGenericType ? mixinType.GetGenericTypeDefinition () : mixinType;

      foreach (MixinContext mixin in mixinContexts)
      {
        if (ReflectionUtility.CanAscribe (mixin.MixinType, typeToSearchFor))
          return mixin;
      }
      return null;
    }

    // A = overridden, B = override
    private static Tuple<MixinContext, MixinContext> GetFirstOverrideThatIsNotOverriddenByBase (IEnumerable<MixinContext> baseMixins,
        IEnumerable<MixinContext> potentialOverrides)
    {
      foreach (MixinContext mixin in baseMixins)
      {
        MixinContext overrideForMixin;
        if ((overrideForMixin = GetOverrideForMixin (potentialOverrides, mixin.MixinType)) != null
            && !ContainsOverrideForMixin (baseMixins, overrideForMixin.MixinType))
          return Tuple.NewTuple (mixin, overrideForMixin);
      }
      return null;
    }

    private static int CalculateHashCode (ClassContext classContext)
    {
      return classContext.Type.GetHashCode ()
          ^ EqualityUtility.GetRotatedHashCode (classContext.Mixins)
          ^ EqualityUtility.GetRotatedHashCode (classContext.CompleteInterfaces);
    }

    private readonly Type _type;
    private readonly Dictionary<Type, MixinContext> _mixins;
    private readonly UncastableEnumerableWrapper<MixinContext> _mixinWrapperForOutside;
    private readonly List<Type> _completeInterfaces;
    private readonly UncastableEnumerableWrapper<Type> _completeInterfaceWrapperForOutside;
    private readonly int _cachedHashCode;

    /// <summary>
    /// Initializes a new <see cref="ClassContext"/> for a given mixin target type.
    /// </summary>
    /// <param name="type">The mixin target type to be represented by this context.</param>
    /// <param name="mixins">A list of <see cref="MixinContext"/> objects representing the mixins applied to this class.</param>
    /// <param name="completeInterfaces">The complete interfaces supported by the class.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
    public ClassContext (Type type, IEnumerable<MixinContext> mixins, IEnumerable<Type> completeInterfaces)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _type = type;

      _mixins = new Dictionary<Type, MixinContext> ();
      foreach (MixinContext mixin in mixins)
        _mixins.Add (mixin.MixinType, mixin);
      _mixinWrapperForOutside = new UncastableEnumerableWrapper<MixinContext> (_mixins.Values);
      
      _completeInterfaces = new List<Type> (new Set<Type> (completeInterfaces));
      _completeInterfaceWrapperForOutside = new UncastableEnumerableWrapper<Type> (_completeInterfaces);

      _cachedHashCode = CalculateHashCode (this);
    }

    /// <summary>
    /// Initializes a new <see cref="ClassContext"/> for a given target type without mixins.
    /// </summary>
    /// <param name="type">The mixin target type to be represented by this context.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
    public ClassContext (Type type)
      : this (type, new MixinContext[0], new Type[0])
    {
    }

    /// <summary>
    /// Initializes a new <see cref="ClassContext"/> for a given mixin target type.
    /// </summary>
    /// <param name="type">The mixin target type to be represented by this context.</param>
    /// <param name="mixins">A list of <see cref="MixinContext"/> objects representing the mixins applied to this class.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
    public ClassContext (Type type, params MixinContext[] mixins)
        : this (type, mixins, new Type[0])
    {
    }

    /// <summary>
    /// Initializes a new <see cref="ClassContext"/> for a given mixin target type and initializes it to be associated with the given
    /// mixin types.
    /// </summary>
    /// <param name="type">The mixin target type to be represented by this context.</param>
    /// <param name="mixinTypes">The mixin types to be associated with this context.</param>
    /// <exception cref="ArgumentNullException">One of the parameters passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="mixinTypes"/> parameter contains duplicates.</exception>
    public  ClassContext (Type type, params Type[] mixinTypes)
        : this (type, new MixinContext[0], new Type[0])
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);

      foreach (Type mixinType in mixinTypes)
      {
        if (!ContainsMixin (mixinType))
        {
          MixinContext context = new MixinContext (mixinType, new Type[0]);
          _mixins.Add (context.MixinType, context);
        }
        else
        {
          string message = string.Format ("The mixin type {0} was tried to be added twice.", mixinType.FullName);
          throw new ArgumentException (message, "mixinTypes");
        }
      }
    }

    private ClassContext (SerializationInfo info, StreamingContext context)
    {
      _type = ReflectionObjectSerializer.DeserializeType ("_type", info);

      int mixinCount = info.GetInt32 ("_mixins.Count");
      _mixins = new Dictionary<Type, MixinContext> (mixinCount);
      for (int i = 0; i < mixinCount; ++i)
      {
        MixinContext mixinContext = MixinContext.DeserializeFromFlatStructure (this, "_mixins[" + i + "]", info);
        _mixins.Add (mixinContext.MixinType, mixinContext);
      }
      _mixinWrapperForOutside = new UncastableEnumerableWrapper<MixinContext> (_mixins.Values);

      int completeInterfaceCount = info.GetInt32 ("_completeInterfaces.Count");
      _completeInterfaces = new List<Type> (completeInterfaceCount);
      for (int i = 0; i < completeInterfaceCount; ++i)
        _completeInterfaces.Add (ReflectionObjectSerializer.DeserializeType ("_completeInterfaces[" + i + "]", info));
      _completeInterfaceWrapperForOutside = new UncastableEnumerableWrapper<Type> (_completeInterfaces);

      _cachedHashCode = CalculateHashCode (this);
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      ReflectionObjectSerializer.SerializeType (_type, "_type", info);
      info.AddValue ("_mixins.Count", _mixins.Count);
      IEnumerator<MixinContext> mixinEnumerator = _mixins.Values.GetEnumerator ();
      for (int i = 0; mixinEnumerator.MoveNext(); ++i)
        mixinEnumerator.Current.SerializeIntoFlatStructure ("_mixins[" + i + "]", info);

      info.AddValue ("_completeInterfaces.Count", _completeInterfaces.Count);
      for (int i = 0; i < _completeInterfaces.Count; ++i)
        ReflectionObjectSerializer.SerializeType (_completeInterfaces[i], "_completeInterfaces[" + i + "]", info);
    }

    /// <summary>
    /// Gets the type represented by this <see cref="ClassContext"/>.
    /// </summary>
    /// <value>The type represented by this context.</value>
    public Type Type
    {
      get { return _type; }
    }

    /// <summary>
    /// Gets the mixins associated with this <see cref="ClassContext"/>.
    /// </summary>
    /// <value>The mixins associated with this context.</value>
    public IEnumerable<MixinContext> Mixins
    {
      get { return _mixinWrapperForOutside; }
    }

    /// <summary>
    /// Gets the complete interfaces associated with this <see cref="ClassContext"/>.
    /// </summary>
    /// <value>The complete interfaces associated with this context (for an explanation, see <see cref="CompleteInterfaceAttribute"/>).</value>
    public IEnumerable<Type> CompleteInterfaces
    {
      get { return _completeInterfaceWrapperForOutside; }
    }

    /// <summary>
    /// Gets the number of mixins associated with this <see cref="ClassContext"/>.
    /// </summary>
    /// <value>The mixin count of this context.</value>
    public int MixinCount
    {
      get
      {
          return _mixins.Count;
      }
    }

    /// <summary>
    /// Gets the number of complete interfaces associated with this <see cref="ClassContext"/>.
    /// </summary>
    /// <value>The complete interface count.</value>
    public int CompleteInterfaceCount
    {
      get
      {
          return _completeInterfaces.Count;
      }
    }

    /// <summary>
    /// Determines whether this <see cref="ClassContext"/> contains the specified mixin type.
    /// </summary>
    /// <param name="mixinType">The mixin type to check for.</param>
    /// <returns>
    /// True if the <see cref="ClassContext"/> contains the given mixin type; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">The <paramref name="mixinType"/> parameter is <see langword="null"/>.</exception>
    public bool ContainsMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
        return _mixins.ContainsKey (mixinType);
    }

    /// <summary>
    /// Determines whether this <see cref="ClassContext"/> contains a mixin type assignable to the specified type.
    /// </summary>
    /// <param name="baseMixinType">The mixin type to check for.</param>
    /// <returns>
    /// True if the <see cref="ClassContext"/> contains a type assignable to the specified type; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">The <paramref name="baseMixinType"/> parameter is <see langword="null"/>.</exception>
    public bool ContainsAssignableMixin (Type baseMixinType)
    {
      ArgumentUtility.CheckNotNull ("baseMixinType", baseMixinType);
        foreach (MixinContext mixin in Mixins)
        {
          if (baseMixinType.IsAssignableFrom (mixin.MixinType))
            return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether a mixin configured with this <see cref="ClassContext"/> overrides the given <paramref name="mixinType"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type which is to be checked for overriders.</param>
    /// <returns>
    /// True if the specified mixin type is overridden in this class context; otherwise, false.
    /// </returns>
    public bool ContainsOverrideForMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      return ContainsOverrideForMixin (Mixins, mixinType);
    }

    /// <summary>
    /// Gets the <see cref="MixinContext"/> for the given mixin type associated with this <see cref="ClassContext"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to retrieve a <see cref="MixinContext"/> for.</param>
    /// <returns>A <see cref="MixinContext"/> for the given mixin type associated with this <see cref="ClassContext"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="mixinType"/> parameter is <see langword="null"/>.</exception>
    public MixinContext GetMixinContext (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
        if (!ContainsMixin (mixinType))
          return null;
        else
          return _mixins[mixinType];
    }

    /// <summary>
    /// Determines whether this <see cref="ClassContext"/> contains the given complete interface.
    /// </summary>
    /// <param name="interfaceType">Interface type to check for.</param>
    /// <returns>
    /// True if this <see cref="ClassContext"/> contains the given complete interface; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> parameter is <see langword="null"/>.</exception>
    public bool ContainsCompleteInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
        return _completeInterfaces.Contains (interfaceType);
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="ClassContext"/>.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"></see> to compare with this <see cref="ClassContext"/>.</param>
    /// <returns>
    /// True if the specified <see cref="T:System.Object"></see> is a <see cref="ClassContext"/> for the same type with equal mixin 
    /// and complete interfaces configuration; otherwise, false.
    /// </returns>
    public override bool Equals (object obj)
    {
      ClassContext other = obj as ClassContext;
      if (other == null)
        return false;
      
        if (!other.Type.Equals (Type) || other._mixins.Count != _mixins.Count || other._completeInterfaces.Count != _completeInterfaces.Count)
          return false;

        foreach (MixinContext mixinContext in _mixins.Values)
        {
          if (!other._mixins.ContainsKey (mixinContext.MixinType) || !other._mixins[mixinContext.MixinType].Equals (mixinContext))
            return false;
        }

        for (int i = 0; i < _completeInterfaces.Count; ++i)
        {
          if (!_completeInterfaces[i].Equals (other._completeInterfaces[i]))
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns a hash code for this <see cref="ClassContext"/>.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="ClassContext"/> which includes the hash codes of this object's complete interfaces and mixin ocntexts.
    /// </returns>
    public override int GetHashCode ()
    {
      return _cachedHashCode;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="ClassContext"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"></see> containing the type names of this context's associated <see cref="Type"/>, all its mixin types, and
    /// complete interfaces.
    /// </returns>
    public override string ToString ()
    {
      StringBuilder sb = new StringBuilder (Type.FullName);
      foreach (MixinContext mixinContext in Mixins)
        sb.Append (" + ").Append (mixinContext.MixinType.FullName);
      foreach (Type completeInterfaceType in CompleteInterfaces)
        sb.Append (" => ").Append (completeInterfaceType.FullName);
      return sb.ToString();
    }

    /// <summary>
    /// Returns a new <see cref="ClassContext"/> with the same mixins and complete interfaces as this object, but a different target type.
    /// </summary>
    /// <param name="type">The target type to create the new <see cref="ClassContext"/> for.</param>
    /// <returns>A clone of this <see cref="ClassContext"/> for a different target type.</returns>
    public ClassContext CloneForSpecificType (Type type)
    {
      ClassContext newInstance = new ClassContext (type, Mixins, CompleteInterfaces);
      return newInstance;
    }

    /// <summary>
    /// Creates a clone of the current class context, replacing its generic parameters with type arguments. This method is only allowed on
    /// class contexts representing a generic type definition.
    /// </summary>
    /// <param name="genericArguments">The type arguments to specialize this context's <see cref="Type"/> with.</param>
    /// <returns>A <see cref="ClassContext"/> which is identical to this one except its <see cref="Type"/> being specialized with the
    /// given <paramref name="genericArguments"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="genericArguments"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><see cref="Type"/> is not a generic type definition.</exception>
    public ClassContext SpecializeWithTypeArguments (Type[] genericArguments)
    {
      ArgumentUtility.CheckNotNull ("genericArguments", genericArguments);

      if (!Type.IsGenericTypeDefinition)
        throw new InvalidOperationException ("This method is only allowed on generic type definitions.");

      return CloneForSpecificType (Type.MakeGenericType (genericArguments));
    }

    /// <summary>
    /// Creates a new <see cref="ClassContext"/> inheriting all data from the given <paramref name="baseContext"/> and applying overriding rules for
    /// mixins and concrete interfaces already defined for this <see cref="ClassContext"/>.
    /// </summary>
    /// <param name="baseContext">The base context to inherit data from.</param>
    /// <returns>A new <see cref="ClassContext"/> combining the mixins of this object with those from the <paramref name="baseContext"/>.</returns>
    /// <exception cref="ConfigurationException">The <paramref name="baseContext"/> contains mixins whose base types or generic
    /// type definitions are already defined on this mixin. The derived context cannot have concrete mixins whose base types
    /// are defined on the parent context.
    /// </exception>
    public ClassContext InheritFrom (ClassContext baseContext)
    {
      ArgumentUtility.CheckNotNull ("baseContext", baseContext);
      return InheritFrom (new ClassContext[] {baseContext});
    }

    /// <summary>
    /// Creates a new <see cref="ClassContext"/> inheriting all data from the given <paramref name="baseContexts"/> and applying overriding rules for
    /// mixins and concrete interfaces already defined for this <see cref="ClassContext"/>.
    /// </summary>
    /// <param name="baseContexts">The base contexts to inherit data from.</param>
    /// <returns>A new <see cref="ClassContext"/> combining the mixins of this object with those from the <paramref name="baseContexts"/>.</returns>
    /// <exception cref="ConfigurationException">The <paramref name="baseContexts"/> contain mixins whose base types or generic
    /// type definitions are already defined on this mixin. The derived context cannot have concrete mixins whose base types
    /// are defined on the parent context.
    /// </exception>
    public ClassContext InheritFrom (IEnumerable<ClassContext> baseContexts)
    {
      ArgumentUtility.CheckNotNull ("baseContexts", baseContexts);

      List<MixinContext> mixins;
      List<Type> interfaces;
      mixins = new List<MixinContext> (Mixins);
      interfaces = new List<Type> (CompleteInterfaces);

      foreach (ClassContext baseContext in baseContexts)
        ApplyInheritance (baseContext, mixins, interfaces);

      return new ClassContext (Type, mixins, interfaces);
    }

    private void ApplyInheritance (ClassContext baseContext, List<MixinContext> mixins, List<Type> interfaces)
    {
      Tuple<MixinContext, MixinContext> overridden_override = GetFirstOverrideThatIsNotOverriddenByBase (mixins, baseContext.Mixins);
      if (overridden_override != null)
      {
        string message = string.Format (
            "The class {0} inherits the mixin {1} from class {2}, but it is explicitly "
                + "configured for the less specific mixin {3}.",
            Type.FullName,
            overridden_override.B.MixinType.FullName,
            baseContext.Type.FullName,
            overridden_override.A.MixinType);
        throw new ConfigurationException (message);
      }

      foreach (MixinContext inheritedMixin in baseContext.Mixins)
      {
        if (!ContainsOverrideForMixin (mixins, inheritedMixin.MixinType))
          mixins.Add (inheritedMixin);
      }

      foreach (Type inheritedInterface in baseContext.CompleteInterfaces)
      {
        if (!interfaces.Contains (inheritedInterface))
          interfaces.Add (inheritedInterface);
      }
    }

    /// <summary>
    /// Returns a new <see cref="ClassContext"/> equivalent to this object but with all mixins ascribable from the given
    /// <paramref name="mixinTypesToSuppress"/> removed.
    /// </summary>
    /// <param name="mixinTypesToSuppress">The mixin types to suppress.</param>
    /// <returns>A copy of this <see cref="ClassContext"/> without any mixins that can be ascribed to the given mixin types.</returns>
    public ClassContext SuppressMixins (IEnumerable<Type> mixinTypesToSuppress)
    {
      Dictionary<Type, MixinContext> mixins = new Dictionary<Type, MixinContext> (_mixins);
      foreach (Type suppressedType in mixinTypesToSuppress)
      {
        foreach (MixinContext mixin in Mixins)
        {
          if (Rubicon.Utilities.ReflectionUtility.CanAscribe (mixin.MixinType, suppressedType))
            mixins.Remove (mixin.MixinType);
        }
      }
      return new ClassContext (Type, mixins.Values, CompleteInterfaces);
    }
  }
}