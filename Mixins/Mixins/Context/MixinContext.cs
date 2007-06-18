using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mixins.Utilities;
using Mixins.Utilities.Serialization;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Mixins.Context
{
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

    public override int GetHashCode ()
    {
      lock (_lockObject)
      {
        return MixinType.GetHashCode() ^ EqualityUtility.GetRotatedHashCode (ExplicitDependencies);
      }
    }

    internal MixinContext Clone (ClassContext classContext, object lockObject)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("lockObject", lockObject);

      lock (_lockObject)
      {
        MixinContext clone = new MixinContext (classContext, MixinType, lockObject);
        foreach (Type explicitDependency in ExplicitDependencies)
          clone.AddExplicitDependency (explicitDependency);
        return clone;
      }
    }

    public MixinContext CloneAndAddTo (ClassContext targetForClone)
    {
      ArgumentUtility.CheckNotNull ("targetForClone", targetForClone);
      MixinContext clone = targetForClone.AddMixin (MixinType);
      foreach (Type dependency in ExplicitDependencies)
        clone.AddExplicitDependency (dependency);
      return clone;
    }

    public int ExplicitDependencyCount
    {
      get {
        lock (_lockObject)
        {
          return _explicitDependencies.Count;
        }
      }
    }

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

    public bool ContainsExplicitDependency (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      lock (_lockObject)
      {
        return _explicitDependencies.Contains (interfaceType);
      }
    }

    public void AddExplicitDependency (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      lock (_lockObject)
      {
        _classContext.EnsureNotFrozen ();
        _explicitDependencies.Add (interfaceType);
      }
    }

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
