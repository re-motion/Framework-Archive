using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding
{

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
  private bool _businessObjectChanged = false;

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
    set 
    { 
      _property = (IBusinessObjectReferenceProperty) value; 
    }
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
    set 
    {
      _property = value;
    }
  }

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

  public void LoadValue (bool interim)
  {
    // load value from "parent" data source
    if (_dataSource != null && _dataSource.BusinessObject != null && ReferenceProperty != null)
    {
      _businessObject = (IBusinessObject) _dataSource.BusinessObject[ReferenceProperty];
      _businessObjectChanged = false;
    }

    // load values into "child" controls
    LoadValues (interim);
  }

  public void SaveValue (bool interim)
  {
    // save values from "child" controls
    SaveValues (interim);

    // if required, save value into "parent" data source
    if (_dataSource != null && _dataSource.BusinessObject != null && ReferenceProperty != null 
        && ReferenceProperty.ReferenceClass != null 
        && (_businessObjectChanged || ReferenceProperty.ReferenceClass.RequiresWriteBack))
    {
      _dataSource.BusinessObject[ReferenceProperty] = _businessObject;
      _businessObjectChanged = false;
    }
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
