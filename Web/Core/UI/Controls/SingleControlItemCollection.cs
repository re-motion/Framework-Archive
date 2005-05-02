using System;
using System.Collections;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

public class SingleControlItemCollectionEnumerator: IEnumerator
{
  private IControlItem _controlItem;
  bool _isMoved;
  bool _isEnd;

  internal SingleControlItemCollectionEnumerator (IControlItem controlItem)
  {
    _controlItem = controlItem;
    _isMoved = false;
    _isEnd = false;
  }

  public void Reset()
  {
    _isMoved = false;
    _isEnd = false;
  }

  public object Current
  {
    get
    {
      if (! _isMoved) throw new InvalidOperationException ("The enumerator is positioned before the first element.");
      if (_isEnd) throw new InvalidOperationException ("The enumerator is positioned after the last element.");
      return _controlItem;
    }
  }

  public bool MoveNext()
  {
    if (_isMoved)
      _isEnd = true;
    _isMoved = true;
    if (_controlItem == null)
      _isEnd = true;
    return ! _isEnd;
  }
}

public class SingleControlItemCollection: IList
{
  private Type[] _supportedTypes;
  private IControlItem _controlItem;

  /// <summary> Creates a new instance. </summary>
  /// <param name="supportedTypes"> Supported types must implement <see cref="IControlItem"/>. </param>
  public SingleControlItemCollection (IControlItem controlItem, Type[] supportedTypes)
  {
    _supportedTypes = supportedTypes;
    Item = controlItem;
  }

  public SingleControlItemCollection (Type[] supportedTypes)
    : this (null, supportedTypes)
  {
  }

  public IControlItem Item
  {
    get { return _controlItem; }
    set 
    {
      if (value != null && ! IsSupportedType (value)) 
        throw new ArgumentTypeException ("value", value.GetType());
      _controlItem = value;
    }
  }

  public int Add (IControlItem value)
  {
    Item = value;
    return 1;
  }

  /// <summary>Tests whether the specified control item's type is supported by the collection. </summary>
  private bool IsSupportedType (IControlItem controlItem)
  {
    Type controlItemType = controlItem.GetType();

    foreach (Type type in _supportedTypes)
    {
      if (type.IsAssignableFrom (controlItemType))
        return true;
    }
    
    return false;
  }

  void ICollection.CopyTo(Array array, int index)
  {
    throw new NotSupportedException();
  }

  int ICollection.Count 
  {
    get { return (_controlItem != null ? 1 : 0); }
  }

  bool ICollection.IsSynchronized 
  { 
    get { return true; }
  }

  object ICollection.SyncRoot
  { 
    get { return this; }
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
     return new SingleControlItemCollectionEnumerator (_controlItem);
  }

  int IList.Add (object value)
  {
    return Add ((IControlItem) value);
  }

  void IList.Clear()
  {
	  Item = null;
  }

  bool IList.Contains (object value)
  {
	  return Item == value;
  }

  int IList.IndexOf (object value)
  {
	  if (Item == value)
      return 0;
    else
      return -1;
  }

  void IList.Insert (int index, object value)
  {
    if (index > 0) throw new NotSupportedException ("Inserting an element above index 0 is not supported.");
    Item = (IControlItem) value;
  }

  bool IList.IsFixedSize
  {
	  get { return true; }
  }

  bool IList.IsReadOnly
  {
	  get { return false; }
  }

  void IList.Remove (object value)
  {
    if (Item == value)
      Item = null;
  }

  void IList.RemoveAt (int index)
  {
    if (index > 0) throw new NotSupportedException ("Removing an element above index 0 is not supported.");
    Item = null;
  }

  object IList.this[int index]
  {
	  get
	  {
      if (index > 0) throw new Exception("The method or operation is not implemented.");
      return Item;
	  }
	  set
	  {
      if (index > 0) throw new Exception("The method or operation is not implemented.");
      Item = (IControlItem) value;
	  }
  }
}

}
