using System;
using System.ComponentModel;
using System.Collections;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

public interface IControlItem
{
  Control OwnerControl { get; set; }
}

public class ControlItemCollection: CollectionBase
{
  private Control _ownerControl;
  private Type[] _supportedTypes;
  /// <summary> true if BeginEdit was called. </summary>
  private bool _isEditing;
  /// <summary> true if _isEditing is true and the collection's values got changed. </summary>
  private bool _isChanged;

  /// <summary> The event raised after the items contained in the collection has been changed. </summary>
  public event CollectionChangeEventHandler CollectionChanged;

  /// <summary> Creates a new instance. </summary>
  /// <param name="ownerControl"> Owner control. </param>
  /// <param name="supportedTypes"> Supported types must implement <see cref="IControlItem"/>. </param>
  public ControlItemCollection (Control ownerControl, Type[] supportedTypes)
  {
    _ownerControl = ownerControl;
    _supportedTypes = supportedTypes;
  }

  /// <summary> Creates a new instance. </summary>
  /// <param name="ownerControl"> Owner control. </param>
  /// <param name="supportedTypes"> Supported types must implement <see cref="IControlItem"/>. </param>
  public ControlItemCollection (IControl ownerControl, Type[] supportedTypes)
  {
    _ownerControl = (Control) ownerControl;
    _supportedTypes = supportedTypes;
  }

  /// <summary> Places the collection into edit mode. </summary>
  /// <remarks> No individual <see cref="CollectionChanged"/> events are raised during edit mode. </remarks>
  public void BeginEdit()
  {
    _isEditing = true;
  }

  /// <summary> Returns the collection to normal mode. </summary>
  /// <remarks> A common <see cref="CollectionChanged"/> event is raised if changes occured during edit-mode. </remarks>
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
  private void OnCollectionChanged (CollectionChangeEventArgs e)
  {
    if (CollectionChanged != null && !_isEditing)
      CollectionChanged(this, e);
  }

  protected override void OnInsert (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (IControlItem));
    IControlItem controlItem = (IControlItem) value;
    if (! IsSupportedType (controlItem)) throw new ArgumentTypeException ("value", controlItem.GetType());

    base.OnInsert (index, value);
    controlItem.OwnerControl = _ownerControl;
    _isChanged |= _isEditing;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));
  }

  protected override void OnSet (int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (IControlItem));
    IControlItem controlItem = (IControlItem) newValue;
    if (! IsSupportedType (controlItem)) throw new ArgumentTypeException ("newValue", controlItem.GetType());

    base.OnSet (index, oldValue, newValue);
    controlItem.OwnerControl = _ownerControl;
    _isChanged |= _isEditing;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Remove, oldValue));
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, newValue));
  }

  public int Add (IControlItem value)
  {
    return List.Add (value);
  }

  public void AddRange (IControlItem[] values)
  {
    ArgumentUtility.CheckNotNull ("values", values);

    foreach (IControlItem controlItem in values)
      Add (controlItem );
  }

  public IControlItem[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (IControlItem[]) arrayList.ToArray (typeof (IControlItem));
  }

  public IControlItem this[int index]
  {
    get { return (IControlItem) List[index]; }
    set { List[index] = value; }
  }

  /// <summary> Gets or sets the control to which this collection belongs. </summary>
  public Control OwnerControl
  {
    get { return _ownerControl; }
    set 
    {
      _ownerControl = value; 
      foreach (IControlItem controlItem in List)
        controlItem.OwnerControl = _ownerControl;
    }
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
}

}
