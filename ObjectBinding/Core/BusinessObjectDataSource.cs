using System;
using System.Collections;
using System.ComponentModel;
using Rubicon.NullableValueTypes;

using System.Drawing.Design;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectDataSource
{
  bool IsWritable { get; }

  void Register (IBusinessObjectBoundControl control);
  void Unregister (IBusinessObjectBoundControl control);

  /// <summary>
  ///   Load the values of the business object into all registered controls.
  /// </summary>
  /// <remarks>
  ///   On initial loads, all values must be loaded. On interim loads, each control decides whether it keeps its own 
  ///   value (e.g., using view state) or whether it reloads the value (useful for complex structures that need no 
  ///   validation).
  /// </remarks>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  void LoadValues (bool interim);

  /// <summary>
  ///   Save the values of the business object from all registered controls.
  /// </summary>
  /// <remarks>
  ///   On final saves, all values must be saved. (It is assumed that invalid values were already identified using 
  ///   validators.) On interim saves, each control decides whether it saves its values into the business object or
  ///   using an alternate mechanism (e.g. view state).
  /// </remarks>
  /// <param name="interim"> Spefifies whether this is the final saving, or an interim saving. </param>
  void SaveValues (bool interim);

  IBusinessObject BusinessObject { get; set; }
  IBusinessObjectClass BusinessObjectClass { get; }

  IBusinessObjectProvider BusinessObjectProvider { get; }
}

public abstract class BusinessObjectDataSource: Component, IBusinessObjectDataSource
{
  private ArrayList _boundControls = null;
  private bool _editMode = true;
  // TODO: private bool _dataBindCalled = false;

  [Category ("Data")]
  public bool EditMode
  {
    get { return _editMode; }
    set { _editMode = value; }
  }

  [Browsable (false)]
  bool IBusinessObjectDataSource.IsWritable
  {
    get { return EditMode; }
  }

  public void Register (IBusinessObjectBoundControl control)
  {
    if (_boundControls == null)
      _boundControls = new ArrayList (5);
    if (! _boundControls.Contains (control))
      _boundControls.Add (control);
  }

  public void Unregister (IBusinessObjectBoundControl control)
  {
    if (_boundControls != null)
      _boundControls.Remove (control);
  }

  public void LoadValues (bool interim)
  {
    if (_boundControls != null)
    {
      foreach (IBusinessObjectBoundControl control in _boundControls)
        control.LoadValue (interim);
    }
  }

  public void SaveValues (bool interim)
  {
    if (_boundControls != null)
    {
      foreach (IBusinessObjectBoundControl control in _boundControls)
      {
        IBusinessObjectBoundModifiableControl writeableControl = control as IBusinessObjectBoundModifiableControl;
        if (writeableControl != null)
          writeableControl.SaveValue (interim);
      }
    }
  }

  [Browsable (false)]
  public abstract IBusinessObject BusinessObject { get; set; }

  [Browsable (false)]
  public abstract IBusinessObjectClass BusinessObjectClass { get; }

  [Browsable (false)]
  public virtual IBusinessObjectProvider BusinessObjectProvider 
  { 
    get { return (BusinessObjectClass == null) ? null : BusinessObjectClass.BusinessObjectProvider; }
  }
}

/// <summary>
///   This data source provides access to the objects returned by other data sources's using the specified property.
/// </summary>
/// <remarks>
///   This class acts as both a source (<see cref="IBusinessObjectDataSource"/>)and consumer 
///   (<see cref="IBusinessObjectBoundControl"/>) of business objects. Note that 
///   <see cref="IBusinessObjectDataSource.BusinessObject"/> and <see cref="IBusinessObjectBoundControl.Value"/>
///   are always identical.
/// </remarks>
public class PropertyBusinessObjectDataSource: BusinessObjectDataSource, IBusinessObjectDataSource, IBusinessObjectBoundModifiableControl
{
  private IBusinessObject _businessObject;
  private IBusinessObjectDataSource _dataSource;
  private string _propertyIdentifier;
  private IBusinessObjectReferenceProperty _property;
  private bool _propertyDirty = true;

  [Browsable (false)]
  bool IBusinessObjectDataSource.IsWritable
  {
    get { return EditMode && _dataSource.IsWritable; }
  }

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

  [Browsable(false)]
  public IBusinessObjectProperty Property
  {
    get { return ReferenceProperty; }
    set { _property = (IBusinessObjectReferenceProperty) value; }
  }

  [Browsable(false)]
  public IBusinessObjectReferenceProperty ReferenceProperty
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
    set { _property = value; }
  }

  [Browsable (false)]
  object IBusinessObjectBoundControl.Value
  {
    get { return _businessObject; }
    set { _businessObject = (IBusinessObject) value; }
  }

  public void LoadValue (bool interim)
  {
    // load value from "parent" data source
    if (_dataSource != null && _dataSource.BusinessObject != null && ReferenceProperty != null)
      _businessObject = (IBusinessObject) _dataSource.BusinessObject[ReferenceProperty];

    // load values into "child" controls
    LoadValues (interim);
  }

  public void SaveValue (bool interim)
  {
    // save values from "child" controls
    SaveValues (interim);

    // save value into "parent" data source
    if (_dataSource != null && _dataSource.BusinessObject != null && ReferenceProperty != null)
      _dataSource.BusinessObject[ReferenceProperty] = _businessObject;
  }

  public bool SupportsProperty (IBusinessObjectProperty property)
  {
    return property is IBusinessObjectReferenceProperty;
  }

  public override IBusinessObject BusinessObject
  {
    get { return _businessObject; }
    set { _businessObject = value; }
  }

  public override IBusinessObjectClass BusinessObjectClass
  {
    get 
    { 
      IBusinessObjectReferenceProperty property = ReferenceProperty;
      return (property == null) ? null : property.ReferenceClass; 
    }
  }

  public override IBusinessObjectProvider BusinessObjectProvider
  {
    get { return (_dataSource == null) ? null : _dataSource.BusinessObjectProvider; }
  }
}

}