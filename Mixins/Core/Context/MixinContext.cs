using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Rubicon.Mixins.Utilities;
using Rubicon.Mixins.Utilities.Serialization;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Mixins.Context
{
	/// <summary>
	/// Represents a single mixin applied to a target class.
	/// </summary>
	/// <threadsafety static="true" instance="true">
	/// <para>The instance methods of this class are synchronized with the instance methods of the associated <see cref="ClassContext"/>.</para>
	/// </threadsafety>
	/// <remarks>Instances of this class are not created directly, but via <see cref="ClassContext.AddMixin"/> and
	/// <see cref="ClassContext.GetOrAddMixinContext"/>.</remarks>
  public class MixinContext
  {
    internal static MixinContext DeserializeFromFlatStructure (ClassContext classContext, object lockObject, string key, SerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("lockObject", lockObject);
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("info", info);

      Type mixinType = ReflectionObjectSerializer.DeserializeType (key + ".MixinType", info);
      MixinContext newContext = new MixinContext (classContext, mixinType, lockObject);

      int dependencyCount = info.GetInt32 (key + ".ExplicitDependencyCount");
      for (int i = 0; i < dependencyCount; ++i)
        newContext.AddExplicitDependency (ReflectionObjectSerializer.DeserializeType (key + ".ExplicitDependencies[" + i + "]", info));

      return newContext;
    }

    private ClassContext _classContext;
    public readonly Type MixinType;
    private Set<Type> _explicitDependencies;
    private UncastableEnumerableWrapper<Type> _explicitDependenciesForOutside;
    private object _lockObject;
    
    internal MixinContext (ClassContext classContext, Type mixinType, object lockObject)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("lockObject", lockObject);

      _classContext = classContext;
      MixinType = mixinType;
      _explicitDependencies = new Set<Type> ();
      _explicitDependenciesForOutside = new UncastableEnumerableWrapper<Type> (_explicitDependencies);
      _lockObject = lockObject;
    }

    internal void SerializeIntoFlatStructure (string key, SerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("info", info);

      lock (_lockObject)
      {
        ReflectionObjectSerializer.SerializeType (MixinType, key + ".MixinType", info);
        info.AddValue (key + ".ExplicitDependencyCount", ExplicitDependencyCount);
        IEnumerator<Type> dependencyEnumerator = ExplicitDependencies.GetEnumerator();
        for (int i = 0; dependencyEnumerator.MoveNext(); ++i)
          ReflectionObjectSerializer.SerializeType (dependencyEnumerator.Current, key + ".ExplicitDependencies[" + i + "]", info);
      }
    }

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="MixinContext"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"></see> to compare with this <see cref="MixinContext"/>.</param>
		/// <returns>
		/// True if the specified <see cref="T:System.Object"></see> is a <see cref="MixinContext"/> for the same mixin type with equal explicit
		/// dependencies; otherwise, false.
		/// </returns>
    public override bool Equals (object obj)
    {
      MixinContext other = obj as MixinContext;
      if (other == null)
        return false;
      
      lock (_lockObject)
      lock (other._lockObject)
      {
        if (!other.MixinType.Equals (this.MixinType) || other.ExplicitDependencyCount != this.ExplicitDependencyCount)
          return false;

        foreach (Type explicitDependency in ExplicitDependencies)
          if (!other.ContainsExplicitDependency (explicitDependency))
            return false;
        return true;
      }
    }

		/// <summary>
		/// Returns a hash code for this <see cref="MixinContext"/>.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="MixinContext"/> which includes the hash codes of its mixin type and its explicit dependencies.
		/// </returns>
    public override int GetHashCode ()
    {
      lock (_lockObject)
      {
        return MixinType.GetHashCode() ^ EqualityUtility.GetRotatedHashCode (ExplicitDependencies);
      }
    }

		/// <summary>
		/// Adds a copy of this <see cref="MixinContext"/> to another <see cref="ClassContext"/>.
		/// </summary>
		/// <param name="targetForClone">The target <see cref="ClassContext"/> to add the new <see cref="MixinContext"/> to.</param>
		/// <returns>The newly created context added to the target <see cref="ClassContext"/> and holding equivalent configuration data to
		/// this <see cref="MixinContext"/>.</returns>
    public MixinContext CloneAndAddTo (ClassContext targetForClone)
    {
      ArgumentUtility.CheckNotNull ("targetForClone", targetForClone);
      MixinContext clone = targetForClone.AddMixin (MixinType);
      foreach (Type dependency in ExplicitDependencies)
        clone.AddExplicitDependency (dependency);
			Assertion.DebugAssert (clone.Equals (this));
      return clone;
    }

		/// <summary>
		/// Gets the number of explicit dependencies added to this <see cref="MixinContext"/>.
		/// </summary>
		/// <value>The explicit dependency count.</value>
		/// <remarks>An explicit dependency is a base call dependency which should be considered for a mixin even though it is not expressed in the
		/// mixin's class declaration. This can be used to define the ordering of mixins in specific mixin configurations.</remarks>
    public int ExplicitDependencyCount
    {
      get {
        lock (_lockObject)
        {
          return _explicitDependencies.Count;
        }
      }
    }

		/// <summary>
		/// Gets the explicit dependencies added to this <see cref="MixinContext"/>.
		/// </summary>
		/// <value>The explicit dependencies added to this <see cref="MixinContext"/>.</value>
		/// <remarks>An explicit dependency is a base call dependency which should be considered for a mixin even though it is not expressed in the
		/// mixin's class declaration. This can be used to define the ordering of mixins in specific mixin configurations.</remarks>
    public IEnumerable<Type> ExplicitDependencies
    {
      get
      {
        lock (_lockObject)
        {
          return _explicitDependenciesForOutside;
        }
      }
    }

		/// <summary>
		/// Determines whether this <see cref="MixinContext"/> contains the given explicit dependency.
		/// </summary>
		/// <param name="interfaceType">The dependency type to check for.</param>
		/// <returns>
		/// True if this <see cref="MixinContext"/> contains the given explicit dependency; otherwise, false.
		/// </returns>
		/// <remarks>An explicit dependency is a base call dependency which should be considered for a mixin even though it is not expressed in the
		/// mixin's class declaration. This can be used to define the ordering of mixins in specific mixin configurations.</remarks>
		/// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> parameter is <see langword="null"/>.</exception>
    public bool ContainsExplicitDependency (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      lock (_lockObject)
      {
        return _explicitDependencies.Contains (interfaceType);
      }
    }

		/// <summary>
		/// Adds the given type as an explicit dependency unless it has already been added to this <see cref="MixinContext"/>.
		/// </summary>
		/// <param name="interfaceType">Interface type for the explicit dependency to add.</param>
		/// <remarks>An explicit dependency is a base call dependency which should be considered for a mixin even though it is not expressed in the
		/// mixin's class declaration. This can be used to define the ordering of mixins in specific mixin configurations.</remarks>
		/// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> parameter is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">The associated <see cref="ClassContext"/> is frozen.</exception>
		public void AddExplicitDependency (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      lock (_lockObject)
      {
        _classContext.EnsureNotFrozen ();
        _explicitDependencies.Add (interfaceType);
      }
    }

		/// <summary>
		/// Removes the given type from the set of explicit dependencies of this <see cref="MixinContext"/>.
		/// </summary>
		/// <param name="interfaceType">Interface type of the dependency to remove.</param>
		/// <returns>True if the dependency was successfully removed; false if the type wasn't added as a dependency.</returns>
		/// <remarks>An explicit dependency is a base call dependency which should be considered for a mixin even though it is not expressed in the
		/// mixin's class declaration. This can be used to define the ordering of mixins in specific mixin configurations.</remarks>
		/// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> parameter is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">The associated <see cref="ClassContext"/> is frozen.</exception>
    public bool RemoveExplicitDependency (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      lock (_lockObject)
      {
        _classContext.EnsureNotFrozen ();
        return _explicitDependencies.Remove (interfaceType);
      }
    }
  }
}
