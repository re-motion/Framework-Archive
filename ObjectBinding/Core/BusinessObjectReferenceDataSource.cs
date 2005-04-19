using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   This interface provides functionality to access to the objects returned by other data sources 
///   using the specified property.
/// </summary>
/// <remarks>
///   <para>
///     Through the use of an <b>IBusinessObjectReferenceDataSource</b> it is possible to get the 
///     <see cref="IBusinessObject"/> identified by the <see cref="IBusinessObjectReferenceProperty"/> from the 
///     <see cref="ReferencedDataSource"/>. This object is then used as the 
///     <see cref="IBusinessObjectReferenceDataSource"/>'s <see cref="BusinessObject"/>, 
///     allowing the cascading of <see cref="IBusinessObject"/> objects.
///     <note type="implementnotes">
///       The <b>IBusinessObjectReferenceDataSource</b> is usually implemented as a cross between the
///       <see cref="IBusinessObjectDataSource"/> from which this interface is inherited and an 
///       <see cref="IBusinessObjectBoundControl"/> or <see cref="IBusinessObjectBoundModifiableControl"/>.
///     </note>
///   </para>
///   <para>
///     <see cref="BusinessObjectReferenceDataSource"/> provides an implementation of this interface.
///   </para>
/// </remarks>
public interface IBusinessObjectReferenceDataSource: IBusinessObjectDataSource
{
  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
  ///   <see cref="IBusinessObject"/> provided by this <see cref="IBusinessObjectReferenceDataSource"/>'s
  ///   <see cref="BusinessObject"/>.
  /// </summary>
  /// <remarks>
  ///   Usually identical to <see cref="IBusinessObjectBoundControl.Property"/>.
  /// </remarks>
  /// <value> 
  ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
  ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
  /// </value>
  IBusinessObjectReferenceProperty ReferenceProperty { get; set; }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
  ///   to which this <see cref="IBusinessObjectReferenceDataSource"/> is bound.
  /// </summary>
  /// <remarks> Usually identical to <see cref="IBusinessObjectBoundControl.DataSource"/>. </remarks>
  /// <value> The <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
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
  /// <remarks>
  ///   <para>
  ///     Implementation of <see cref="IBusinessObjectBoundControl.LoadValue"/>.
  ///   </para>
  ///   <para>
  ///     For details on <paramref name="interim"/>, see <see cref="IBusinessObjectDataSource.LoadValues"/>.
  ///   </para>
  /// </remarks>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  public void LoadValue (bool interim)
  {
    // load value from "parent" data source
    if (ReferencedDataSource != null && ReferencedDataSource.BusinessObject != null && ReferenceProperty != null)
    {
      _businessObject = (IBusinessObject) ReferencedDataSource.BusinessObject[ReferenceProperty];
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
  /// <remarks>
  ///   <para>
  ///     Implementation of <see cref="IBusinessObjectBoundModifiableControl.SaveValue"/>.
  ///   </para>
  ///   <para>
  ///     For details on <paramref name="interim"/>, see <see cref="IBusinessObjectDataSource.SaveValues"/>.
  ///   </para>
  /// </remarks>
  /// <param name="interim"> Spefifies whether this is the final saving, or an interim saving. </param>
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
  ///   Implementation of <see cref="IBusinessObjectBoundControl.SupportsProperty"/>.
  /// </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to be tested. </param>
  /// <value>
  ///   <see langword="true"/> if the <paremref name="property"/> is of type 
  ///   <see cref="IBusinessObjectReferenceProperty"/>.
  /// </value>
  public bool SupportsProperty (IBusinessObjectProperty property)
  {
    return property is IBusinessObjectReferenceProperty;
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObject"/> who's properties will be loaded into the bound controls.
  /// </summary>
  /// <remarks> Setting the <b>BusinessObject</b> does not set the <see cref="_businessObjectChanged"/> flag. </remarks>
  /// <value> The <see cref="IBusinessObjectClass"/> of the bound <see cref="IBusinessObject"/>. </value>
  public override IBusinessObject BusinessObject
  {
    get { return _businessObject; }
    set { _businessObject = value; }
  }

  /// <summary> 
  ///   Gets the <see cref="IBusinessObjectReferenceProperty.ReferenceClass"/> of the <see cref="ReferenceProperty"/>.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectClass"/> of the <see cref="ReferenceProperty"/> or <see langword="null"/>
  ///   if no <see cref="ReferenceProperty"/> is set.
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
  /// <value>
  ///   The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>.
  ///   Must not return <see langword="null"/>.
  /// </value>
  public override IBusinessObjectProvider BusinessObjectProvider
  {
    get { return (ReferencedDataSource == null) ? null : ReferencedDataSource.BusinessObjectProvider; }
  }

  /// <summary>
  ///   See <see cref="IBusinessObjectReferenceDataSource.ReferenceProperty"/> for information on how to implement 
  ///   this abstract property.
  /// </summary>
  public abstract IBusinessObjectReferenceProperty ReferenceProperty { get; set; }

  /// <summary>
  ///   See <see cref="IBusinessObjectReferenceDataSource.ReferencedDataSource"/> for information on how to implement 
  ///   this abstract property.
  /// </summary>
  public abstract IBusinessObjectDataSource ReferencedDataSource { get; }
}

/// <summary>
///   This data source provides access to the objects returned by other data sources using the specified property.
/// </summary>
/// <remarks>
///   This class acts as both a source (<see cref="IBusinessObjectDataSource"/>) and consumer 
///   (<see cref="IBusinessObjectBoundControl"/>) of business objects. 
///   <note>
///     <see cref="IBusinessObjectDataSource.BusinessObject"/> and <see cref="IBusinessObjectBoundControl.Value"/>
///     are always identical.
///   </note>
/// </remarks>
public class BusinessObjectReferenceDataSource: BusinessObjectReferenceDataSourceBase, IBusinessObjectBoundModifiableControl
{
  private IBusinessObjectDataSource _dataSource;
  private string _propertyIdentifier;
  private IBusinessObjectReferenceProperty _property;
  private bool _propertyDirty = true;

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
  ///   to which this <see cref="BusinessObjectReferenceDataSource"/> is bound.
  /// </summary>
  /// <value> An <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
  [Category ("Data")]
  public IBusinessObjectDataSource DataSource
  {
    get { return _dataSource; }
    set
    {
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
  ///   to which this <see cref="IBusinessObjectReferenceDataSource"/> is bound.
  /// </summary>
  /// <remarks> Identical to <see cref="DataSource"/>. </remarks>
  /// <value> An <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
  public override IBusinessObjectDataSource ReferencedDataSource
  {
    get { return _dataSource; }
  }

  /// <summary>
  ///   Gets or sets the string representation of the <see cref="ReferenceProperty"/>.
  /// </summary>
  /// <value> 
  ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for
  ///   the <see cref="IBusinessObjectReferenceProperty"/>. 
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

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
  ///   <see cref="IBusinessObject"/> provided by this <see cref="IBusinessObjectReferenceDataSource"/>'s
  ///   <see cref="BusinessObject"/>.
  /// </summary>
  /// <remarks> Identical to <see cref="ReferenceProperty"/>. </remarks>
  /// <value> 
  ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
  ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
  /// </value>
  [Browsable(false)]
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
  ///   <see cref="IBusinessObject"/> provided by this <see cref="IBusinessObjectReferenceDataSource"/>'s
  ///   <see cref="BusinessObject"/>.
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

  /// <summary> Gets or sets the value provided by the <see cref="BusinessObjectReferenceDataSource"/>. </summary>
  /// <value> The <see cref="IBusinessObject"/> accessed using <see cref="P:IBusinessObjectBoundControl.Property"/>. </value>
  [Browsable (false)]
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
  ///   <see langword="false"/> the <see cref="ReferenceProperty"/> is not accessible for the current 
  ///   <see cref="DataSource"/>'s current <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>
  ///   and <see cref="IBusinessObjectDataSource.BusinessObject"/>.
  /// </value>
  [Browsable (false)]
  public bool IsValid
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
