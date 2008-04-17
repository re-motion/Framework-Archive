using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// A strongly typed version of <see cref="ArrayList"/>.
  /// </summary>
  [Serializable]
  [Obsolete ("Use System.Collections.Generic.List<T> instead.")]
  [DebuggerDisplay ("Count={Count}")]
  public class TypedArrayList : ArrayList, ISerializable
  {
    private Type _elementType;

    public TypedArrayList (Type elementType)
    {
      Initialize (elementType);
    }

    public TypedArrayList (Type elementType, int capacity)
        : base (capacity)
    {
      Initialize (elementType);
    }

    public TypedArrayList (Type elementType, ICollection collection)
        : base()
        // problem: if Constructor public TypedArrayList (Type elementType, ICollection collection) : base (collection)
        // is used, base (collection) calls AddRange. At this time, _elementType is still null, but AddRange checks Type
    {
      Initialize (elementType);
      AddRange (collection);
    }

    public TypedArrayList (TypedArrayList list)
    {
      _elementType = list._elementType;
      AddRange (list);
    }

    protected TypedArrayList (SerializationInfo info, StreamingContext context)
    {
      _elementType = (Type) info.GetValue ("_elementType", typeof (Type));
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("_elementType", _elementType);
    }

    private void Initialize (Type elementType)
    {
      ArgumentUtility.CheckNotNull ("elementType", elementType);
      _elementType = elementType;
    }

    public override int Add (object value)
    {
      ArgumentUtility.CheckNotNullAndType ("value", value, _elementType);
      return base.Add (value);
    }

    public override void AddRange (ICollection c)
    {
      ArgumentUtility.CheckNotNull ("c", c);
      ArgumentUtility.CheckItemsNotNullAndType ("c", c, _elementType);
      base.AddRange (c);
    }

    public override void Insert (int index, object value)
    {
      ArgumentUtility.CheckNotNullAndType ("value", value, _elementType);
      base.Insert (index, value);
    }

    public override void SetRange (int index, ICollection c)
    {
      ArgumentUtility.CheckItemsNotNullAndType ("c", c, _elementType);
      base.SetRange (index, c);
    }

    public override object this [int index]
    {
      get { return base[index]; }
      set
      {
        ArgumentUtility.CheckNotNullAndType ("value", value, _elementType);
        base[index] = value;
      }
    }

    public override object Clone ()
    {
      return new TypedArrayList (this);
    }

    /// <summary>
    ///   Copies the elements of the TypedArrayList to an array of the specified type.
    /// </summary>
    /// <returns></returns>
    public new Array ToArray ()
    {
      return base.ToArray (_elementType);
    }
  }
}