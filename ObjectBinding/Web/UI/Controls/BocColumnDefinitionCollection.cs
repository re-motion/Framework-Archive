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
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this
  ///   <see cref="BocColumnDefinitionCollection"/> belongs.
  /// </summary>
  private IBusinessObjectBoundWebControl _ownerControl;

  /// <summary> 
  ///   The types derived from <see cref="BocColumnDefinition"/> which may be added 
  ///   to the <see cref="BocColumnDefinitionCollection"/>. 
  /// </summary>
  private Type[] _supportedTypes;

  /// <summary> 
  ///   Initializes a new instance of the <see cref="BocColumnDefinitionCollection"/> class
  ///   with the <see cref="IBusinessObjectBoundWebControl"/> to which it belongs and the
  ///   list of <see cref="BocColumnDefinition"/> derived types supported by the collection.
  ///  </summary>
  /// <param name="ownerControl">
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this collection belongs.
  /// </param>
  /// <param name="supportedTypes"> 
  ///   The types derived from <see cref="BocColumnDefinition"/> which may be added 
  ///   to the <see cref="BocColumnDefinitionCollection"/>. 
  /// </param>
  internal BocColumnDefinitionCollection (
      IBusinessObjectBoundWebControl ownerControl, 
      Type[] supportedTypes)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("supportedTypes", supportedTypes);
    foreach (Type type in supportedTypes)
    {
      if (! typeof (BocColumnDefinition).IsAssignableFrom (type))
        throw new ArgumentException ("Type '" + type.FullName + "' in 'supportedTypes' is not derived from BocColumnDefiniton.");
    }

    _supportedTypes = supportedTypes;
    // Do not use ownerControl for more than storing the reference inside the constructor
    _ownerControl = ownerControl;
  }

  /// <summary> 
  ///   Initializes a new instance of the <see cref="BocColumnDefinitionCollection"/> class
  ///   with the <see cref="IBusinessObjectBoundWebControl"/> to which it belongs.
  ///  </summary>
  ///  <remarks>
  ///   An instance initialized by this contructor supports all types derived from 
  ///   <see cref="BocColumnDefinition"/>.
  ///  </remarks>
  /// <param name="ownerControl">
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this 
  ///   <see cref="BocColumnDefinitionCollection"/> belongs.
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

  /// <summary> Gets or sets the element at the specified index. </summary>
  /// <value> The element at the specified index. </value>
  public BocColumnDefinition this[int index]
  {
    get { return (BocColumnDefinition) List[index]; }
    set { List[index] = value; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> to which this 
  ///   <see cref="BocColumnDefinitionCollection"/> belongs.
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
  /// <returns> <see langword="true"/> if the <see cref="BocColumnDefinition"/> is suppported. </returns>
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
