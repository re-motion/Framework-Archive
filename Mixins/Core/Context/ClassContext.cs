using System;
using System.Collections.Generic;
using Rubicon.Mixins;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.Utilities;
using Rubicon.Mixins.Utilities.Serialization;
using Rubicon.Utilities;
using System.Runtime.Serialization;
using System.Text;

namespace Rubicon.Mixins.Context
{
	/// <summary>
	/// Holds the mixin configuration information for a single mixin target class.
	/// </summary>
	/// <threadsafety static="true" instance="true"/>
  [Serializable]
  public sealed class ClassContext : ISerializable, ICloneable
  {
    private readonly Type _type;
    private readonly Dictionary<Type, MixinContext> _mixins;
    private readonly UncastableEnumerableWrapper<MixinContext> _mixinWrapperForOutside;
    private readonly List<Type> _completeInterfaces;
    private readonly UncastableEnumerableWrapper<Type> _completeInterfaceWrapperForOutside;
    private readonly object _lockObject = new object ();

    private bool _isFrozen = false;

		/// <summary>
		/// Initializes a new <see cref="ClassContext"/> for a given mixin target type.
		/// </summary>
		/// <param name="type">The mixin target type to be represented by this context.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
    public ClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _type = type;
      _mixins = new Dictionary<Type, MixinContext>();
      _mixinWrapperForOutside = new UncastableEnumerableWrapper<MixinContext> (_mixins.Values);
      _completeInterfaces = new List<Type>();
      _completeInterfaceWrapperForOutside = new UncastableEnumerableWrapper<Type> (_completeInterfaces);
    }

