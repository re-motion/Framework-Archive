using System;
using System.Collections;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Collections
{
  [Serializable]
  public class Hashtable<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>
    where TValue: class
  {
  }

  /// <summary>
  /// A strongly typed version of <see cref="Hashtable"/>.
  /// </summary>
  [Serializable]
  [Obsolete ("Use System.Collections.Generic.Dictionary<TKey, TValue> instead. (Note one major difference between Hashtable and Dictionary: Hashtable returns a null reference is a key is not found, while Dictionary throws an exception. Consider using Dictionary.TryGetValue where the Hashtable indexer is used now.")]
  public class TypedHashtable : Hashtable, ISerializable
  {
    private Type _keyType;
    private Type _valueType;

    public TypedHashtable (Type keyType, Type valueType)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (Type keyType, Type valueType, int capacity, float loadFactor, IHashCodeProvider hcp, IComparer comparer)
      : base (capacity, loadFactor, hcp, comparer)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (Type keyType, Type valueType, IDictionary d)
      : base (d)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (Type keyType, Type valueType, int capacity)
      : base (capacity)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (Type keyType, Type valueType, IDictionary d, float loadFactor)
      : base (d, loadFactor)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (Type keyType, Type valueType, IHashCodeProvider hcp, IComparer comparer)
      : base (hcp, comparer)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (Type keyType, Type valueType, int capacity, float loadFactor)
      : base (capacity, loadFactor)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (Type keyType, Type valueType, IDictionary d, IHashCodeProvider hcp, IComparer comparer)
      : base (d, hcp, comparer)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (Type keyType, Type valueType, int capacity, IHashCodeProvider hcp, IComparer comparer)
      : base (capacity, hcp, comparer)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (Type keyType, Type valueType, IDictionary d, float loadFactor, IHashCodeProvider hcp, IComparer comparer)
      : base (d, loadFactor, hcp, comparer)
    {
      Initialize (keyType, valueType);
    }

    public TypedHashtable (TypedHashtable hashtable)
      : base (hashtable.EqualityComparer)
    {
      hcp = hashtable.hcp;
      comparer = hashtable.comparer;
      Initialize (hashtable._keyType, hashtable._valueType);
      foreach (DictionaryEntry dict in hashtable)
        this.Add (dict.Key, dict.Value);
    }

    protected TypedHashtable (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
      _keyType = (Type) info.GetValue ("_keyType", typeof (Type));
      _valueType = (Type) info.GetValue ("_valueType", typeof (Type));
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);
      info.AddValue ("_keyType", _keyType);
      info.AddValue ("_valueType", _valueType);
    }

    private void Initialize (Type keyType, Type valueType)
    {
      ArgumentUtility.CheckNotNull ("keyType", keyType);
      ArgumentUtility.CheckNotNull ("valueType", valueType);
      _keyType = keyType;
      _valueType = valueType;
    }

    public override void Add (object key, object value)
    {
      ArgumentUtility.CheckNotNullAndType ("key", key, _keyType);
      ArgumentUtility.CheckType ("value", value, _valueType);
      base.Add (key, value);
    }

    public override object this[object key]
    {
      get { return base[key]; }
      set
      {
        ArgumentUtility.CheckType ("key", key, _keyType);
        ArgumentUtility.CheckType ("value", value, _valueType);
        base[key] = value;
      }
    }

    /// <summary>
    /// Gets a typed array containing the keys of the hashtable.
    /// </summary>
    public Array KeysArray
    {
      get
      {
        ArrayList arrayList = new ArrayList (Keys);
        return arrayList.ToArray (_keyType);
      }
    }

    /// <summary>
    /// Gets a typed array containing the values of the hashtable.
    /// </summary>
    public Array ValuesArray
    {
      get
      {
        ArrayList arrayList = new ArrayList (Values);
        return arrayList.ToArray (_valueType);
      }
    }

    public override object Clone ()
    {
      return new TypedHashtable (this);
    }

  }

}