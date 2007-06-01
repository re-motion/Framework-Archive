using System;
using System.Collections;
using System.Collections.Generic;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.Utilities;
using Mixins.Utilities.Serialization;
using Mixins.Validation;
using Rubicon.Collections;
using Rubicon.Utilities;
using System.Runtime.Serialization;

namespace Mixins.Context
{
  [Serializable]
  public sealed class ClassContext : ISerializable, ICloneable
  {
    private static BaseClassDefinitionBuilder s_definitionBuilder = new BaseClassDefinitionBuilder();

    private readonly Type _type;
    private readonly List<Type> _mixins;
    private readonly UncastableEnumerableWrapper<Type> _mixinWrapperForOutside;
    private readonly object _lockObject = new object();

    private BaseClassDefinition _analyzedDefinition = null;

    public ClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _type = type;
      _mixins = new List<Type>();
      _mixinWrapperForOutside = new UncastableEnumerableWrapper<Type> (_mixins);
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
      int count = info.GetInt32 ("_mixins.Count");
      _mixins = new List<Type> (count);
      for (int i = 0; i < count; ++i)
        AddMixin (ReflectionObjectSerializer.DeserializeType ("_mixins[" + i + "]", info));
      _mixinWrapperForOutside = new UncastableEnumerableWrapper<Type> (_mixins);
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      lock (_lockObject)
      {
        ReflectionObjectSerializer.SerializeType (_type, "_type", info);
        info.AddValue ("_mixins.Count", _mixins.Count);
        for (int i = 0; i < _mixins.Count; ++i)
          ReflectionObjectSerializer.SerializeType (_mixins[i], "_mixins[" + i + "]", info);
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

    public bool IsFrozen
    {
      get
      {
        lock (_lockObject)
        {
          return _analyzedDefinition != null;
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

    private void EnsureNotFrozen ()
    {
      if (IsFrozen)
        throw new InvalidOperationException (string.Format ("The class context for {0} is frozen.", Type.FullName));
    }

    public BaseClassDefinition Analyze ()
    {
      lock (_lockObject)
      {
        if (_analyzedDefinition == null)
        {
          _analyzedDefinition = s_definitionBuilder.Build (this);
        }
        return _analyzedDefinition;
      }
    }

    public Type GetConcreteType ()
    {
      BaseClassDefinition definition = Analyze();
      DefaultValidationLog log = Validator.Validate (definition);
      if (log.GetNumberOfFailures () > 0 || log.GetNumberOfUnexpectedExceptions () > 0)
        throw new ValidationException (log);

      // TODO: Implement with caching
      return null;
    }

    public override bool Equals (object obj)
    {
      lock (_lockObject)
      {
        ClassContext other = obj as ClassContext;
        if (other == null || !other.Type.Equals (Type) || other._mixins.Count != _mixins.Count)
          return false;

        for (int i = 0; i < _mixins.Count; ++i)
        {
          if (!_mixins[i].Equals (other._mixins[i]))
            return false;
        }

        return true;
      }
    }

    public override int GetHashCode ()
    {
      lock (_lockObject)
      {
        return Type.GetHashCode() ^ EqualityUtility.GetRotatedHashCode (_mixins);
      }
    }

    public ClassContext Clone ()
    {
      ClassContext newInstance = new ClassContext (Type);
      foreach (Type mixin in Mixins)
        newInstance.AddMixin (mixin);
      return newInstance;
    }

    object ICloneable.Clone ()
    {
      return Clone();
    }
  }
}