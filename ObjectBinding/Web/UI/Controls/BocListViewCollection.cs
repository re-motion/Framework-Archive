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
public sealed class BocColumnDefinitionSetCollection : CollectionBase
{
  /// <summary> <see langword="true"/> if <see cref="BeginEdit"/> was called. </summary>
  private bool _isEditing;

  /// <summary>
  ///   <see langword="true"/> if <see cref="_isEditing"/> <see langword="true"/> and the 
  ///   collection's values got changed.
  /// </summary>
  private bool _isChanged;

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
  internal BocColumnDefinitionSetCollection (IBusinessObjectBoundWebControl ownerControl)
  {
    _ownerControl = ownerControl;
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

  /// <summary> Performs additional custom processes before inserting a new element. </summary>
  /// <param name="index"> The zero-based index at which to insert value. </param>
  /// <param name="value"> The new value of the element at index. </param>
  protected override void OnInsert(int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (BocColumnDefinitionSet));
    base.OnInsert (index, value);    
    ((BocColumnDefinitionSet) value).OwnerControl = _ownerControl;
    _isChanged |= _isEditing;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));
  }

  /// <summary> Performs additional custom processes before setting a value. </summary>
  /// <param name="index"> The zero-based index at which <paramref name="oldValue"/> can be found. </param>
  /// <param name="oldValue"> The value to replace with <paramref name="newValue"/>. </param>
  /// <param name="newValue"> The new value of the element at index. </param>
  protected override void OnSet(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (BocColumnDefinitionSet));
    base.OnSet (index, oldValue, newValue);    
    ((BocColumnDefinitionSet) newValue).OwnerControl = _ownerControl;
    _isChanged |= _isEditing;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Remove, oldValue));
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, newValue));
  }

  /// <summary> Adds an item to the <see cref="IList"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="BocColumnDefinitionSet"/> to add to the <see cref="IList"/>. 
  /// </param>
  /// <returns> The position into which the new element was inserted. </returns>
  public int Add (BocColumnDefinitionSet value)
  {
    return List.Add (value);
  }

  /// <summary> Adds the <see cref="BocColumnDefinitionSet"/> array to the <see cref="IList"/>. </summary>
  /// <param name="values"> 
  ///   The <see cref="BocColumnDefinitionSet"/> array to add to the <see cref="IList"/>. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  public void AddRange (BocColumnDefinitionSet[] values)
  {
    ArgumentUtility.CheckNotNull ("values", values);
    
    BeginEdit();
    foreach (BocColumnDefinitionSet columnDefinitionSet in values)
      Add (columnDefinitionSet);
    EndEdit();
  }

  /// <summary> Copies the elements of the <see cref="IList"/> to a new array. </summary>
  /// <returns> 
  ///   An <see cref="BocColumnDefinitionSet"/> array containing the elements of the 
  ///   <see cref="IList"/>.
  /// </returns>
  public BocColumnDefinitionSet[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (BocColumnDefinitionSet[]) arrayList.ToArray (typeof (BocColumnDefinitionSet));
  }

  /// <summary> Gets the element at the specified index. </summary>
  /// <value> The element at the specified index. </value>
  public BocColumnDefinitionSet this[int index]
  {
    get { return (BocColumnDefinitionSet) List[index]; }
    set { List[index] = value; }
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
      foreach (BocColumnDefinitionSet columnDefinitionSet in List)
        columnDefinitionSet.OwnerControl = _ownerControl;
    }
  }
}

}
