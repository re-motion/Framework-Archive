using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{
 
/// <summary> A collection of <see cref="PropertyPathBinding"/> objects. </summary>
[Editor (typeof (PropertyPathBindingCollectionEditor), typeof (UITypeEditor))]
public sealed class PropertyPathBindingCollection : CollectionBase
{
  /// <summary> 
  ///   The <see cref="IBusinessObjectBoundControl"/> to which this 
  ///   <see cref="PropertyPathBindingCollection"/> belongs.
  /// </summary>
  private IBusinessObjectBoundControl _ownerControl;

  /// <summary> 
  ///   Initializes a new instance of the <see cref="PropertyPathBindingCollection"/> class
  ///   with the <see cref="IBusinessObjectBoundWebControl"/> to which it belongs.
  /// </summary>
  /// <param name="ownerControl">
  ///   The <see cref="IBusinessObjectBoundControl"/> to which this 
  ///   <see cref="PropertyPathBindingCollection"/> belongs.
  /// </param>
  internal PropertyPathBindingCollection (IBusinessObjectBoundControl ownerControl)
  {
    _ownerControl = ownerControl;
  }

  /// <summary> Performs additional custom processes before inserting a new element. </summary>
  /// <param name="index"> The zero-based index at which to insert value. </param>
  /// <param name="value"> The new value of the element at index. </param>
  protected override void OnInsert(int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (PropertyPathBinding));
    base.OnInsert (index, value);    
    if (_ownerControl != null)
      ((PropertyPathBinding) value).DataSource = _ownerControl.DataSource;
  }

  /// <summary> Performs additional custom processes before setting a value. </summary>
  /// <param name="index"> The zero-based index at which <paramref name="oldValue"/> can be found. </param>
  /// <param name="oldValue"> The value to replace with <paramref name="newValue"/>. </param>
  /// <param name="newValue"> The new value of the element at index. </param>
  protected override void OnSet(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (PropertyPathBinding));
    base.OnSet (index, oldValue, newValue);    
    if (_ownerControl != null)
      ((PropertyPathBinding) newValue).DataSource = _ownerControl.DataSource;
  }

  /// <summary> Adds an item to the <see cref="IList"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="PropertyPathBinding"/> to add to the <see cref="IList"/>. 
  /// </param>
  /// <returns> The position into which the new element was inserted. </returns>
  public int Add (PropertyPathBinding value)
  {
    return List.Add (value);
  }

  /// <summary> Adds the <see cref="PropertyPathBinding"/> array to the <see cref="IList"/>. </summary>
  /// <param name="values"> 
  ///   The <see cref="PropertyPathBinding"/> array to add to the <see cref="IList"/>. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  public void AddRange (PropertyPathBinding[] values)
  {
    ArgumentUtility.CheckNotNull ("values", values);
    foreach (PropertyPathBinding propertyPathBinding in values)
      Add (propertyPathBinding);
  }

  /// <summary> Adds the <see cref="PropertyPathBinding"/> array to the <see cref="IList"/>. </summary>
  /// <param name="propertyPaths">
  ///   The <see cref="BusinessObjectPropertyPath"/> objects to add.
  /// </param>
  public void AddRange (BusinessObjectPropertyPath[] propertyPaths)
  {
    foreach (BusinessObjectPropertyPath propertyPath in propertyPaths)
      Add (new PropertyPathBinding (propertyPath));
  }

  /// <summary> Adds the <see cref="PropertyPathBinding"/> array to the <see cref="IList"/>. </summary>
  /// <param name="propertyPathIdentifiers">
  ///   The identifiers for the <see cref="BusinessObjectPropertyPath"/> objects to add.
  /// </param>
  public void AddRange (string[] propertyPathIdentifiers)
  {
    foreach (string propertyPathIdentifier in propertyPathIdentifiers)
      Add (new PropertyPathBinding (propertyPathIdentifier));
  }

  /// <summary> Copies the elements of the <see cref="IList"/> to a new array. </summary>
  /// <returns> 
  ///   An <see cref="PropertyPathBinding"/> array containing the elements of the 
  ///   <see cref="IList"/>.
  /// </returns>
  public PropertyPathBinding[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (PropertyPathBinding[]) arrayList.ToArray (typeof (PropertyPathBinding));
  }
 
  /// <summary> Gets or sets the element at the specified index. </summary>
  /// <value> The element at the specified index. </value>
  public PropertyPathBinding this[int index]
  {
    get
    {
      PropertyPathBinding propertyPathBinding = (PropertyPathBinding) List[index];
      return propertyPathBinding;
    }
    set
    {
      List[index] = value; 
    }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectBoundControl"/> to which this 
  ///   <see cref="PropertyPathBindingCollection"/> belongs.
  /// </summary>
  internal IBusinessObjectBoundControl OwnerControl
  {
    get
    { 
      return _ownerControl; 
    }
    set
    {
      _ownerControl = value; 
      foreach (PropertyPathBinding binding in List)
        binding.OwnerControl = _ownerControl;
    }
  }
}

}
