using System;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web;
using Rubicon.ObjectBinding.Design;
using Rubicon.NullableValueTypes;

namespace Rubicon.ObjectBinding.Web.Controls
{

public class BusinessObjectReferenceSearchDataSourceControl: BusinessObjectReferenceDataSourceControl
{
  protected override bool SupportsPropertyMultiplicity(bool isList)
  {
    return true;
  }

  public override void LoadValues (bool interim)
  {
    throw new NotSupportedException ("Use BusinessObjectReferenceDataSourceControl for actual data.");
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public override DataSourceMode Mode
  {
    get { return DataSourceMode.Search; }
    set { if (value != DataSourceMode.Search) throw new NotSupportedException ("BusinessObjectReferenceSearchDataSourceControl supports only DataSourceMode.Search."); }
  }
}

public class BusinessObjectReferenceDataSourceControl
    : BusinessObjectBoundModifiableWebControl, IBusinessObjectDataSourceControl, IBusinessObjectReferenceDataSource
{
  private class InternalBusinessObjectReferenceDataSource: BusinessObjectReferenceDataSourceBase
  {
    private BusinessObjectReferenceDataSourceControl _parent;

    public InternalBusinessObjectReferenceDataSource (BusinessObjectReferenceDataSourceControl parent)
    {
      _parent = parent;
    }

    public override IBusinessObjectReferenceProperty ReferenceProperty
    {
      get { return _parent.ReferenceProperty; }
      set { _parent.ReferenceProperty = value; }
    }

    public override IBusinessObjectDataSource ReferencedDataSource
    {
      get { return _parent.DataSource; }
    }

    public bool IsDirty
    {
      get { return _businessObjectChanged; }
      set { _businessObjectChanged = value; }
    }
  }

  private InternalBusinessObjectReferenceDataSource _internalDataSource;

  protected override Type[] SupportedPropertyInterfaces
  {
    get { return new Type[] { typeof (IBusinessObjectReferenceProperty) }; }
  }

  public BusinessObjectReferenceDataSourceControl ()
  {
    _internalDataSource = new InternalBusinessObjectReferenceDataSource (this);
  }

  protected override object ValueImplementation
  {
    get { return _internalDataSource.BusinessObject; }
    set { _internalDataSource.BusinessObject = (IBusinessObject) value; }
  }

  public override void LoadValue (bool interim) // inherited from control interface
  {
    _internalDataSource.LoadValue (interim);
  }

  public virtual void LoadValues (bool interim) // inherited from data source interface
  {
    _internalDataSource.LoadValues (interim);
  }

  public override void SaveValue (bool interim) // inherited from control interface
  {
    if (! IsReadOnly)
      _internalDataSource.SaveValue (interim);
  }

  public virtual void SaveValues (bool interim) // inherited data source interface
  {
    if (! IsReadOnly)
      _internalDataSource.SaveValue (interim);
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectBoundControl[] BoundControls
  {
    get { return _internalDataSource.BoundControls; }
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectReferenceProperty ReferenceProperty
  {
    get { return (IBusinessObjectReferenceProperty) Property; }
    set { Property = value; }
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectDataSource ReferencedDataSource 
  { 
    get { return _internalDataSource.ReferencedDataSource; }
  }


  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObject BusinessObject
  {
    get { return _internalDataSource.BusinessObject; }
    set { _internalDataSource.BusinessObject = value; }
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectClass BusinessObjectClass
  {
    get { return _internalDataSource.BusinessObjectClass; }
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectProvider BusinessObjectProvider
  {
    get { return _internalDataSource.BusinessObjectProvider; }
  }

  public void Register (IBusinessObjectBoundControl control)
  {
    _internalDataSource.Register (control);
  }

  public void Unregister (IBusinessObjectBoundControl control)
  {
    _internalDataSource.Unregister (control);
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public virtual DataSourceMode Mode
  {
    get { return IsReadOnly ? DataSourceMode.Read : DataSourceMode.Edit; }
    set { ReadOnly = (NaBoolean) (value == DataSourceMode.Read); } // "search" needs edit mode
  }

  // TODO: redesign IsDirty semantics!
  public override bool IsDirty
  {
    get { return _internalDataSource.IsDirty; }
    set { _internalDataSource.IsDirty = value; }
  }

  protected override void Render (System.Web.UI.HtmlTextWriter writer)
  {
    // invisible control
  }
}


/* public class BusinessObjectReferenceDataSourceControl: BusinessObjectDataSourceControl, IBusinessObjectBoundModifiableControl
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
*/
}
