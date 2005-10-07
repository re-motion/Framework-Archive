using System;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;
using Rubicon.ObjectBinding.Web;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.NullableValueTypes;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///   <b>BusinessObjectReferenceSearchDataSourceControl</b> is used to supply an <see cref="IBusinessObjectClass"/>
///   to an <see cref="IBusinessObjectBoundWebControl"/> used for displaying a search result.
/// </summary>
/// <remarks>
///   Since a search result is usually an <see cref="IBusinessObject"/> list without an actual parent object
///   to connect to the data source, normal data binding is not possible. By using the 
///   <b>BusinessObjectReferenceSearchDataSourceControl</b> it is possible to provide the meta data to the bound
///   controls dispite the lack of a parent object.
/// </remarks>
public class BusinessObjectReferenceSearchDataSourceControl: BusinessObjectReferenceDataSourceControl
{
  /// <summary> Multiplicity is always supported. </summary>
  /// <param name="isList"> Not evaluated. </param>
  /// <returns> Always <see langword="true"/>. </returns>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return true;
  }

  /// <summary> Not supported by <see cref="BusinessObjectReferenceSearchDataSourceControl"/>. </summary>
  /// <param name="interim"> Not evaluated. </param>
  public override void LoadValues (bool interim)
  {
    throw new NotSupportedException ("Use BusinessObjectReferenceDataSourceControl for actual data.");
  }

  /// <summary>
  ///   Gets or sets the current <see cref="DataSourceMode"/> of this 
  ///   <see cref="BusinessObjectReferenceSearchDataSourceControl"/>.
  /// </summary>
  /// <value> <see cref="DataSourceMode.Search"/>. </value>
  /// <exception cref="NotSupportedException"> Thrown upon an attempt to set a value other than <see cref="DataSourceMode.Search"/>. </exception>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [DefaultValue (DataSourceMode.Search)]
  public override DataSourceMode Mode
  {
    get { return DataSourceMode.Search; }
    set { if (value != DataSourceMode.Search) throw new NotSupportedException ("BusinessObjectReferenceSearchDataSourceControl supports only DataSourceMode.Search."); }
  }
}

/// <summary>
///   <b>BusinessObjectReferenceDataSourceControl</b> provides an <see cref="IBusinessObjectReferenceDataSource"/>
///   to controls of type <see cref="IBusinessObjectBoundWebControl"/> inside an <b>ASPX Web Form</b> or 
///   <b>ASCX User Control</b>.
/// </summary>
/// <seealso cref="IBusinessObjectReferenceDataSource"/>
/// <seealso cref="IBusinessObjectDataSourceControl"/>
#if net20
[System.Web.UI.NonVisualControl]
#endif
[Designer (typeof (BocDataSourceDesigner))]
public class BusinessObjectReferenceDataSourceControl: 
    BusinessObjectBoundModifiableWebControl, IBusinessObjectDataSourceControl, IBusinessObjectReferenceDataSource
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

  /// <summary>
  ///   <see cref="BusinessObjectReferenceDataSourceControl"/> supports properties of type
  ///   <see cref="IBusinessObjectReferenceProperty"/>.
  /// </summary>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return new Type[] { typeof (IBusinessObjectReferenceProperty) }; }
  }

  // Default summary will be created.
  public BusinessObjectReferenceDataSourceControl ()
  {
    _internalDataSource = new InternalBusinessObjectReferenceDataSource (this);
  }

  /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
  /// <value> The value must be of type <see cref="IBusinessObject"/>. </value>
  protected override object ValueImplementation
  {
    get { return _internalDataSource.BusinessObject; }
    set { _internalDataSource.BusinessObject = (IBusinessObject) value; }
  }

  /// <summary> Initializes a new instance of the BusinessObjectReferenceDataSourceControl class. </summary>
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
  /// <remarks> 
  ///   Actual saving only occurs if <see cref="BusinessObjectBoundModifiableWebControl.IsReadOnly"/> evaluates 
  ///   <see langword="false"/>. 
  /// </remarks>
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
  /// <remarks>
  ///   Actual saving only occurs if <see cref="BusinessObjectBoundModifiableWebControl.IsReadOnly"/> evaluates 
  ///  <see langword="false"/>. 
  /// </remarks>
  public virtual void SaveValues (bool interim) // inherited data source interface
  {
    if (! IsReadOnly)
      _internalDataSource.SaveValues (interim);
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
  ///   to which this <see cref="BusinessObjectReferenceDataSourceControl"/> connects.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
  ///   <see cref="BusinessObjectReferenceDataSourceControl"/> connects.
  ///  </value>
  /// <remarks> Identical to <see cref="BusinessObjectBoundWebControl.DataSource"/>. </remarks>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectDataSource ReferencedDataSource 
  { 
    get { return _internalDataSource.ReferencedDataSource; }
  }


  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObject"/> accessed through the <see cref="ReferenceProperty"/>.
  /// </summary>
  /// <value> An <see cref="IBusinessObject"/> or <see langword="null"/>. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObject BusinessObject
  {
    get { return _internalDataSource.BusinessObject; }
    set { _internalDataSource.BusinessObject = value; }
  }

  /// <summary> 
  ///   Gets the <see cref="IBusinessObjectReferenceProperty.ReferenceClass"/> of the <see cref="ReferenceProperty"/>.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectClass"/> or <see langword="null"/> if no <see cref="ReferenceProperty"/> is set.
  /// </value>
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
  /// <value> The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>. </value>
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
    get
    {
      // return IsReadOnly ? DataSourceMode.Read : DataSourceMode.Edit; 
      if (IsReadOnly)
        return DataSourceMode.Read;
      if (DataSource != null && DataSource.Mode == DataSourceMode.Search)
        return DataSourceMode.Search;
      return DataSourceMode.Edit; 
    }
    set 
    {
      // "search" needs edit mode
      ReadOnly = (NaBoolean) (value == DataSourceMode.Read); 
    }
  }

  /// <summary>
  ///   Gets an array of <see cref="IBusinessObjectBoundControl"/> objects bound to this 
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <value> An array of <see cref="IBusinessObjectBoundControl"/> objects. </value>
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
