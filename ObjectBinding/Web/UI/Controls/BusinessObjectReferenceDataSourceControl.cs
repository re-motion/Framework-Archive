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

/// <summary>
///   <see cref="BusinessObjectReferenceDataSourceControl"/> provides an 
///   <see cref="IBusinessObjectReferenceDataSource"/> to controls of type 
///   <see cref="IBusinessObjectBoundWebControl"/> inside an <b>ASPX Web Form</b> or <b>ASCX User Control</b>.
/// </summary>
#if net20
[NonVisualControl]
#endif
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

  // TODO: redesign IsDirty semantics!
  public override bool IsDirty
  {
    get { return _internalDataSource.IsDirty; }
    set { _internalDataSource.IsDirty = value; }
  }

  /// <summary> 
  ///   Loads the <see cref="BusinessObject"/> from the <see cref="ReferencedDataSource"/> using 
  ///   <see cref="ReferenceProperty"/> and populates the bound controls using <see cref="LoadValues"/>.
  /// </summary>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  public override void LoadValue (bool interim) // inherited from control interface
  {
    _internalDataSource.LoadValue (interim);
  }

  /// <summary> Loads the values of the <see cref="BusinessObject"/> into all bound controls. </summary>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  public virtual void LoadValues (bool interim) // inherited from data source interface
  {
    _internalDataSource.LoadValues (interim);
  }

  /// <summary> 
  ///   Saves the values from the bound controls using <see cref="SaveValues"/>
  ///   and writes the <see cref="BusinessObject"/> back into the <see cref="ReferencedDataSource"/> using 
  ///   <see cref="ReferenceProperty"/>.
  /// </summary>
  /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
  /// <remarks> Actual saving only occurs if <see cref="IsReadOnly"/> evaluates <see langword="false"/>. </remarks>
  public override void SaveValue (bool interim) // inherited from control interface
  {
    if (! IsReadOnly)
      _internalDataSource.SaveValue (interim);
  }

  /// <summary> 
  ///   Saves the values of the <see cref="BusinessObject"/> from all bound controls implementing
  ///   <see cref="IBusinessObjectBoundModifiableControl"/>.
  /// </summary>
  /// <param name="interim"> Spefifies whether this is the final saving, or an interim saving. </param>
  /// <remarks> Actual saving only occurs if <see cref="IsReadOnly"/> evaluates <see langword="false"/>. </remarks>
  public virtual void SaveValues (bool interim) // inherited data source interface
  {
    if (! IsReadOnly)
      _internalDataSource.SaveValue (interim);
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
  ///   <see cref="IBusinessObject"/> to which this <see cref="BusinessObjectReferenceDataSourceControl"/> connects.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
  ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
  /// </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectReferenceProperty ReferenceProperty
  {
    get { return (IBusinessObjectReferenceProperty) Property; }
    set { Property = value; }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
  ///   to which this <see cref="IBusinessObjectReferenceDataSourceControl"/> connects.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
  ///   <see cref="IBusinessObjectReferenceDataSourceControl"/> connects.
  ///  </value>
  /// <remarks> Identical to <see cref="DataSource"/>. </remarks>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectDataSource ReferencedDataSource 
  { 
    get { return _internalDataSource.ReferencedDataSource; }
  }


  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObject"/> connected to this 
  ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
  /// </summary>
  /// <value>
  ///   An <see cref="IBusinessObject"/> or <see langword="null"/>. Must be compatible with
  ///   <see cref="BusinessObjectClass"/>.
  /// </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObject BusinessObject
  {
    get { return _internalDataSource.BusinessObject; }
    set { _internalDataSource.BusinessObject = value; }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectClass"/> of the connected <see cref="IBusinessObject"/>
  ///   connected to this <see cref="BusinessObjectReferenceDataSourceControl"/>.
  /// </summary>
  /// <value> The <see cref="IBusinessObjectClass"/> of the connected <see cref="IBusinessObject"/>. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectClass BusinessObjectClass
  {
    get { return _internalDataSource.BusinessObjectClass; }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectProvider"/> of this 
  ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
  /// </summary>
  /// <value>
  ///   The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>.
  ///   Must not return <see langword="null"/>.
  /// </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectProvider BusinessObjectProvider
  {
    get { return _internalDataSource.BusinessObjectProvider; }
  }

  /// <summary>
  ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls bound to this
  ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
  /// </summary>
  /// <param name="control"> 
  ///   The <see cref="IBusinessObjectBoundControl"/> to be registered with this
  ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
  /// </param>
  public void Register (IBusinessObjectBoundControl control)
  {
    _internalDataSource.Register (control);
  }

  /// <summary>
  ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls bound to this
  ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
  /// </summary>
  /// <param name="control"> 
  ///   The <see cref="IBusinessObjectBoundControl"/> to be unregistered from this 
  ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
  /// </param>
  public void Unregister (IBusinessObjectBoundControl control)
  {
    _internalDataSource.Unregister (control);
  }

  /// <summary>
  ///   Gets or sets the current <see cref="DataSourceMode"/> of this 
  ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
  /// </summary>
  /// <value> A value of the <see cref="DataSourceMode"/> enumeration. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public virtual DataSourceMode Mode
  {
    get { return IsReadOnly ? DataSourceMode.Read : DataSourceMode.Edit; }
    set { ReadOnly = (NaBoolean) (value == DataSourceMode.Read); } // "search" needs edit mode
  }

  /// <summary>
  ///   Gets an array of <see cref="IBusinessObjectBoundControl"/> objects bound to this 
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <value> An array or <see cref="IBusinessObjectBoundControl"/> objects. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectBoundControl[] BoundControls
  {
    get { return _internalDataSource.BoundControls; }
  }

  /// <summary>
  ///   Overrides the implementation of <see cref="System.Web.UI.Control.Render">Control.Render</see>. 
  ///   Does not render any output.
  /// </summary>
  /// <param name="writer">
  ///   The <see cref="System.Web.UI.HtmlTextWriter"/> object that receives the server control content. 
  /// </param>
  protected override void Render (System.Web.UI.HtmlTextWriter writer)
  {
    //  No output, control is invisible
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
