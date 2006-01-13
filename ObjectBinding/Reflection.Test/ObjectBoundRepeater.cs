// Requires:
// System.dll, System.Design.dll, System.Drawing.dll, System.Web.dll
// Rubicon.Core.dll, Rubicon.ObjectBinding.dll, Rubicon.ObjectBinding.Web.dll, Rubicon.Web.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rubicon.Collections;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace OBRTest
{
public class ObjectBoundRepeater: Repeater, IBusinessObjectBoundModifiableWebControl
{
  #region BusinessObjectBinding implementation

  /// <summary>
  ///   Gets the <see cref="BusinessObjectBinding"/> object used to manage the binding for this
  ///   <see cref="ObjectBoundRepeater"/>.
  /// </summary>
  /// <value> The <see cref="BusinessObjectBinding"/> instance used to manage this control's binding. </value>
  [Browsable(false)]
  public BusinessObjectBinding Binding
  {
    get { return _binding; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> this <see cref="IBusinessObjectBoundWebControl"/> 
  ///   is bound to.
  /// </summary>
  /// <value> An <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public new IBusinessObjectDataSource DataSource
  {
    get { return _binding.DataSource; }
    set { _binding.DataSource = value; }
  }

  /// <summary>
  ///   Gets or sets the string representation of the <see cref="Property"/>.
  /// </summary>
  /// <value> 
  ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for
  ///   the <see cref="IBusinessObjectProperty"/>. 
  /// </value>
  [Category ("Data")]
  [Description ("The string representation of the Property.")]
  [Editor (typeof (PropertyPickerEditor), typeof (UITypeEditor))]
  [DefaultValue ("")]
  [MergableProperty (false)]
  public string PropertyIdentifier
  {
    get { return _binding.PropertyIdentifier; }
    set { _binding.PropertyIdentifier = value; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectProperty"/> used for accessing the data to be loaded into 
  ///   <see cref="Value"/>.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectProperty"/> that is part of the bound <see cref="IBusinessObject"/>'s
  ///   <see cref="IBusinessObjectClass"/>
  /// </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectProperty Property
  {
    get { return _binding.Property; }
    set { _binding.Property = value; }
  }

  /// <summary>
  ///   Gets or sets the <b>ID</b> of the <see cref="IBusinessObjectDataSourceControl"/> encapsulating the 
  ///   <see cref="IBusinessObjectDataSource"/> this  <see cref="IBusinessObjectBoundWebControl"/> is bound to.
  /// </summary>
  /// <value> 
  ///   A string set to the <b>ID</b> of an <see cref="IBusinessObjectDataSourceControl"/> inside the current
  ///   naming container.
  /// </value>
  [TypeConverter (typeof (BusinessObjectDataSourceControlConverter))]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [Description ("The ID of the BusinessObjectDataSourceControl control used as data source.")]
  [DefaultValue ("")]
  public string DataSourceControl
  {
    get { return _binding.DataSourceControl; }
    set { _binding.DataSourceControl = value; }
  }
  #endregion

  // static members
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };
  
  private BusinessObjectBinding _binding;
  /// <summary> Set or cleared depending on <see cref="HasValidBinding"/> during <see cref="OnLoad"/>. </summary>
  private bool _hasVisibleBinding = true;
  private IList _value;
  private Boolean _isDirty = true;
  private NaBooleanEnum _required = NaBooleanEnum.Undefined;
  private NaBooleanEnum _readOnly = NaBooleanEnum.Undefined;
  private TypedArrayList _validators;

  private ArrayList _dataSources = new ArrayList();
  private ArrayList _dataEditControls = new ArrayList();

	public ObjectBoundRepeater()
	{
    _binding = new BusinessObjectBinding (this);
	}


  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public override string DataMember
  {
    get { return string.Empty; }
    set { throw new NotSupportedException(); }
  }

  protected override void OnItemDataBound (RepeaterItemEventArgs e)
  {
    base.OnItemDataBound (e);
    if (IsDesignMode)
      return;

    IBusinessObject obj = (IBusinessObject) e.Item.DataItem;
    
    foreach (Control control in e.Item.Controls)
    {
      if (control is IBusinessObjectDataSource)
      {
        IBusinessObjectDataSource dataSource = (IBusinessObjectDataSource) control;
        _dataSources.Add (dataSource);

        dataSource.BusinessObject = obj;
      }
      else if (control is IDataEditControl)
      {
        DataEditUserControl dataEditControl = (DataEditUserControl) control;
        _dataEditControls.Add (dataEditControl);
    
        dataEditControl.BusinessObject = obj;        
      }
    }
  }


  /// <summary> Overrides <see cref="Control.OnInit"/>. </summary>
  /// <remarks>
  ///   Calls <see cref="Control.EnsureChildControls"/> and the <see cref="BusinessObjectBinding.EnsureDataSource"/>
  ///   method.
  /// </remarks>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    EnsureChildControls();
    _binding.EnsureDataSource();
  }

  /// <summary> Overrides <see cref="Control.OnLoad"/>. </summary>
  /// <remarks> Evaluates <see cref="HasValidBinding"/> to determine the control's visibility. </remarks>
  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);
    if (! IsDesignMode)
      _hasVisibleBinding = HasValidBinding;
  }

  /// <summary>
  ///   Gets a flag specifying whether the <see cref="IBusinessObjectBoundControl"/> has a valid binding configuration.
  /// </summary>
  /// <remarks>
  ///   The configuration is considered invalid if data binding is configured for a property 
  ///   that is not available for the bound class or object.
  /// </remarks>
  /// <value> 
  ///   <list type="bullet">
  ///     <item>
  ///       <b>true</b> if the <see cref="DataSource"/> or the <see cref="Property"/> is 
  ///       <b>null</b>. 
  ///     </item>
  ///     <item>
  ///       The result of the 
  ///       <see cref="IBusinessObjectProperty.IsAccessible">IBusinessObjectProperty.IsAccessible</see> method.
  ///     </item>
  ///     <item>Otherwise, <b>false</b> is returned.</item>
  ///   </list>
  /// </value>
  [Browsable (false)]
  public bool HasValidBinding
  {
    get 
    { 
      IBusinessObjectDataSource dataSource = _binding.DataSource;
      IBusinessObjectProperty property = _binding.Property;
      if (dataSource == null || property == null)
        return true;
      return property.IsAccessible (dataSource.BusinessObjectClass, dataSource.BusinessObject);
    } 
  }

  /// <summary> Overrides <see cref="Control.Visible"/>. </summary>
  /// <value> 
  ///   <para>
  ///     The <b>set accessor</b> passes the value to the base class's <b>Visible</b> property.
  ///   </para><para>
  ///     The <b>get accessor</b> ANDs the base class's <b>Visible</b> setting with the value of the 
  ///     <see cref="HasValidBinding"/> property cached during <see cref="OnLoad"/>.
  ///   </para>
  /// </value>
  /// <remarks>
  ///   The control only saves the set value of <b>Visible</b> into the view state. Therefor the control can change
  ///   its visibilty during during subsequent postbacks.
  /// </remarks>
  public override bool Visible
  {
    get { return _hasVisibleBinding && base.Visible; }
    set { base.Visible = value; }
  }

  
  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  public virtual void LoadValue (bool interim)
  {
    if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      ValueImplementation = DataSource.BusinessObject.GetProperty (Property);

    if (! interim)
      _isDirty = false;

    Controls.Clear();
    if (! interim)
      base.ClearChildViewState();
    this.CreateControlHierarchy (true);
    base.ChildControlsCreated = true;

    // iterate over Controls, look for datasources
    foreach (IBusinessObjectDataSource dataSource in _dataSources)
    {
      dataSource.LoadValues (interim);
    }
    foreach (IDataEditControl dataEditControl in _dataEditControls)
    {
      dataEditControl.LoadValues (interim);
    }
  }
  
  /// <summary>
  ///   Saves the <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
  /// </summary>
  /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
  public virtual void SaveValue (bool interim)
  {
    // iterate over Controls, look for datasources
    foreach (IBusinessObjectDataSource dataSource in _dataSources)
    {
      dataSource.SaveValues (interim);
    }
    foreach (IDataEditControl dataEditControl in _dataEditControls)
    {
      dataEditControl.SaveValues (interim);
    }
  }

  /// <summary>
  ///   Gets or sets the dirty flag.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <b>true</b>. The value is 
  ///   set to <b>false</b> during loading
  ///   and saving values. Text changes by the user cause <c>IsDirty</c> to be reset to 
  ///   <b>false</b> during the
  ///   loading phase of the request (i.e., before the page's <c>Load</c> event is raised).
  /// </remarks>
  [Browsable (false)]
  public virtual bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }


  /// <summary> Gets or sets the current value. </summary>
  /// <value> An object implementing <see cref="IList"/>. </value>
  [Browsable (false)]
  public IList Value
  {
    get 
    { 
      return _value; 
    }
    set 
    {
      base.DataSource = value;
      _value = value; 
    }
  }

  /// <summary> Gets or sets the value provided by the <see cref="IBusinessObjectBoundControl"/>. </summary>
  /// <value> An object or boxed value. </value>
  /// <remarks>
  ///   <para>
  ///     Override <see cref="ValueImplementation"/> to define the behaviour of <c>Value</c>. 
  ///   </para><para>
  ///     Redefine <c>Value</c> using the keyword <b>new</b> to provide a typesafe implementation in derived classes.
  ///   </para>
  /// </remarks>
  object IBusinessObjectBoundControl.Value
  {
    get { return ValueImplementation; }
    set { ValueImplementation = value; }
  }

  /// <summary> See <see cref="ObjectBoundRepeater.Value"/> for details on this property. </summary>
  [Browsable (false)]
  protected virtual object ValueImplementation 
  {
    get { return Value; }
    set { Value = (IList) value; }
  }


  /// <summary> Evalutes whether this control is in <b>Design Mode</b>. </summary>
  [Browsable (false)]
  protected bool IsDesignMode
  {
    get { return ControlHelper.IsDesignMode (this, Context); }
  }


  /// <summary> Gets the text to be written into the label for this control. </summary>
  /// <value> <b>null</b> for the default implementation. </value>
  [Browsable(false)]
  public virtual string DisplayName 
  {
    get { return (Property != null) ? Property.DisplayName : null; }
  }

  /// <summary> Specifies the relative URL to the help text for this control. </summary>
  [Browsable(false)]
  public virtual string HelpUrl
  {
    get { return null; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its ClientID.
  /// </summary>
  /// <value> This instance for the default implementation. </value>
  [Browsable(false)]
  public virtual Control TargetControl
  {
    get { return this; }
  }

  /// <summary>
  ///   If <b>UseLabel</b> is <b>true</b>, it is valid to generate HTML &lt;label&gt; tags referencing 
  ///   <see cref="TargetControl"/>.
  /// </summary>
  /// <value>
  ///   <b>false</b>.
  /// </value>
  [Browsable(false)]
  public virtual bool UseLabel
  {
    get { return false; }
  }


  /// <summary> Gets or sets a flag that specifies whether the value of the control is required. </summary>
  /// <remarks>
  ///   Set this property to <see cref="NaBooleanEnum.Undefined"/> in order to use the default value 
  ///   (see <see cref="IsRequired"/>).
  /// </remarks>
  [Description("Explicitly specifies whether the control is required.")]
  [Category ("Data")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public NaBooleanEnum Required
  {
    get { return _required; }
    set { _required = value; }
  }

  /// <summary> Gets or sets a flag that specifies whether the control should be displayed in read-only mode. </summary>
  /// <remarks>
  ///   Set this property to <see cref="NaBooleanEnum.Undefined"/> in order to use the default value 
  ///   (see <see cref="IsReadOnly"/>). Note that if the data source is in read-only mode, the
  ///   control is read-only too, even if this property is set to <c>false</c>.
  /// </remarks>
  [Description("Explicitly specifies whether the control should be displayed in read-only mode.")]
  [Category ("Data")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public NaBooleanEnum ReadOnly
  {
    get { return _readOnly; }
    set { _readOnly = value; }
  }

  /// <summary>
  ///   Gets a flag that determines whether the control is to be displayed in read-only mode.
  /// </summary>
  /// <remarks>
  ///     In read-only mode, a <see cref="System.Web.UI.WebControls.Label"/> control is used to display the value.
  ///     Otherwise, a <see cref="System.Web.UI.WebControls.TextBox"/> control is used to display and edit the value.
  /// </remarks>
  /// <value>
  ///   <list type="bullet">
  ///     <item>
  ///       Whether the control is bound or unbound, if the value of the <see cref="ReadOnly"/> property is 
  ///       <see cref="NaBooleanEnum.True"/>, <b>true</b> is returned.
  ///     </item>
  ///     <item>
  ///       If the control is bound to an <see cref="IBusinessObjectDataSourceControl"/> and 
  ///       <see cref="IBusinessObjectDataSource.Mode">DataSource.Mode</see> is set to 
  ///       <see cref="DataSourceMode.Search"/>, <b>false</b> is returned.
  ///     </item>
  ///     <item>
  ///       If the control is unbound (<see cref="ObjectBoundRepeater.DataSource"/> or 
  ///       <see cref="ObjectBoundRepeater.Property"/> is <b>null</b>) and the
  ///       <see cref="ReadOnly"/> property is not <see cref="NaBooleanEnum.True"/>, 
  ///       <b>false</b> is returned.
  ///     </item>
  ///     <item>
  ///       If the control is bound (<see cref="ObjectBoundRepeater.DataSource"/> and  
  ///       <see cref="ObjectBoundRepeater.Property"/> are not <b>null</b>), 
  ///       the following rules are used to determine the value of this property:
  ///       <list type="bullet">
  ///         <item>
  ///           If the <see cref="IBusinessObjectDataSource.Mode">DataSource.Mode</see> of the control's
  ///           <see cref="ObjectBoundRepeater.DataSource"/> is set to <see cref="DataSourceMode.Read"/>, 
  ///           <b>true</b> is returned.
  ///         </item>
  ///         <item>
  ///           If the <see cref="IBusinessObjectDataSource.BusinessObject">DataSource.BusinessObject</see> is 
  ///           <b>null</b> and the control is not in <b>Design Mode</b>, 
  ///           <b>true</b> is returned.
  ///         </item>
  ///         <item>
  ///           If the control's <see cref="ReadOnly"/> property is <see cref="NaBooleanEnum.False"/>, 
  ///           <b>false</b> is returned.
  ///         </item>
  ///         <item>
  ///           Otherwise, <see langword="Property.IsReadOnly"/> is evaluated and returned.
  ///         </item>
  ///       </list>
  ///     </item>
  ///   </list>
  /// </value>
  [Browsable(false)]
  public virtual bool IsReadOnly
  {
    get
    {
      if (_readOnly == NaBooleanEnum.True) // (Bound Control || Unbound Control) && ReadOnly==true
        return true;
      if (DataSource != null && DataSource.Mode == DataSourceMode.Search) // Search DataSource 
        return false;
      if (Property == null || DataSource == null) // Unbound Control && (ReadOnly==false || ReadOnly==undefined)
        return false;
      if (DataSource.Mode == DataSourceMode.Read) // Bound Control && Reader DataSource
        return true;
      if (! IsDesignMode && DataSource.BusinessObject == null) // Bound Control but no BusinessObject
        return true;
      if (_readOnly == NaBooleanEnum.False) // Bound Control && ReadOnly==false
        return false;
      return Property.IsReadOnly (DataSource.BusinessObject); // ReadOnly==undefined: ObjectModel pulls
    }
  }

  /// <summary>
  ///   Gets a flag that determines whether the control is to be treated as a required value.
  /// </summary>
  /// <remarks>
  ///     The value of this property is used to decide whether <see cref="CreateValidators"/> should 
  ///     include a <see cref="RequiredFieldValidator"/> for this control.
  /// </remarks>
  /// <value>
  ///   The following rules are used to determine the value of this property:
  ///   <list type="bullet">
  ///     <item>If the control is read-only, <b>false</b> is returned.</item>
  ///     <item>
  ///       If the <see cref="Required"/> property is not <see cref="NaBooleanEnum.Undefined"/>, 
  ///       the value of <see cref="Required"/> is returned.
  ///     </item>
  ///     <item>
  ///       If the <see cref="ObjectBoundRepeater.Property"/> contains a property definition with the
  ///       <see cref="IBusinessObjectProperty.IsRequired"/> flag set, <b>true</b> is returned. 
  ///     </item>
  ///     <item>Otherwise, <b>false</b> is returned.</item>
  ///   </list>
  /// </value>
  [Browsable(false)]
  public virtual bool IsRequired 
  {
    get 
    {
      if (IsReadOnly)
        return false;
      if (_required != NaBooleanEnum.Undefined)
        return _required == NaBooleanEnum.True;
      if (Property != null)
        return (bool) Property.IsRequired;
      return false;
    }
  }
 

  /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
  /// <returns> An (empty) array of <see cref="BaseValidator"/> controls. </returns>
  public virtual BaseValidator[] CreateValidators()
  {
    return new BaseValidator[0];
  }

  /// <summary> Registers a validator that references this control. </summary>
  /// <remarks> 
  ///   <para>
  ///     The control may choose to ignore this call. 
  ///   </para><para>
  ///     The registered validators are evaluated when <see cref="Validate"/> is called.
  ///   </para>
  /// </remarks>
  public virtual void RegisterValidator (BaseValidator validator)
  {
    if (_validators == null)
      _validators = new TypedArrayList (typeof (BaseValidator));

    _validators.Add (validator);
  }

  /// <summary> Calls <see cref="BaseValidator.Validate"/> on all registered validators. </summary>
  /// <returns> <b>true</b>, if all validators validated. </returns>
  public virtual bool Validate()
  {
    if (_validators == null)
      return true;

    bool isValid = true;
    for (int i = 0; i < _validators.Count; i++)
    {
      BaseValidator validator = (BaseValidator) _validators[i];
      validator.Validate();
      isValid &= validator.IsValid;
    }
    return isValid;
  }


  /// <summary>
  ///   Tests whether this <see cref="ObjectBoundRepeater"/> can be bound to the <paramref name="property"/>.
  /// </summary>
  /// <param name="property"> 
  ///   The <see cref="IBusinessObjectProperty"/> to be tested. Must not be <b>null</b>.
  /// </param>
  /// <returns>
  ///   <list type="bullet">
  ///     <item>
  ///       <b>true</b> is <see cref="SupportedPropertyInterfaces"/> is null.
  ///     </item>
  ///     <item>
  ///       <b>false</b> if the <see cref="DataSource"/> is in <see cref="DataSourceMode.Search"/> mode.
  ///     </item>
  ///     <item>Otherwise, <see langword="IsPropertyInterfaceSupported"/> is evaluated and returned as result.</item>
  ///   </list>
  /// </returns>
  public virtual bool SupportsProperty (IBusinessObjectProperty property)
  {
    ArgumentUtility.CheckNotNull ("property", property);
    
    if (SupportedPropertyInterfaces == null)
      return true;

    bool isSearchMode = DataSource != null && DataSource.Mode == DataSourceMode.Search;
    if (! isSearchMode && ! SupportsPropertyMultiplicity (property.IsList))
    {
      return false;
    }

    return BusinessObjectBoundWebControl.IsPropertyInterfaceSupported (property, SupportedPropertyInterfaces);
  }

  /// <summary>
  ///   Gets or sets the list of<see cref="Type"/> objects for the <see cref="IBusinessObjectProperty"/> 
  ///   implementations that can be bound to this control.
  /// </summary>
  protected virtual Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary> Gets a value that indicates whether properties with the specified multiplicity are supported. </summary>
  /// <returns> <b>true</b> if the multiplicity specified by <paramref name="isList"/> is supported. </returns>
  protected virtual bool SupportsPropertyMultiplicity (bool isList)
  {
    return isList;
  }

  
  /// <summary> Calls <see cref="Control.OnPreRender"/> on every invocation. </summary>
  /// <remarks> Used by the <see cref="WebControlDesigner"/>. </remarks>
  void IControlWithDesignTimeSupport.PreRenderForDesignMode()
  {
    if (! IsDesignMode)
      throw new InvalidOperationException ("PreRenderChildControlsForDesignMode may only be called during design time.");
    return;
  }

}
}
