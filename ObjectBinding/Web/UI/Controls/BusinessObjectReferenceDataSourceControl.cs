using System;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

public class BusinessObjectReferenceDataSourceControl: BusinessObjectDataSourceControl, IBusinessObjectBoundModifiableControl
{
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectBoundModifiableControl _control;

  public BusinessObjectReferenceDataSourceControl()
  {
    _dataSource = new BusinessObjectReferenceDataSource();
    _control = (IBusinessObjectBoundModifiableControl) _dataSource;
  }

  protected override IBusinessObjectDataSource GetDataSource()
  {
    return _dataSource;
  }

  /// <summary>
  /// The <see cref="BusinessObjectReferenceDataSource"/> that is wrapped by this data source control.
  /// </summary>
  [Browsable (false)]
  public BusinessObjectReferenceDataSource WrappedDataSource
  {
    get { return _dataSource; }
  }

  /// <summary>
  /// The data source that is referenced by this data source.
  /// </summary>
  [Category ("Data")]
  public IBusinessObjectDataSource ReferencedDataSource
  {
    get { return _dataSource.DataSource; }
    set { _dataSource.DataSource = value; }
  }

  [Category ("Data")]
  [Editor (typeof (PropertyPickerEditor), typeof (UITypeEditor))]
  public string PropertyIdentifier
  {
    get { return _dataSource.PropertyIdentifier; }
    set { _dataSource.PropertyIdentifier = value; }
  }

  [Browsable (false)]
  public IBusinessObjectReferenceProperty ReferenceProperty
  {
    get { return _dataSource.ReferenceProperty; }
    set { _dataSource.ReferenceProperty = value; }
  }
  
  [Browsable (false)]
  IBusinessObjectProperty IBusinessObjectBoundControl.Property
  {
    get { return _control.Property; }
    set { _control.Property = value; }
  }

  bool IBusinessObjectBoundControl.SupportsProperty (IBusinessObjectProperty property)
  {
    return _control.SupportsProperty (property);
  }

  [Browsable (false)]
  IBusinessObjectDataSource IBusinessObjectBoundControl.DataSource
  {
    get { return ReferencedDataSource; }
    set { ReferencedDataSource = value;}
  }

  [Browsable (false)]
  object IBusinessObjectBoundControl.Value
  {
    get { return _control.Value; }
    set { _control.Value = value; }
  }

  void IBusinessObjectBoundControl.LoadValue (bool interim)
  {
    _control.LoadValue (interim);
  }

  void IBusinessObjectBoundModifiableControl.SaveValue (bool interim)
  {
    _control.SaveValue (interim);
  }
}

}
