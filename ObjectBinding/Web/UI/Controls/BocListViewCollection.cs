using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{
 
/// <summary> A collection of <see cref="BocColumnDefinitionSet"/> objects. </summary>
[Editor (typeof (BocColumnDefinitionSetCollectionEditor), typeof (UITypeEditor))]
public sealed class BocColumnDefinitionSetCollection : IList, ICollection, IEnumerable
{
  /// <summary> <see langword="true"/> if <see cref="BeginEdit"/> was called. </summary>
  private bool _isEditing;
  /// <summary>
  ///   <see langword="true"/> if <see cref="_isEditing"/> <see langword="true"/> and the 
  ///   collection's values got changed.
  /// </summary>
  private bool _isChanged;
  /// <summary> The items contained on the collection. </summary>
  private ArrayList _items;
  /// <summary> 
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this collection belongs to.
  /// </summary>
  private IBusinessObjectBoundWebControl _ownerControl;

  /// <summary>
  ///   The event raised after the items contained in the collection have been changed.
  /// </summary>
  public event CollectionChangeEventHandler CollectionChanged;

  /// <summary>
  ///   Constructor.
  /// </summary>
  /// <param name="ownerControl">
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this collection belongs to.
  /// </param>
  /// <param name="columnDefinitionSets">
  ///   The <see cref="BocColumnDefinitionSet"/> objects to initialize the collection with.
  /// </param>
  internal BocColumnDefinitionSetCollection (
      IBusinessObjectBoundWebControl ownerControl, 
      BocColumnDefinitionSet[] columnDefinitionSets)
    : this (ownerControl)
  {
    AddRange (columnDefinitionSets);
  }

  /// <summary>
  ///   Constructor.
  /// </summary>
  /// <param name="ownerControl">
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this collection belongs to.
  /// </param>
  internal BocColumnDefinitionSetCollection (IBusinessObjectBoundWebControl ownerControl)
  {
    _ownerControl = ownerControl;
    _items = new ArrayList();
  }

  /// <summary> Places the collection into edit mode. </summary>
  /// <remarks> No individual <see cref="CollectionChanged"/> events are raised during edit mode. </remarks>
  public void BeginEdit()
  {
    _isEditing = true;
  }

  /// <summary> Returns the collection to normal mode. </summary>
  /// <remarks> A common <see cref="CollectionChanged"/> event is raised if changes 
  /// occured during edit-mode. </remarks>
  public void EndEdit()
  {
    if (_isEditing)
    {
      _isEditing = false;
      if (_isChanged)
      {
        _isChanged = false;
        OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Refresh, null));
      }
    }
  }

  /// <summary> Raises the <see cref="CollectionChanged"/> event. </summary>
  /// <remarks> Event is only raised if the collection is not in edit mode. </remarks>
  /// <param name="e"> The <see cref="EventArgs"/> to be passed to the event.</param>
  private void OnCollectionChanged (CollectionChangeEventArgs e)
  {
    if (CollectionChanged != null && !_isEditing)
      CollectionChanged(this, e);
  }

  /// <summary> Removes all items from the IList. </summary>
  public void Clear()
  {
    _items.Clear();
    _isChanged |= _isEditing;
   OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Refresh, null));
  }

  /// <summary> Determines whether the <see cref="IList"/> contains a specific value. </summary>
  /// <param name="value"> 
  ///   The <see cref="Object"/> to locate in the <see cref="IList"/>.
  /// </param>
  /// <returns> 
  ///   <see langword="true"/> if the <see cref="Object"/> is found in the <see cref="IList"/>;
  ///   otherwise, <see langword="false"/>.
  /// </returns>
  bool IList.Contains (object value)
  {
    return Contains ((BocColumnDefinitionSet) value);
  }

  /// <summary> Determines whether the <see cref="IList"/> contains a specific value. </summary>
  /// <param name="value"> 
  ///   The <see cref="BocColumnDefinitionSet"/> to locate in the <see cref="IList"/>. 
  /// </param>
  /// <returns> 
  ///   <see langword="true"/> if the <see cref="BocColumnDefinitionSet"/> is found in the 
  ///   <see cref="IList"/>; otherwise, <see langword="false"/>.
  /// </returns>
  public bool Contains (BocColumnDefinitionSet value)
  { 
    int index = IndexOf (value); 
    return (index != -1);
  }

  /// <summary> Returns an enumerator that can iterate through a collection. </summary>
  /// <returns> An <see cref="IEnumerator"/> that can be used to iterate through the collection.</returns>
  public IEnumerator GetEnumerator()
  {
    return _items.GetEnumerator();
  }

  /// <summary> Determines the index of a specific item in the <see cref="IList"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="Object"/> to locate in the <see cref="IList"/>. 
  /// </param>
  /// <returns> The index of <paramref name="value"/> if found in the list; otherwise, -1. </returns>
  int IList.IndexOf (object value)
  {
    return IndexOf ((BocColumnDefinitionSet) value);
  }

  /// <summary> Determines the index of a specific item in the <see cref="IList"/>. </summary>
  /// <param name="value">
  ///   The <see cref="BocColumnDefinitionSet"/> to locate in the <see cref="IList"/>. 
  /// </param>
  /// <returns> The index of <paramref name="value"/> if found in the list; otherwise, -1. </returns>
  public int IndexOf (BocColumnDefinitionSet value)
  { 
    return _items.IndexOf (value);
  }

  /// <summary> Adds an item of type <see cref="BocColumnDefinitionSet"/> to the <see cref="IList"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="BocColumnDefinitionSet"/> to add to the <see cref="IList"/>.
  /// </param>
  /// <returns> The position into which the new element was inserted. </returns>
  int IList.Add (object value)
  {
    ArgumentUtility.CheckType ("value", value, typeof (BocColumnDefinitionSet));
    return Add ((BocColumnDefinitionSet) value);
  }

  /// <summary> Adds an item to the <see cref="IList"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="BocColumnDefinitionSet"/> to add to the <see cref="IList"/>. 
  /// </param>
  /// <returns> The position into which the new element was inserted. </returns>
  public int Add (BocColumnDefinitionSet value)
  {
    int count = _items.Add (value);
    value.OwnerControl = _ownerControl;
    _isChanged |= _isEditing;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));
    return count;
  }

  /// <summary> Adds <see cref="BocColumnDefinitionSet"/> array to the <see cref="IList"/>. </summary>
  /// <param name="columnDefinitionSets"> 
  ///   The <see cref="BocColumnDefinitionSet"/> array to add to the <see cref="IList"/>. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  public void AddRange (BocColumnDefinitionSet[] columnDefinitionSets)
  {
    ArgumentUtility.CheckNotNull ("columnDefinitionSets", columnDefinitionSets);

    BeginEdit();
    foreach (BocColumnDefinitionSet columnDefinitionSet in columnDefinitionSets)
      Add (columnDefinitionSet);
    EndEdit();
  }

  /// <summary> Inserts an item to the <see cref="IList"/> at the specified position. </summary>
  /// <param name="index"> The zero-based index at which <paramref name="value"/> should be inserted. </param>
  /// <param name="value">
  ///   The <see cref="BocColumnDefinitionSet"/> to insert into the <see cref="IList"/>.
  /// </param>
  void IList.Insert(int index, object value)
  {
    ArgumentUtility.CheckType ("value", value, typeof (BocColumnDefinitionSet));
    Insert (index, (BocColumnDefinitionSet) value);
  }

  /// <summary> Inserts an item to the <see cref="IList"/> at the specified position. </summary>
  /// <param name="index"> The zero-based index at which <paramref name="value"/> should be inserted. </param>
  /// <param name="value">
  ///   The <see cref="BocColumnDefinitionSet"/> to insert into the <see cref="IList"/>.
  /// </param>
  void Insert(int index, BocColumnDefinitionSet value)
  {
    _items.Insert (index, value);
    value.OwnerControl = _ownerControl;
    _isChanged |= _isEditing;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));
  }

  /// <summary> Removes the first occurrence of a specific object from the <see cref="IList"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="BocColumnDefinitionSet"/> to remove from the <see cref="IList"/>. 
  /// </param>
  void IList.Remove (object value)
  { 
    ArgumentUtility.CheckType ("value", value, typeof (BocColumnDefinitionSet));
    Remove ((BocColumnDefinitionSet) value);
  }

  /// <summary> Removes the first occurrence of a specific object from the <see cref="IList"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="BocColumnDefinitionSet"/> to remove from the <see cref="IList"/>. 
  /// </param>
  public void Remove (BocColumnDefinitionSet value)
  {
    int removeAt = IndexOf (value);
    if (removeAt == -1) throw new InvalidOperationException("The passed BocColumnDefinitionSet is not part of this collection");
    RemoveAt(removeAt);
  }

  /// <summary> Removes the <see cref="IList"/> item at the specified index. </summary>
  /// <param name="index"> The zero-based index of the item to remove. </param>
  public void RemoveAt(int index)
  {
    BocColumnDefinitionSet columnDefinitionSet = (BocColumnDefinitionSet) _items[index];
    _items.RemoveAt(index);
    _isChanged |= _isEditing;
    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, columnDefinitionSet));
  }

  /// <summary>
  ///  Copies the elements of the <see cref="ICollection "/> to an <see cref="Array"/>, 
  ///  starting at a particular <c>Array</c> index.
  /// </summary>
  /// <param name="array">
  ///   The one-dimensional <see cref="Array"/> that is the destination of the elements copied 
  ///   from <see cref="ICollection"/>. The <c>Array</c> must have zero-based indexing.
  /// </param>
  /// <param name="index">
  ///   The zero-based index in <paramref name="array"/> at which copying begins. 
  /// </param>
  public void CopyTo (Array array, int index)
  {
    _items.CopyTo (array, index);
  }

  /// <summary> Copies the elements of the <see cref="IList"/> to a new array. </summary>
  /// <returns> 
  ///   An <see cref="BocColumnDefinitionSet"/> array containing the elements of the 
  ///   <see cref="IList"/>.
  /// </returns>
  public BocColumnDefinitionSet[] ToArray()
  {
    return (BocColumnDefinitionSet[]) _items.ToArray (typeof (BocColumnDefinitionSet));
  }

  /// <summary> Gets the element at the specified index. </summary>
  /// <value> The element at the specified index. </value>
  /// <exception cref="NotSupportedException"> Thrown if the property is set. </exception>
  object IList.this [int index]
  {
    get { return _items[index]; }
    set { throw new NotSupportedException(); }
  }

  /// <summary> Gets the element at the specified index. </summary>
  /// <value> The element at the specified index. </value>
  public BocColumnDefinitionSet this[int index]
  {
    get { return (BocColumnDefinitionSet) _items[index]; }
  }

  /// <summary> Gets the number of elements contained in the <see cref="ICollection"/>. </summary>
  /// <value> The number of elements contained in the <see cref="ICollection"/>. </value>
  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public int Count
  {
    get { return _items.Count; } 
  }

  /// <summary> Gets a value indicating whether the <see cref="IList"/> is read-only. </summary>
  /// <value> <see langword="false"/>, the <see cref="IList"/> is not read-only. </value>
  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public bool IsReadOnly
  { 
    get { return false; }
  }

  /// <summary>
  ///   Gets a value indicating whether access to the <see cref="ICollection "/> is synchronized 
  ///   (thread-safe).
  /// </summary>
  /// <value> <see langword="false"/>, access is not synchronized. </value>
  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public bool IsSynchronized
  { 
    get { return false; }
  }

  /// <summary> 
  ///   Gets an object that can be used to synchronize access to the <see cref="ICollection "/>.
  /// </summary>
  /// <value> An object that can be used to synchronize access to the <see cref="ICollection"/>. </value>
  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public object SyncRoot
  { 
    get { return _items.SyncRoot; } 
  }

  /// <summary> Gets a value indicating whether the <see cref="IList "/> has a fixed size. </summary>
  /// <value> <see langword="false"/>, the <see cref="IList"/> has no fixed size. </value>
  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public bool IsFixedSize
  {
    get { return false; }
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this collection belongs to.
  /// </summary>
  internal IBusinessObjectBoundWebControl OwnerControl
  {
    get { return _ownerControl; }
    set 
    {
      _ownerControl = value; 
      foreach (BocColumnDefinitionSet columnDefinitionSet in _items)
        columnDefinitionSet.OwnerControl = _ownerControl;
    }
  }
}

}
