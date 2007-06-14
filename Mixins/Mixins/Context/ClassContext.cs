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
    private readonly Dictionary<Type, MixinContext> _mixins;
    private readonly UncastableEnumerableWrapper<MixinContext> _mixinWrapperForOutside;
    private readonly List<Type> _completeInterfaces;
    private readonly UncastableEnumerableWrapper<Type> _completeInterfaceWrapperForOutside;
    private readonly object _lockObject = new object ();

    private bool _isFrozen = false;

    public ClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _type = type;
      _mixins = new Dictionary<Type, MixinContext>();
      _mixinWrapperForOutside = new UncastableEnumerableWrapper<MixinContext> (_mixins.Values);
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

    public Type Type
    {
      get { return _type; }
    }

    public IEnumerable<MixinContext> Mixins
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
        return _mixins.ContainsKey (mixinType);
      }
    }

    public void AddMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      lock (_lockObject)
      {
        EnsureNotFrozen();
        if (!ContainsMixin (mixinType))
        {
          MixinContext context = new MixinContext (this, mixinType, _lockObject);
          AddMixinContext (context);
        }
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

    internal void EnsureNotFrozen ()
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

    public override int GetHashCode ()
    {
      lock (_lockObject)
      {
        return Type.GetHashCode() ^ EqualityUtility.GetRotatedHashCode (_mixins.Values) ^ EqualityUtility.GetRotatedHashCode (_completeInterfaces);
      }
    }

    public ClassContext Clone ()
    {
      lock (_lockObject)
      {
        ClassContext newInstance = new ClassContext (Type);
        foreach (MixinContext mixinContext in Mixins)
          newInstance.AddMixinContext (mixinContext.Clone(newInstance, newInstance._lockObject));
        foreach (Type completeInterface in CompleteInterfaces)
          newInstance.AddCompleteInterface (completeInterface);
        return newInstance;
      }
    }

    object ICloneable.Clone ()
    {
      return Clone();
    }

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