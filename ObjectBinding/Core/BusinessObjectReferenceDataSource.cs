using System;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   This interface provides functionality to access to the <see cref="IBusinessObject"/> returned by other 
///   data sources using the specified <see cref="IBusinessObjectReferenceProperty"/>.
/// </summary>
/// <remarks>
///   <para>
///     Through the use of an <b>IBusinessObjectReferenceDataSource</b> it is possible to access a referenced 
///     <see cref="IBusinessObject"/> identified by the <see cref="IBusinessObjectReferenceProperty"/> from the 
///     primary <see cref="IBusinessObject"/> connected to the <see cref="ReferencedDataSource"/>. The referenced
///     object is then used as this <see cref="IBusinessObjectReferenceDataSource"/>'s <see cref="BusinessObject"/>, 
///     allowing the cascading of <see cref="IBusinessObject"/> objects.
///     <note type="inheritinfo">
///       The <b>IBusinessObjectReferenceDataSource</b> is usually implemented as a cross between the
///       <see cref="IBusinessObjectDataSource"/> from which this interface is inherited and an 
///       <see cref="IBusinessObjectBoundControl"/> or <see cref="IBusinessObjectBoundModifiableControl"/>.
///     </note>
///   </para>
///   <para>
///     <see cref="BusinessObjectReferenceDataSource"/> provides an implementation of this interface. Since the
///     <see cref="IBusinessObjectDataSource.BusinessObjectClass"/> is determined by the selected 
///     <see cref="ReferenceProperty"/>, the generic <see cref="BusinessObjectReferenceDataSource"/> will be sufficient
///     for most (or possibly all) specialized business object models.
///   </para>
/// </remarks>
/// <seealso cref="IBusinessObjectDataSource"/>
public interface IBusinessObjectReferenceDataSource: IBusinessObjectDataSource
{
  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
  ///   <see cref="IBusinessObject"/> to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
  ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
  /// </value>
  /// <remarks>
  ///   Usually identical to <see cref="IBusinessObjectBoundControl.Property"/>, i.e. <b>ReferenceProperty</b>
  ///   gets or sets the current value of <see cref="IBusinessObjectBoundControl.Property"/>.
  /// </remarks>
  IBusinessObjectReferenceProperty ReferenceProperty { get; set; }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
  ///   to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
  ///   <see cref="IBusinessObjectReferenceDataSource"/> connects.
  ///  </value>
  /// <remarks>
  ///   Usually identical to <see cref="IBusinessObjectBoundControl.DataSource"/>, i.e. <b>ReferencedDataSource</b>
  ///   gets the current value of <see cref="IBusinessObjectBoundControl.DataSource"/>.
  /// </remarks>
  IBusinessObjectDataSource ReferencedDataSource { get; }
}

