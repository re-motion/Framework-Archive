using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Controls
{
 
/// <summary> A collection of <see cref="MenuItem"/> objects. </summary>
[Editor (typeof (MenuItemCollectionEditor), typeof (UITypeEditor))]
public class MenuItemCollection : CollectionBase
{
  /// <summary> 
  ///   The <see cref="IControl"/> to which this <see cref="MenuItemCollection"/> belongs.
  /// </summary>
  private IControl _ownerControl;

  /// <summary> 
  ///   The types derived from <see cref="MenuItem"/> which may be added 
  ///   to the <see cref="MenuItemCollection"/>. 
  /// </summary>
  private Type[] _supportedTypes;

  /// <summary> 
  ///   Initializes a new instance of the <see cref="MenuItemCollection"/> class with the 
  ///   <see cref="IControl"/> to which it belongs and the list of <see cref="MenuItem"/> derived types 
  ///   supported by the collection.
  ///  </summary>
  /// <param name="ownerControl">
  ///   The <see cref="IControl"/> to which this collection belongs.
  /// </param>
  /// <param name="supportedTypes"> 
  ///   The types derived from <see cref="MenuItem"/> which may be added to the 
  ///   <see cref="MenuItemCollection"/>. 
  /// </param>
  internal MenuItemCollection (IControl ownerControl, Type[] supportedTypes)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("supportedTypes", supportedTypes);
    foreach (Type type in supportedTypes)
    {
      if (! typeof (MenuItem).IsAssignableFrom (type))
        throw new ArgumentException ("Type '" + type.FullName + "' in 'supportedTypes' is not derived from BocColumnDefiniton.");
    }

    _supportedTypes = supportedTypes;
    // Do not use ownerControl for more than storing the reference inside the constructor
    _ownerControl = ownerControl;
  }

  /// <summary> 
  ///   Initializes a new instance of the <see cref="MenuItemCollection"/> class with the 
  ///   <see cref="IControl"/> to which it belongs.
  ///  </summary>
  ///  <remarks>
  ///   An instance initialized by this contructor supports all types derived from <see cref="MenuItem"/>.
  ///  </remarks>
  /// <param name="ownerControl">
  ///   The <see cref="IControl"/> to which this <see cref="MenuItemCollection"/> belongs.
  /// </param>
  internal MenuItemCollection (IControl ownerControl)
    : this (ownerControl, new Type[] {typeof (MenuItem)})
  {
  }

  /// <summary> Performs additional custom processes before inserting a new element. </summary>
  /// <param name="index"> The zero-based index at which to insert value. </param>
  /// <param name="value"> The new value of the element at index. </param>
  protected override void OnInsert (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (MenuItem));
    MenuItem menuItem = (MenuItem) value;
    if (!IsSupportedType (menuItem))
      throw new ArgumentException ("The collection does not support a MenuItem of type '" + menuItem.GetType() + "'.", "value");
    base.OnInsert (index, menuItem);    
    menuItem.OwnerControl = _ownerControl;
  }

  /// <summary> Performs additional custom processes before setting a value. </summary>
  /// <param name="index"> The zero-based index at which <paramref name="oldValue"/> can be found. </param>
  /// <param name="oldValue"> The value to replace with <paramref name="newValue"/>. </param>
  /// <param name="newValue"> The new value of the element at index. </param>
  protected override void OnSet (int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (MenuItem));
    MenuItem menuItem = (MenuItem) newValue;
    if (!IsSupportedType (menuItem))
      throw new ArgumentException ("The collection does not support a MenuItem of type '" + menuItem.GetType() + "'.", "newValue");
    base.OnSet (index, oldValue, menuItem);    
    menuItem.OwnerControl = _ownerControl;
  }

  /// <summary> Adds an item to the <see cref="IList"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="MenuItem"/> to add to the <see cref="IList"/>. 
  /// </param>
  /// <returns> The position into which the new element was inserted. </returns>
  public int Add (MenuItem value)
  {
    return List.Add (value);
  }

  /// <summary> Adds the <see cref="MenuItem"/> array to the <see cref="IList"/>. </summary>
  /// <param name="values"> 
  ///   The <see cref="MenuItem"/> array to add to the <see cref="IList"/>. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  public void AddRange (MenuItem[] values)
  {
    ArgumentUtility.CheckNotNull ("values", values);
    
    foreach (MenuItem menuItem in values)
      Add (menuItem);
  }

  /// <summary> Copies the elements of the <see cref="IList"/> to a new array. </summary>
  /// <returns> 
  ///   An <see cref="MenuItem"/> array containing the elements of the 
  ///   <see cref="IList"/>.
  /// </returns>
  public MenuItem[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (MenuItem[]) arrayList.ToArray (typeof (MenuItem));
  }

  /// <summary> Gets or sets the element at the specified index. </summary>
  /// <value> The element at the specified index. </value>
  public MenuItem this[int index]
  {
    get { return (MenuItem) List[index]; }
    set { List[index] = value; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> to which this 
  ///   <see cref="BocColumnDefinitionCollection"/> belongs.
  /// </summary>
  internal IControl OwnerControl
  {
    get { return _ownerControl; }
    set 
    {
      _ownerControl = value; 
      foreach (MenuItem menuItem in List)
        menuItem.OwnerControl = _ownerControl;
    }
  }

  /// <summary>
  ///   Tests whether the specified <see cref="MenuItem"/>'s type is supported by the
  ///   collection.
  /// </summary>
  /// <param name="menuItem"> The <see cref="MenuItem"/> to be tested. </param>
  /// <returns> <see langword="true"/> if the <see cref="MenuItem"/> is suppported. </returns>
  private bool IsSupportedType (MenuItem menuItem)
  {
    Type menuItemType = menuItem.GetType();

    foreach (Type type in _supportedTypes)
    {
      if (type.IsAssignableFrom (menuItemType))
        return true;
    }
    
    return false;
  }
}

}
