using System;
using System.Collections;
using System.Collections.Generic;
using Mixins.Utilities;
using Mixins.Utilities.Serialization;
using Rubicon.Collections;
using Rubicon.Utilities;
using System.Runtime.Serialization;

namespace Mixins.Context
{
  [Serializable]
  public class ClassContext : ISerializable
  {
    private Type _type;
    private List<Type> _mixins;
    private Set<Type> _mixinsForFastLookup;

    public ClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _type = type;
      _mixins = new List<Type>();
      _mixinsForFastLookup = new Set<Type>();
    }

    private ClassContext (SerializationInfo info, StreamingContext context)
    {
      _type = ReflectionObjectSerializer.DeserializeType ("_type", info);
      int count = info.GetInt32 ("_mixins.Count");
      _mixins = new List<Type> (count);
      _mixinsForFastLookup = new Set<Type>();
      for (int i = 0; i < count; ++i)
        AddMixin (ReflectionObjectSerializer.DeserializeType ("_mixins[" + i + "]", info));
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      ReflectionObjectSerializer.SerializeType (_type, "_type", info);
      info.AddValue ("_mixins.Count", _mixins.Count);
      for (int i = 0; i < _mixins.Count; ++i)
        ReflectionObjectSerializer.SerializeType (_mixins[i], "_mixins[" + i + "]", info);
    }

    public Type Type
    {
      get { return _type; }
    }

    public IEnumerable<Type> Mixins
    {
      get { return _mixins; }
    }

    public int MixinCount
    {
      get { return _mixins.Count; }
    }

    public bool ContainsMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      return _mixinsForFastLookup.Contains (mixinType);
    }

    public void AddMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      if (!ContainsMixin (mixinType))
      {
        _mixins.Add (mixinType);
        _mixinsForFastLookup.Add (mixinType);
      }
    }

    public override bool Equals (object obj)
    {
      ClassContext other = obj as ClassContext;
      if (other == null || !other.Type.Equals (Type) || other._mixins.Count != _mixins.Count)
        return false;

      for (int i = 0; i < _mixins.Count; ++i)
        if (!_mixins[i].Equals (other._mixins[i]))
          return false;

      return true;
    }

    public override int GetHashCode ()
    {
      return Type.GetHashCode() ^ EqualityUtility.GetRotatedHashCode (_mixins);
    }
  }
}