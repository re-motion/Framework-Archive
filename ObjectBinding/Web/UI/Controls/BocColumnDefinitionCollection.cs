using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{
 
/// <summary> A collection of <see cref="BocColumnDefinition"/> objects. </summary>
[Editor (typeof (BocColumnDefinitionCollectionEditor), typeof (UITypeEditor))]
public sealed class BocColumnDefinitionCollection : CollectionBase
{
  /// <summary> 
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this collection belongs to.
  /// </summary>
  private IBusinessObjectBoundWebControl _ownerControl;

  /// <summary> The BocColumnDefinition types supported by this collection instance. </summary>
  private Type[] _supportedTypes;

  /// <summary>
  ///   Constructor.
  /// </summary>
  /// <param name="ownerControl">
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this collection belongs to.
  /// </param>
  /// <param name="supportedTypes"> 
  ///   The <see cref="BocColumnDefinition"/> types supported by this collection. 
  /// </param>
  internal BocColumnDefinitionCollection (
    IBusinessObjectBoundWebControl ownerControl, 
    Type[] supportedTypes)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("supportedTypes", supportedTypes);

    _ownerControl = ownerControl;
    _supportedTypes = supportedTypes;
  }

  /// <summary>
  ///   Constructor.
  /// </summary>
  /// <param name="ownerControl">
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this collection belongs to.
  /// </param>
  internal BocColumnDefinitionCollection (IBusinessObjectBoundWebControl ownerControl)
    : this (ownerControl, new Type[] {typeof (BocColumnDefinition)})
  {}

  /// <summary> Performs additional custom processes before inserting a new element. </summary>
  /// <param name="index"> The zero-based index at which to insert value. </param>
  /// <param name="value"> The new value of the element at index. </param>
  protected override void OnInsert(int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (BocColumnDefinition));
    BocColumnDefinition columnDefinition = (BocColumnDefinition) value;
    if (!IsSupportedType (columnDefinition))
      throw new ArgumentException ("The collection does not support a BocColumnDefinition of type '" + columnDefinition.GetType() + "'.", "value");
    base.OnInsert (index, columnDefinition);    
    columnDefinition.OwnerControl = _ownerControl;
  }

  /// <summary> Performs additional custom processes before setting a value. </summary>
  /// <param name="index"> The zero-based index at which <paramref name="oldValue"/> can be found. </param>
  /// <param name="oldValue"> The value to replace with <paramref name="newValue"/>. </param>
  /// <param name="newValue"> The new value of the element at index. </param>
  protected override void OnSet(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (BocColumnDefinition));
    BocColumnDefinition columnDefinition = (BocColumnDefinition) newValue;
    if (!IsSupportedType (columnDefinition))
      throw new ArgumentException ("The collection does not support a BocColumnDefinition of type '" + columnDefinition.GetType() + "'.", "newValue");
    base.OnSet (index, oldValue, columnDefinition);    
    columnDefinition.OwnerControl = _ownerControl;
  }

  /// <summary> Adds an item to the <see cref="IList"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="BocColumnDefinitionSet"/> to add to the <see cref="IList"/>. 
  /// </param>
  /// <returns> The position into which the new element was inserted. </returns>
  public int Add (BocColumnDefinition value)
  {
    return List.Add (value);
  }

  /// <summary> Adds the <see cref="BocColumnDefinition"/> array to the <see cref="IList"/>. </summary>
  /// <param name="values"> 
  ///   The <see cref="BocColumnDefinition"/> array to add to the <see cref="IList"/>. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  public void AddRange (BocColumnDefinition[] values)
  {
    ArgumentUtility.CheckNotNull ("values", values);
    
    foreach (BocColumnDefinition columnDefinition in values)
      Add (columnDefinition);
  }

  /// <summary> Copies the elements of the <see cref="IList"/> to a new array. </summary>
  /// <returns> 
  ///   An <see cref="BocColumnDefinition"/> array containing the elements of the 
  ///   <see cref="IList"/>.
  /// </returns>
  public BocColumnDefinition[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (BocColumnDefinition[]) arrayList.ToArray (typeof (BocColumnDefinition));
  }

  /// <summary> Gets the element at the specified index. </summary>
  /// <value> The element at the specified index. </value>
  public BocColumnDefinition this[int index]
  {
    get { return (BocColumnDefinition) List[index]; }
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
      foreach (BocColumnDefinition columnDefinition in List)
        columnDefinition.OwnerControl = _ownerControl;
    }
  }

  /// <summary>
  ///   Tests whether the specified <see cref="BocColumnDefinition"/>'s type is supported by the
  ///   collection.
  /// </summary>
  /// <param name="columnDefinition"> The <see cref="BocColumnDefinition"/> to be tested. </param>
  /// <returns><see langword="true"/> if the <see cref="BocColumnDefinition"/> is suppported. </returns>
  private bool IsSupportedType (BocColumnDefinition columnDefinition)
  {
    Type columnDefinitionType = columnDefinition.GetType();

    foreach (Type type in _supportedTypes)
    {
      if (type.IsAssignableFrom (columnDefinitionType))
        return true;
    }
    
    return false;
  }
}

}
