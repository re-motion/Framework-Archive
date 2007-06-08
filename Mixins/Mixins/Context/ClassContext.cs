using System;
using System.Collections.Generic;
using Mixins.Utilities;
using Mixins.Utilities.Serialization;
using Mixins.Validation;
using Rubicon.Utilities;
using System.Runtime.Serialization;
using System.Text;

namespace Mixins.Context
{
  [Serializable]
  public sealed class ClassContext : ISerializable, ICloneable
  {
    private readonly Type _type;
    private readonly List<Type> _mixins;
    private readonly UncastableEnumerableWrapper<Type> _mixinWrapperForOutside;
    private readonly List<Type> _completeInterfaces;
    private readonly UncastableEnumerableWrapper<Type> _completeInterfaceWrapperForOutside;
    private readonly object _lockObject = new object ();

    private bool _isFrozen = false;

    public ClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _type = type;
      _mixins = new List<Type>();
      _mixinWrapperForOutside = new UncastableEnumerableWrapper<Type> (_mixins);
      _completeInterfaces = new List<Type>();
      _completeInterfaceWrapperForOutside = new UncastableEnumerableWrapper<Type> (_completeInterfaces);
    }

    public ClassContext (Type type, params Type[] mixinTypes)
        : this (type)
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);
      foreach (Type mixinType in mixinTypes)
        AddMixin (mixinType);
    }

    private ClassContext (SerializationInfo info, StreamingContext context)
    {
      _type = ReflectionObjectSerializer.DeserializeType ("_type", info);

      int mixinCount = info.GetInt32 ("_mixins.Count");
      _mixins = new List<Type> (mixinCount);
      for (int i = 0; i < mixinCount; ++i)
        AddMixin (ReflectionObjectSerializer.DeserializeType ("_mixins[" + i + "]", info));
      _mixinWrapperForOutside = new UncastableEnumerableWrapper<Type> (_mixins);

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
        for (int i = 0; i < _mixins.Count; ++i)
          ReflectionObjectSerializer.SerializeType (_mixins[i], "_mixins[" + i + "]", info);
        info.AddValue ("_completeInterfaces.Count", _completeInterfaces.Count);
        for (int i = 0; i < _completeInterfaces.Count; ++i)
          ReflectionObjectSerializer.SerializeType (_completeInterfaces[i], "_completeInterfaces[" + i + "]", info);
      }
    }

    public Type Type
    {
      get { return _type; }
    }

    public IEnumerable<Type> Mixins
    {
      get { return _mixinWrapperForOutside; }
    }

    public IEnumerable<Type> CompleteInterfaces
    {
      get { return _completeInterfaceWrapperForOutside; }
    }

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

    public bool ContainsMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      lock (_lockObject)
      {
        return _mixins.Contains (mixinType);
      }
    }

    public void AddMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      lock (_lockObject)
      {
        EnsureNotFrozen();
        if (!ContainsMixin (mixinType))
          _mixins.Add (mixinType);
      }
    }

    public bool RemoveMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      lock (_lockObject)
      {
        EnsureNotFrozen();
        return _mixins.Remove (mixinType);
      }
    }

    public bool ContainsCompleteInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      lock (_lockObject)
      {
        return _completeInterfaces.Contains (interfaceType);
      }
    }

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

    public bool RemoveCompleteInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      lock (_lockObject)
      {
        EnsureNotFrozen();
        return _completeInterfaces.Remove (interfaceType);
      }
    }

    
    public void Freeze()
    {
      lock (_lockObject)
      {
        _isFrozen = true;
      }
    }

    private void EnsureNotFrozen ()
    {
      if (IsFrozen)
        throw new InvalidOperationException (string.Format ("The class context for {0} is frozen.", Type.FullName));
    }

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

        for (int i = 0; i < _mixins.Count; ++i)
        {
          if (!_mixins[i].Equals (other._mixins[i]))
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

    public override int GetHashCode ()
    {
      lock (_lockObject)
      {
        return Type.GetHashCode() ^ EqualityUtility.GetRotatedHashCode (_mixins) ^ EqualityUtility.GetRotatedHashCode (_completeInterfaces);
      }
    }

    public ClassContext Clone ()
    {
      ClassContext newInstance = new ClassContext (Type);
      foreach (Type mixin in Mixins)
        newInstance.AddMixin (mixin);
      foreach (Type completeInterface in CompleteInterfaces)
        newInstance.AddCompleteInterface (completeInterface);
      return newInstance;
    }

    object ICloneable.Clone ()
    {
      return Clone();
    }

    public override string ToString ()
    {
      StringBuilder sb = new StringBuilder (Type.FullName);
      foreach (Type mixinType in Mixins)
        sb.Append (" + ").Append (mixinType.FullName);
      foreach (Type completeInterfaceType in CompleteInterfaces)
        sb.Append (" => ").Append (completeInterfaceType.FullName);
      return sb.ToString();
    }
  }
}