		/// <summary>
		/// Initializes a new <see cref="ClassContext"/> for a given mixin target type and initializes it to be associated with the given
		/// mixin types.
		/// </summary>
		/// <param name="type">The mixin target type to be represented by this context.</param>
		/// <param name="mixinTypes">The mixin types to be associated with this context.</param>
		/// <exception cref="ArgumentNullException">One of the parameters passed to this method is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="mixinTypes"/> parameter contains duplicates.</exception>
    public ClassContext (Type type, params Type[] mixinTypes)
        : this (type)
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);

			foreach (Type mixinType in mixinTypes)
			{
				try
				{
					AddMixin (mixinType);
				}
				catch (InvalidOperationException ex)
				{
					string message = string.Format ("The mixin type {0} was tried to be added twice.", mixinType.FullName);
					throw new ArgumentException (message, "mixinTypes", ex);
				}
			}
    }

    private ClassContext (SerializationInfo info, StreamingContext context)
    {
      _type = ReflectionObjectSerializer.DeserializeType ("_type", info);

      int mixinCount = info.GetInt32 ("_mixins.Count");
      _mixins = new Dictionary<Type, MixinContext> (mixinCount);
      for (int i = 0; i < mixinCount; ++i)
        AddMixinContext (MixinContext.DeserializeFromFlatStructure (this, _lockObject, "_mixins[" + i + "]", info));
      _mixinWrapperForOutside = new UncastableEnumerableWrapper<MixinContext> (_mixins.Values);

      int completeInterfaceCount = info.GetInt32 ("_completeInterfaces.Count");
      _completeInterfaces = new List<Type> (completeInterfaceCount);
      for (int i = 0; i < completeInterfaceCount; ++i)
        AddCompleteInterface (ReflectionObjectSerializer.DeserializeType ("_completeInterfaces[" + i + "]", info));
      _completeInterfaceWrapperForOutside = new UncastableEnumerableWrapper<Type> (_completeInterfaces);
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      lock (_lockObject)
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
        lock (_lockObject)
        {
          return _mixins.Count;
        }
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
        lock (_lockObject)
        {
          return _completeInterfaces.Count;
        }
      }
    }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ClassContext"/> is frozen.
		/// </summary>
		/// <value>True if this instance is frozen; otherwise, false.</value>
		/// <remarks>A <see cref="ClassContext"/> can be frozen by calling its <see cref="Freeze"/> method. Frozen contexts can be inspected,
		/// but they cannot be changed (e.g. by adding or modifying mixins or complete interfaces). <see cref="ClassContext">ClassContexts</see> are
		/// automatically frozen when a <see cref="BaseClassDefinition"/> is built from them via the <see cref="BaseClassDefinitionBuilder"/> or
		/// <see cref="BaseClassDefinitionCache"/> classes.</remarks>
    public bool IsFrozen
    {
      get
      {
        lock (_lockObject)
        {
          return _isFrozen;
        }
      }
    }

		/// <summary>
		/// Freezes this <see cref="ClassContext"/> and thus protects it and its contained <see cref="MixinContext">MixinContexts</see> against
		/// further modification.
		/// </summary>
		/// <remarks>A <see cref="ClassContext"/> can be frozen by calling its <see cref="Freeze"/> method. Frozen contexts can be inspected,
		/// but they cannot be changed (e.g. by adding or modifying mixins or complete interfaces). <see cref="ClassContext">ClassContexts</see> are
		/// automatically frozen when a <see cref="BaseClassDefinition"/> is built from them via the <see cref="BaseClassDefinitionBuilder"/> or
		/// <see cref="BaseClassDefinitionCache"/> classes.</remarks>
		public void Freeze ()
		{
			lock (_lockObject)
			{
				_isFrozen = true;
			}
		}

		internal void EnsureNotFrozen ()
		{
			if (IsFrozen)
				throw new InvalidOperationException (string.Format ("The class context for {0} is frozen.", Type.FullName));
		}

		/// <summary>
		/// Determines whether this <see cref="ClassContext"/> contains the specified mixin type.
		/// </summary>
		/// <param name="mixinType">The mixin type to check for.</param>
		/// <returns>
		/// True if the specified mixin type contains mixin; otherwise, false.
		/// </returns>
		/// <exception cref="ArgumentNullException">The <paramref name="mixinType"/> parameter is <see langword="null"/>.</exception>
    public bool ContainsMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      lock (_lockObject)
      {
        return _mixins.ContainsKey (mixinType);
      }
    }

		/// <summary>
		/// Associates a mixin type with this <see cref="ClassContext"/>.
		/// </summary>
		/// <param name="mixinType">The mixin type to be associated with this context.</param>
		/// <returns>A new <see cref="MixinContext"/> created for the given mixin type.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="mixinType"/> parameter is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">The <see cref="ClassContext"/> is frozen or the mixin has already been
		/// added to this context.</exception>
    public MixinContext AddMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      lock (_lockObject)
      {
        EnsureNotFrozen();
        if (!ContainsMixin (mixinType))
        {
          MixinContext context = new MixinContext (this, mixinType, _lockObject);
          AddMixinContext (context);
          return context;
        }
        else
          throw new InvalidOperationException ("Mixin " + mixinType.FullName + " already added to class " + Type.FullName + ".");
      }
    }

    private void AddMixinContext (MixinContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      lock (_lockObject)
      {
        if (ContainsMixin (context.MixinType))
          throw new InvalidOperationException ("The class context already contains a mixin context for type " + context.MixinType.FullName + ".");
        else
          _mixins.Add (context.MixinType, context);
      }
    }

		/// <summary>
		/// Gets the <see cref="MixinContext"/> for the given mixin type associated with this <see cref="ClassContext"/>, adding a new one if necessary.
		/// </summary>
		/// <param name="mixinType">The mixin type to retrieve a <see cref="MixinContext"/> for.</param>
		/// <returns>A <see cref="MixinContext"/> for the given mixin type associated with this <see cref="ClassContext"/>.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="mixinType"/> parameter is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">The <see cref="ClassContext"/> is frozen.</exception>
    public MixinContext GetOrAddMixinContext (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      lock (_lockObject)
      {
        if (!ContainsMixin (mixinType))
          AddMixin (mixinType);
        return _mixins[mixinType];
      }
    }

		/// <summary>
		/// Removes the given mixin from this <see cref="ClassContext"/>.
		/// </summary>
		/// <param name="mixinType">The mixin type to be removed.</param>
		/// <returns>True if a mixin context was removed for the given type, false if no such mixin context existed.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="mixinType"/> parameter is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">The <see cref="ClassContext"/> is frozen.</exception>
    public bool RemoveMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      lock (_lockObject)
      {
        EnsureNotFrozen();
        return _mixins.Remove (mixinType);
      }
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
      lock (_lockObject)
      {
        return _completeInterfaces.Contains (interfaceType);
      }
    }

		/// <summary>
		/// Adds the given type to this <see cref="ClassContext"/> as a complete interface.
		/// </summary>
		/// <param name="interfaceType">Type to add as a complete interface.</param>

    public void AddCompleteInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      lock (_lockObject)
      {
        EnsureNotFrozen();
        if (!ContainsCompleteInterface (interfaceType))
          _completeInterfaces.Add (interfaceType);
      }
    }

		/// <summary>
		/// Removes the given complete interface from this <see cref="ClassContext"/>.
		/// </summary>
		/// <param name="interfaceType">Type to be removed as a complete interface.</param>
		/// <returns>True if the type was successfully removed; false if it was not added as a complete interface.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> parameter is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">The <see cref="ClassContext"/> is frozen.</exception>
		public bool RemoveCompleteInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      lock (_lockObject)
      {
        EnsureNotFrozen();
        return _completeInterfaces.Remove (interfaceType);
      }
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
      
      lock (_lockObject)
      lock (other._lockObject)
      {
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
    }

		/// <summary>
		/// Returns a hash code for this <see cref="ClassContext"/>.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="ClassContext"/> which includes the hash codes of this object's complete interfaces and mixin ocntexts.
		/// </returns>
    public override int GetHashCode ()
    {
      lock (_lockObject)
      {
        return Type.GetHashCode() ^ EqualityUtility.GetRotatedHashCode (_mixins.Values) ^ EqualityUtility.GetRotatedHashCode (_completeInterfaces);
      }
    }

		/// <summary>
		/// Creates a deep clone of this instance.
		/// </summary>
		/// <returns>A new <see cref="ClassContext"/> holding equivalent configuration data as this <see cref="ClassContext"/> does.</returns>
    public ClassContext Clone ()
    {
      lock (_lockObject)
      {
        ClassContext newInstance = new ClassContext (Type);

				foreach (MixinContext mixinContext in Mixins)
					mixinContext.CloneAndAddTo (newInstance);

        foreach (Type completeInterface in CompleteInterfaces)
          newInstance.AddCompleteInterface (completeInterface);

				Assertion.DebugAssert (newInstance.Equals (this));

        return newInstance;
      }
    }

    object ICloneable.Clone ()
    {
      return Clone();
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
 }
}