using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding
{

public abstract class BusinessObjectReferenceDataSourceBase: BusinessObjectDataSource, IBusinessObjectDataSource
{
  [CLSCompliant (false)]
  protected IBusinessObject _businessObject;

  [CLSCompliant (false)]
  protected bool _businessObjectChanged = false;

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
    get { return (ReferencedDataSource == null) ? null : ReferencedDataSource.BusinessObjectProvider; }
  }

  public abstract IBusinessObjectReferenceProperty ReferenceProperty { get; set; }

  public abstract IBusinessObjectDataSource ReferencedDataSource { get; }
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
public class BusinessObjectReferenceDataSource: BusinessObjectReferenceDataSourceBase, IBusinessObjectBoundModifiableControl
{
  private IBusinessObjectDataSource _dataSource;
  private string _propertyIdentifier;
  private IBusinessObjectReferenceProperty _property;
  private bool _propertyDirty = true;

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
  IBusinessObjectProperty IBusinessObjectBoundControl.Property
  {
    get { return ReferenceProperty; }
    set 
    { 
      _property = (IBusinessObjectReferenceProperty) value; 
    }
  }

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

  public override IBusinessObjectDataSource ReferencedDataSource
  {
    get { return _dataSource; }
  }
}

}