/// <summary>
///   This abstract class provides base functionality usually required by an implementation of
///   <see cref="IBusinessObjectReferenceDataSource"/>.
/// </summary>
/// <remarks>
///   Only members with functionality common to all implementations of <see cref="IBusinessObjectReferenceDataSource"/> 
///   have been implemented. The actual implementation of the <see cref="IBusinessObjectBoundModifiableControl"/> 
///   interface is left to the child class. 
/// </remarks>
/// <seealso cref="IBusinessObjectReferenceDataSource"/>
public abstract class BusinessObjectReferenceDataSourceBase: 
    BusinessObjectDataSource, IBusinessObjectReferenceDataSource
{
  /// <summary>
  ///   The <see cref="IBusinessObject"/> accessed through <see cref="ReferenceProperty"/> and provided as 
  ///   the <see cref="BusinessObject"/>.
  /// </summary>
  [CLSCompliant (false)]
  protected IBusinessObject _businessObject;

  /// <summary>
  ///   A flag that is cleared when the <see cref="_businessObject"/> is loaded from or saved to the
  ///   <see cref="ReferencedDataSource"/>.
  /// </summary>
  [CLSCompliant (false)]
  protected bool _businessObjectChanged = false;

  /// <summary> 
  ///   Loads the <see cref="BusinessObject"/> from the <see cref="ReferencedDataSource"/> using 
  ///   <see cref="ReferenceProperty"/> and populates the bound controls using 
  ///   <see cref="BusinessObjectDataSource.LoadValues"/>.
  /// </summary>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  /// <remarks>
  ///   For details on <b>LoadValue</b>, 
  ///   see <see cref="IBusinessObjectDataSource.LoadValues">IBusinessObjectDataSource.LoadValues</see>.
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundControl.LoadValue">IBusinessObjectBoundControl.LoadValue</seealso>
  public void LoadValue (bool interim)
  {
    // load value from "parent" data source
    if (ReferencedDataSource != null && ReferencedDataSource.BusinessObject != null && ReferenceProperty != null)
    {
      _businessObject = (IBusinessObject) ReferencedDataSource.BusinessObject[ReferenceProperty];
      if (_businessObject == null && Mode == DataSourceMode.Edit && ReferenceProperty.CreateIfNull)
        _businessObject = ReferenceProperty.Create (ReferencedDataSource.BusinessObject);     
      _businessObjectChanged = false;
    }

    // load values into "child" controls
    LoadValues (interim);
  }

  /// <summary> 
  ///   Saves the values from the bound controls using <see cref="BusinessObjectDataSource.SaveValues"/>
  ///   and writes the <see cref="BusinessObject"/> back into the <see cref="ReferencedDataSource"/> using 
  ///   <see cref="ReferenceProperty"/>.
  /// </summary>
  /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
  /// <remarks>
  ///   For details on <b>SaveValue</b>, 
  ///   see <see cref="IBusinessObjectDataSource.SaveValues">IBusinessObjectDataSource.SaveValues</see>.
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundModifiableControl.SaveValue">IBusinessObjectBoundModifiableControl.SaveValue</seealso>
  public void SaveValue (bool interim)
  {
    // save values from "child" controls
    SaveValues (interim);

    // if required, save value into "parent" data source
    if (ReferencedDataSource != null && ReferencedDataSource.BusinessObject != null && ReferenceProperty != null 
        && ReferenceProperty.ReferenceClass != null 
        && (_businessObjectChanged || ReferenceProperty.ReferenceClass.RequiresWriteBack))
    {
      ReferencedDataSource.BusinessObject[ReferenceProperty] = _businessObject;
      _businessObjectChanged = false;
    }
  }

  /// <summary>
  ///   Tests whether the <see cref="IBusinessObjectReferenceDataSource"/> can be bound to the 
  ///   <paramref name="property"/>.
  /// </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to be tested. </param>
  /// <returns>
  ///   <see langword="true"/> if the <paremref name="property"/> is of type 
  ///   <see cref="IBusinessObjectReferenceProperty"/>.
  /// </returns>
  /// <seealso cref="IBusinessObjectBoundControl.SupportsProperty">IBusinessObjectBoundControl.SupportsProperty</seealso>
  public bool SupportsProperty (IBusinessObjectProperty property)
  {
    return property is IBusinessObjectReferenceProperty;
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObject"/> accessed through the <see cref="ReferenceProperty"/>.
  /// </summary>
  /// <value> An <see cref="IBusinessObject"/> or <see langword="null"/>. </value>
  /// <remarks> Setting the <b>BusinessObject</b> does not set the <see cref="_businessObjectChanged"/> flag. </remarks>
  public override IBusinessObject BusinessObject
  {
    get { return _businessObject; }
    set { _businessObject = value; }
  }

  /// <summary> 
  ///   Gets the <see cref="IBusinessObjectReferenceProperty.ReferenceClass"/> of the <see cref="ReferenceProperty"/>.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectClass"/> or <see langword="null"/> if no <see cref="ReferenceProperty"/> is set.
  /// </value>
  public override IBusinessObjectClass BusinessObjectClass
  {
    get 
    { 
      IBusinessObjectReferenceProperty property = ReferenceProperty;
      return (property == null) ? null : property.ReferenceClass; 
    }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectProvider"/> used for accessing supplementary information on the bound
  ///   <see cref="IBusinessObject"/>.
  /// </summary>
  /// <value> The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>. </value>
  public override IBusinessObjectProvider BusinessObjectProvider
  {
    get { return (ReferencedDataSource == null) ? null : ReferencedDataSource.BusinessObjectProvider; }
  }

  /// <summary>
  ///   See <see cref="IBusinessObjectReferenceDataSource.ReferenceProperty">IBusinessObjectReferenceDataSource.ReferenceProperty</see>
  ///   for information on how to implement this abstract property.
  /// </summary>
  public abstract IBusinessObjectReferenceProperty ReferenceProperty { get; set; }

  /// <summary>
  ///   See <see cref="IBusinessObjectReferenceDataSource.ReferencedDataSource">IBusinessObjectReferenceDataSource.ReferencedDataSource</see>
  ///   for information on how to implement this abstract property.
  /// </summary>
  public abstract IBusinessObjectDataSource ReferencedDataSource { get; }
}

/// <summary>
///   This data source provides access to the objects returned by other data sources using the specified property.
/// </summary>
/// <remarks>
///   This class acts as both source (<see cref="IBusinessObjectDataSource"/>) and consumer 
///   (<see cref="IBusinessObjectBoundControl"/>) of business objects. 
///   <note>
///     <see cref="IBusinessObjectDataSource.BusinessObject"/> and <see cref="IBusinessObjectBoundControl.Value"/>
///     are always identical.
///   </note>
/// </remarks>
/// <seealso cref="IBusinessObjectReferenceDataSource"/>
/// <seealso cref="IBusinessObjectBoundModifiableControl"/>
public class BusinessObjectReferenceDataSource: 
    BusinessObjectReferenceDataSourceBase, 
    IBusinessObjectBoundModifiableControl
{
  private IBusinessObjectDataSource _dataSource;
  private string _propertyIdentifier;
  private IBusinessObjectReferenceProperty _property;
  private bool _propertyDirty = true;

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
  ///   to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
  ///   <see cref="IBusinessObjectReferenceDataSource"/> connects.
  ///  </value>
  [Category ("Data")]
  public IBusinessObjectDataSource DataSource
  {
    get { return _dataSource; }
    set
    {
      if (value == this) throw new ArgumentException ("Assigning a reference data source as its own data source is not allowed.", "value");
      if (_dataSource != null)
        _dataSource.Unregister (this);
      _dataSource = value; 
      if (value != null)
        value.Register (this); 
      _propertyDirty = true;
    }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
  ///   to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
  ///   <see cref="IBusinessObjectReferenceDataSource"/> connects.
  ///  </value>
  /// <remarks> Identical to <see cref="DataSource"/>. </remarks>
  public override IBusinessObjectDataSource ReferencedDataSource
  {
    get { return _dataSource; }
  }

  /// <summary>
  ///   Gets or sets the string representation of the <see cref="ReferenceProperty"/>.
  /// </summary>
  /// <value> 
  ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for
  ///   the <see cref="IBusinessObjectReferenceProperty"/> returned by <see cref="ReferenceProperty"/>. 
  /// </value>
  [Category ("Data")]
  [Editor (typeof (PropertyPickerEditor), typeof (UITypeEditor))]
  public string PropertyIdentifier
  {
    get { return _propertyIdentifier; }
    set 
    { 
      _propertyIdentifier = value; 
      _propertyDirty = true;
    }
  }

  // Ndoc is unable to understand the explicit interface implemtation of a property
  // Ndoc 1.3 has a workaround for this: DocumentExplicitInterfaceImplementations=false, which is the default
  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
  ///   <see cref="IBusinessObject"/> to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
  ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
  /// </value>
  /// <remarks> Identical to <see cref="ReferenceProperty"/>. </remarks>
  IBusinessObjectProperty IBusinessObjectBoundControl.Property
  {
    get { return ReferenceProperty; }
    set 
    { 
      _property = (IBusinessObjectReferenceProperty) value; 
    }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
  ///   <see cref="IBusinessObject"/> to which this <see cref="BusinessObjectReferenceDataSource"/> connects.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
  ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
  /// </value>
  [Browsable(false)]
  public override IBusinessObjectReferenceProperty ReferenceProperty
  {
    get 
    { 
      if (_propertyDirty)
      {
        if (_dataSource != null && _dataSource.BusinessObjectClass != null && _propertyIdentifier != null)
          _property = (IBusinessObjectReferenceProperty) _dataSource.BusinessObjectClass.GetPropertyDefinition (_propertyIdentifier);
        else
          _property = null;
        _propertyDirty = false;
      }
      return _property; 
    }
    set 
    {
      _property = value;
    }
  }

  // Ndoc seems unable to understand the explicit interface implemtation of a property
  // Ndoc 1.3 has a workaround for this: DocumentExplicitInterfaceImplementations=false, which is the default
  /// <summary> Gets or sets the value provided by the <see cref="BusinessObjectReferenceDataSource"/>. </summary>
  /// <value> The <see cref="IBusinessObject"/> accessed using <see cref="P:IBusinessObjectBoundControl.Property"/>. </value>
  object IBusinessObjectBoundControl.Value
  {
    get { return _businessObject; }
    set 
    { 
      _businessObject = (IBusinessObject) value; 
      _businessObjectChanged = true;
    }
  }

  /// <summary>
  ///   Gets a flag specifying whether the <see cref="BusinessObjectReferenceDataSource"/> has a valid configuration.
  /// </summary>
  /// <value> 
  ///   <see langword="false"/> if the <see cref="ReferenceProperty"/> is not accessible for the 
  ///   <see cref="DataSource"/>'s <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>
  ///   and <see cref="IBusinessObjectDataSource.BusinessObject"/>.
  /// </value>
  [Browsable (false)]
  public bool HasValidBinding
  {
    get 
    { 
      if (_dataSource == null || _property == null)
        return true;

      return _property.IsAccessible (_dataSource.BusinessObjectClass, _dataSource.BusinessObject);
    }
  }
}

}
