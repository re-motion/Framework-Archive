using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Globalization;
using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.UI.Controls
{
/// <summary>
///   A <see cref="PropertyPathBinding"/> encapsulates the creation of a 
///   <see cref="BusinessObjectPropertyPath"/> from its string representation and an
///   <see cref="IBusinessObjectDataSource"/>
/// </summary>
public class PropertyPathBinding: BusinessObjectControlItem, IBusinessObjectClassSource
{
  /// <summary> <see langword="true"/> once the <see cref="PropertyPath"/> has been set. </summary>
  private bool _isPopertyPathEvaluated;
  /// <summary> 
  ///   The <see cref="BusinessObjectPropertyPath"/> mananged by this 
  ///   <see cref="PropertyPathBinding"/>.
  /// </summary>
  private BusinessObjectPropertyPath _propertyPath;
  /// <summary> 
  ///   The <see cref="string"/> representing the <see cref="BusinessObjectPropertyPath"/> mananged 
  ///   by this <see cref="PropertyPathBinding"/>.
  /// </summary>
  private string _propertyPathIdentifier;

  /// <summary> 
  ///   Initializes a new instance of the <see cref="PropertyPathBinding"/> class with the
  ///   <see cref="BusinessObjectPropertyPath"/> managed by this instance.
  ///  </summary>
  /// <param name="propertyPath">
  ///   The <see cref="BusinessObjectPropertyPath"/> mananged by this 
  ///   <see cref="PropertyPathBinding"/>.
  /// </param>
  public PropertyPathBinding (BusinessObjectPropertyPath propertyPath)
  {
    PropertyPath = propertyPath;
  }

  public PropertyPathBinding (string propertyPathIdentifier)
  {
    PropertyPathIdentifier = propertyPathIdentifier;
  }

  public PropertyPathBinding()
  {
  }

  /// <summary>
  ///   Returns a <see cref="string"/> that represents this <see cref="PropertyPathBinding"/>.
  /// </summary>
  /// <returns>
  ///   Returns the class name of the instance.
  /// </returns>
  public override string ToString()
  {
    return GetType().Name;
  }

  /// <summary> 
  ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> used to evaluate the 
  ///   <see cref="PropertyPathIdentifier"/>. 
  /// </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public IBusinessObjectDataSource DataSource
  {
    get
    {
      if (OwnerControl != null)
        return OwnerControl.DataSource; 
      return null;
    }
  }

  /// <summary> 
  ///   Gets or sets the <see cref="BusinessObjectPropertyPath"/> mananged by this 
  ///   <see cref="PropertyPathBinding"/>.
  /// </summary>
  /// <value>
  ///   A <see cref="BusinessObjectPropertyPath"/> or <see langword="null"/> if the 
  ///   <see cref="PropertyPathIdentifier"/> has not been evaluated.
  /// </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BusinessObjectPropertyPath PropertyPath
  {
    get 
    {
      if (! _isPopertyPathEvaluated)
      {
        if (OwnerControl == null)
          throw new InvalidOperationException ("PropertyPath could not be resolved because the object is not part of an IBusinessObjectBoundControl.");

        bool isDesignMode = Rubicon.Web.Utilities.ControlHelper.IsDesignMode (OwnerControl);
        bool isDataSourceNull = DataSource == null;
        bool isBusinessObjectClassNull = BusinessObjectClass == null;

        if (isDesignMode && isBusinessObjectClassNull)
            return null;

        if (StringUtility.IsNullOrEmpty (_propertyPathIdentifier))
        {
          _propertyPath = null;
        }
        else
        {
          if (isDataSourceNull)
            throw new InvalidOperationException ("PropertyPath could not be resolved because the DataSource is not set.");

          _propertyPath = BusinessObjectPropertyPath.Parse (BusinessObjectClass, _propertyPathIdentifier);
        }
        _isPopertyPathEvaluated = true;
      }

      return _propertyPath;
    }
    set 
    {
      _propertyPath = value; 
      _propertyPathIdentifier = (value == null) ? string.Empty : value.ToString();
      _isPopertyPathEvaluated = true;
    }
  }

  /// <summary> 
  ///   Gets or sets the <see cref="string"/> representing the 
  ///   <see cref="BusinessObjectPropertyPath"/> mananged by this <see cref="PropertyPathBinding"/>.
  /// </summary>
  /// <value> 
  ///   A <see cref="string"/> formatted as a valid property path. 
  /// </value>
  [Editor (typeof (PropertyPathPickerEditor), typeof (UITypeEditor))]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [Description ("A string representing a valid property path.")]
  //  No default value
  public string PropertyPathIdentifier
  {
    get 
    {
      return _propertyPathIdentifier; 
    }
    set 
    { 
      _propertyPathIdentifier = value;
      _propertyPath = null;
      _isPopertyPathEvaluated = false;
    }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public IBusinessObjectClass BusinessObjectClass
  {
    get 
    {
      IBusinessObjectReferenceProperty property = null;
      if (OwnerControl != null)
        property = OwnerControl.Property as IBusinessObjectReferenceProperty;
      if (property != null)
        return property.ReferenceClass;
      if (DataSource != null)
        return DataSource.BusinessObjectClass;
      return null;
    }
  }
}

}
