using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.Design;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> This control can be used to display or edit enumeration values. </summary>
/// <include file='doc\include\Controls\BocEnumValue.xml' path='BocEnumValue/Class/*' />
[ValidationProperty ("Value")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocEnumValue: BusinessObjectBoundModifiableWebControl //, IPostBackDataHandler
{
	// constants

  private const string c_nullIdentifier = "--null--";
  private const string c_nullDisplayName = "Undefined";
  private const string c_nullItemValidationMessage = "Please select an item.";

  /// <summary> 
  ///   Text displayed when control is displayed in desinger and is read-only has no contents.
  /// </summary>
  private const string c_designModeEmptyLabelContents = "#";

  private const int c_defaultListControlWidthInPoints = 150;

  // types

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectEnumerationProperty) };

  private static readonly object s_eventSelectionChanged = new object();

	// member fields

  /// <summary>
  ///   <see langword="true"/> if <see cref="Value"/> has been changed since last call to
  ///   <see cref="SaveValue"/>.
  /// </summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="ListControl"/> used in edit mode. </summary>
  private ListControl _listControl = null;

  /// <summary> The <see cref="Label"/> used in read-only mode. </summary>
  private Label _label = null;

  /// <summary> The <see cref="BocDateTimeValueValidator"/> returned by <see cref="CreateValidators"/>. </summary>
  private CompareValidator _notNullItemValidator = new CompareValidator();

  /// <summary> The actual enum value. </summary>
  private object _value = null;

  /// <summary> 
  ///   The <see cref="IEnumerationValueInfo.Identifier"/> matching the <see cref="_value"/>.
  /// </summary>
  private string _internalValue = null;

  /// <summary> 
  ///   The <see cref="IEnumerationValueInfo.Identifier"/> set through the 
  ///   <see cref="ListControl"/>. 
  /// </summary>
  private string _newInternalValue = null;

  /// <summary>
  ///   The chached <see cref="IEnumerationValueInfo"/> object matching the <see cref="_value"/>.
  /// </summary>
  private IEnumerationValueInfo _enumerationValueInfo = null;
 
  /// <summary> The <see cref="Style"/> applied to this controls an all sub-controls. </summary>
  private Style _commonStyle = new Style ();

  /// <summary> The <see cref="Style"/> applied to the <see cref="ListControl"/>. </summary>
  private ListControlStyle _listControlStyle = new ListControlStyle ();

  /// <summary> The <see cref="Style"/> applied to the <see cref="_label"/>. </summary>
  private Style _labelStyle = new Style ();

  /// <summary> State field for special behaviour during load view state. </summary>
  /// <remarks> Used by <see cref="InternalLoadValue"/>. </remarks>
  private bool _isLoadViewState;

  // construction and disposing

  /// <summary> Simple constructor. </summary>
	public BocEnumValue()
	{
    //  empty
	}

	// methods and properties

  /// <summary>
  ///   Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls.
  /// </summary>
  /// <remarks>
  ///   If the <see cref="ListControl"/> could not be created from <see cref="ListControlStyle"/>,
  ///   the control is set to read-only.
  /// </remarks>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    _listControl = _listControlStyle.Create (false);
    _label = new Label();

    if (_listControl == null)
    {
      _listControl = new DropDownList();
      ReadOnly = NaBoolean.True;
    }

    _listControl.ID = this.ID + "_Boc_ListControl";
    _listControl.EnableViewState = true;
    Controls.Add (_listControl);

    _label.ID = this.ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);


    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);
    _listControl.SelectedIndexChanged += new EventHandler(ListControl_SelectedIndexChanged);
  }

  /// <summary>
  ///   Calls the parent's <c>OnLoad</c> method and prepares the binding information.
  /// </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    //Binding.EvaluateBinding();

    if (! IsDesignMode)
    {
      string newInternalValue = PageUtility.GetRequestCollectionItem (Page, _listControl.UniqueID); // gets enum identifier

      if (newInternalValue == c_nullIdentifier)
        _newInternalValue = null;
      else if (newInternalValue != null)
        _newInternalValue = newInternalValue;
      else
        _newInternalValue = null;

      if (newInternalValue != null && _newInternalValue != InternalValue)
        _isDirty = true;
    }
  }

  /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
  /// <param name="e"> <see cref="EventArgs.Empty"/>. </param>
  protected virtual void OnSelectionChanged (EventArgs e)
  {
    EventHandler eventHandler = (EventHandler) Events[s_eventSelectionChanged];
    if (eventHandler != null)
      eventHandler (this, e);
  }

  /// <summary>
  ///   Calls the parent's <c>OnPreRender</c> method and ensures that the sub-controls are 
  ///   properly initialized.
  /// </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    //  First call
    EnsureChildControlsInitialized();
  }

  /// <summary>
  ///   Calls the parent's <c>Render</c> method and ensures that the sub-controls are 
  ///   properly initialized.
  /// </summary>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content. 
  /// </param>
  protected override void Render (HtmlTextWriter writer)
  {
    //  Second call has practically no overhead
    //  Required to get optimum designer support.
    EnsureChildControlsInitialized ();

    base.Render (writer);
  }

  /// <summary>
  ///   Calls the parents <c>LoadViewState</c> method and restores this control's specific data.
  /// </summary>
  /// <param name="savedState">
  ///   An <see cref="Object"/> that represents the control state to be restored.
  /// </param>
  protected override void LoadViewState (object savedState)
  {
    _isLoadViewState = true;

    object[] values = (object[]) savedState;

    base.LoadViewState (values[0]);
    Value = values[1];
    if (values[2] != null)
      _internalValue = (string) values[2];
    _isDirty = (bool)  values[3];

    _isLoadViewState = false;
  }

  /// <summary>
  ///   Calls the parents <c>SaveViewState</c> method and saves this control's specific data.
  /// </summary>
  /// <returns>
  ///   Returns the server control's current view state.
  /// </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[4];

    values[0] = base.SaveViewState();
    values[1] = Value;
    values[2] = _internalValue;
    values[3] = _isDirty;

    return values;
  }

  /// <summary>
  ///   Loads the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> or uses the cached
  ///   information if <paramref name="interim"/> is <see langword="false"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="false"/> to load the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      //Binding.EvaluateBinding();
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        ValueImplementation = DataSource.BusinessObject.GetProperty (Property);
        _isDirty = false;
      }
    }
  }

  /// <summary>
  ///   Writes the <see cref="Value"/> into the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> if <paramref name="interim"/> 
  ///   is <see langword="false"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="false"/> to write the <see cref="Value"/> into the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void SaveValue (bool interim)
  {
    if (! interim)
    {
      //Binding.EvaluateBinding();
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
        DataSource.BusinessObject.SetProperty (Property, Value);
    }
  }

  /// <summary>
  ///   Generates the validators depending on the control's configuration.
  /// </summary>
  /// <remarks>
  ///   Generates a validator that checks that the selected item is not the null item if the 
  ///   control is in edit-mode and input is required.
  /// </remarks>
  /// <returns> Returns a list of <see cref="BaseValidator"/> objects. </returns>
  public override BaseValidator[] CreateValidators()
  {
    if (! IsRequired)
      return new BaseValidator[]{};

    BaseValidator[] validators = new BaseValidator[1];
    
    _notNullItemValidator.ID = this.ID + "_ValidatorNotNullItem";
    _notNullItemValidator.ControlToValidate = TargetControl.ID;
    _notNullItemValidator.ValueToCompare = c_nullIdentifier;
    _notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
    _notNullItemValidator.ErrorMessage = c_nullItemValidationMessage;

    validators[0] = _notNullItemValidator;

    //  No validation that only enabled enum values get selected and saved.
    //  This behaviour mimics the Fabasoft enum behaviour

    return validators;
  }

  /// <summary>
  ///   Populates the <see cref="ListControl"/> from the <see cref="Property"/>'s 
  ///   list of enabled enum values before calling <see cref="InternalLoadValue"/>.
  /// </summary>
  protected virtual void RefreshEnumList()
  {
    if (! IsReadOnly)
    {
      if (Property != null)
      {
        _listControl.Items.Clear();

        if (! IsRequired)
          _listControl.Items.Add (CreateNullItem());

        IEnumerationValueInfo[] valueInfos = Property.GetEnabledValues();

        foreach (IEnumerationValueInfo valueInfo in valueInfos)
        {
          ListItem item = new ListItem (valueInfo.DisplayName, valueInfo.Identifier);
          _listControl.Items.Add (item);
        }
      }
    }

    InternalLoadValue();
  }

  /// <summary> Initializes the child controls. </summary>
  protected override void InitializeChildControls()
  {
    bool isReadOnly = IsReadOnly;

    _listControl.Visible = ! isReadOnly;
    _label.Visible = isReadOnly;

    if (isReadOnly)
    {
      _label.Width = Width;
      _label.Height = Height;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);

      if (IsDesignMode && StringUtility.IsNullOrEmpty (_label.Text))
      {
        _label.Text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else if (! IsDesignMode && EnumerationValueInfo != null)
      {
        _label.Text = EnumerationValueInfo.DisplayName;
      }
    }
    else
    {
      //  Prevent a collapsed control
      _listControl.Width = Unit.Point (c_defaultListControlWidthInPoints);

      if (! Width.IsEmpty)
        _listControl.Width = Width;
      _listControl.Height = Height;
      _listControl.ApplyStyle (_commonStyle);
      _listControlStyle.ApplyStyle (_listControl);
    }
  }

  /// <summary> Refreshes the sub-controls for the new value. </summary>
  private void InternalLoadValue()
  {
    InternalLoadValue (null);
  }

  /// <overloads>Overloaded.</overloads>
  /// <summary> Refreshes the sub-controls for the new value. </summary>
  /// <param name="removeItemWithIdentifier">
  ///   if not <see langword="null"/> it removes the item with the matching identifier 
  ///   from the <see cref="ListControl"/>.
  ///   Used for required values once they are set to an item different from the null item,
  ///   or after a disabled value get's deselected.
  /// </param>
  private void InternalLoadValue (string removeItemWithIdentifier)
  {
    bool hasPropertyAfterInitializion = ! _isLoadViewState && Property != null;

    if (! IsReadOnly)
    {
      bool isNullItem =    InternalValue == null
                        || ! hasPropertyAfterInitializion;

      //  Prevent unnecessary removal
      if (removeItemWithIdentifier != null && ! isNullItem)
      {
        ListItem itemToRemove = _listControl.Items.FindByValue (removeItemWithIdentifier);
        _listControl.Items.Remove (itemToRemove);
      }

      //  Check if null item is to be selected
      if (isNullItem)
      {
        //  No null item in the list
        if (_listControl.Items.FindByValue (c_nullIdentifier) == null)
          _listControl.Items.Insert (0, CreateNullItem());

        _listControl.SelectedValue = c_nullIdentifier;
      }
      else
      {
          //  Item currently not in the list
        if (_listControl.Items.FindByValue (InternalValue) == null)
        {
          ListItem item = new ListItem (
            EnumerationValueInfo.DisplayName, 
            EnumerationValueInfo.Identifier);

          _listControl.Items.Add (item);
        }

        _listControl.SelectedValue = InternalValue;
      }
    }
  }

  /// <summary>
  ///   Raises this control's <see cref="SelectionChanged"/> event if the value was changed 
  ///   through the <see cref="ListControl"/>.
  /// </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void ListControl_SelectedIndexChanged (object sender, EventArgs e)
  {
    if (_newInternalValue != InternalValue)
    {
      InternalValue = _newInternalValue;

      OnSelectionChanged (EventArgs.Empty);
    }
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    RefreshEnumList();
  }

  /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
  /// <returns> A <see cref="ListItem"/>. </returns>
  private ListItem CreateNullItem()
  {
    string nullDisplayName = (_listControl is DropDownList) ? string.Empty : c_nullDisplayName;

    ListItem emptyItem = new ListItem (nullDisplayName, c_nullIdentifier);
    return emptyItem;
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectEnumerationProperty"/> object this control is bound to.
  /// </summary>
  /// <value>An <see cref="IBusinessObjectEnumerationProperty"/> object.</value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectEnumerationProperty Property
  {
    get { return (IBusinessObjectEnumerationProperty) base.Property; }
    set 
    {
      ArgumentUtility.CheckType ("value", value, typeof (IBusinessObjectEnumerationProperty));

      base.Property = (IBusinessObjectEnumerationProperty) value; 
    }
  }
  
  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Used for communicating with the outside world.
  ///   </para><para>
  ///     Must be serializable.
  ///   </para><para>
  ///      Must be part of the enum identified by 
  ///      <see cref="IBusinessObjectProperty.PropertyType"/>.
  ///   </para><para>
  ///     Relies on the implementation of
  ///     <see cref="IBusinessObjectEnumerationProperty.GetValueInfoByValue"/>
  ///     for verification of the passed enum value.
  ///   </para><para>
  ///     If  <see cref="Property"/> is <see langword="null"/>, no type checking can be performed.
  ///   </para>
  /// </remarks>
  /// <value> 
  ///   The enumeration value currently displayed 
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  [Browsable(false)]
  public new object Value
  {
    get { return _value; }
    set
    {
      _value = value;

      if (Property != null && _value != null)
        _enumerationValueInfo = Property.GetValueInfoByValue (_value);
      else
        _enumerationValueInfo = null;

      if (_enumerationValueInfo != null)
        InternalValue = _enumerationValueInfo.Identifier;
      else
        InternalValue = null;
    }
  }

  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }

  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  /// <remarks> Only used to simplify access to the <see cref="IEnumerationValueInfo"/>. </remarks>
  /// <value> 
  ///   The <see cref="EnumerationValueInfo"/> object
  ///   or <see langword="null"/> if no item / the null item is selected 
  ///   or <see cref="Property"/> is <see langword="null"/>.
  /// </value>
  protected IEnumerationValueInfo EnumerationValueInfo
  {
    get 
    {
      if (_enumerationValueInfo == null && Property != null && _value != null)
        _enumerationValueInfo = Property.GetValueInfoByValue (_value);

      return _enumerationValueInfo; 
    }
  }

  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  /// <remarks> 
  ///   <para>
  ///     Used to identifiy the currently selected item.
  ///   </para><para>
  ///     If <see cref="Property"/> is not set when the selection changed event fires,
  ///     <see cref="Value"/> will not be properly set to the new value.
  ///   </para>
  /// </remarks>
  /// <value> 
  ///   The <see cref="IEnumerationValueInfo.Identifier"/> object
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  protected virtual string InternalValue
  {
    get 
    {
      if (_internalValue == null && EnumerationValueInfo != null)
        _internalValue = EnumerationValueInfo.Identifier;

      return _internalValue; 
    }
    set 
    {
      if (_internalValue == value)
        return;

      string oldInternalValue = _internalValue;

      _internalValue = value;
      
        //  Still chached in _enumerationValueInfo
      if (   _enumerationValueInfo != null 
          && _enumerationValueInfo.Identifier == _internalValue)
      {
        _value = _enumerationValueInfo.Value;
      }
      //  Can get a new EnumerationValueInfo
      else if (_internalValue != null && Property != null)
      {
        _enumerationValueInfo = Property.GetValueInfoByIdentifier (_internalValue);
        _value = _enumerationValueInfo.Value;
      }
      else
      {
        _value = null;
        _enumerationValueInfo = null;
      }

      string removeItemWithIdentifier = null;

      if (oldInternalValue == null && IsRequired)
      {
        removeItemWithIdentifier = c_nullIdentifier;
      }
      else if (oldInternalValue != null && Property != null)
      {
        IEnumerationValueInfo oldEnumerationValueInfo =
            Property.GetValueInfoByIdentifier (oldInternalValue);
        
        if (oldEnumerationValueInfo != null && ! oldEnumerationValueInfo.IsEnabled)
          removeItemWithIdentifier = oldInternalValue;
      }
      
      InternalLoadValue (removeItemWithIdentifier);
    }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; 
  ///   using it's ClientID.
  /// </summary>
  public override Control TargetControl 
  {
    get { return (_listControl != null) ? _listControl : (Control) this; }
  }

  /// <summary>
  ///   Specifies whether the selected value within the control has been changed 
  ///   since the last load/save operation.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <c>true</c>. The value is set to <c>false</c> 
  ///   during loading and saving values. Text changes by the user cause <c>IsDirty</c> to be 
  ///   reset to <c>false</c> during the loading phase of the request (i.e., before the page's 
  ///   <c>Load</c> event is raised).
  /// </remarks>
  public override bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  /// <summary>
  ///   The list of<see cref="Type"/> objects for the <see cref="IBusinessObjectProperty"/> 
  ///   implementations that can be bound to this control.
  /// </summary>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary> Overrides <see cref="Rubicon.Web.UI.ISmartControl.UseLabel"/>. </summary>
  public override bool UseLabel
  {
    get 
    {
      bool isDropDownList = _listControlStyle.ControlType == ListControlType.DropDownList;
      bool isListBox = _listControlStyle.ControlType == ListControlType.ListBox;
    
      return ! (isDropDownList || isListBox);
    }
  }

  /// <summary> This event is fired when the selection is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the selection changes.")]
  public event EventHandler SelectionChanged
  {
    add { Events.AddHandler (s_eventSelectionChanged, value); }
    remove { Events.RemoveHandler (s_eventSelectionChanged, value); }
  }

  /// <summary>
  ///   The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <c>Font</c> 
  ///   attributes (Bold, Italic etc.) to <c>true</c>, this cannot be overridden using 
  ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/>  properties.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style CommonStyle
  {
    get { return _commonStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the TextBox (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public ListControlStyle ListControlStyle
  {
    get { return _listControlStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the Label (read-only mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the Label (read-only mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style LabelStyle
  {
    get { return _labelStyle; }
  }

  /// <summary> Gets the <see cref="ListControl"/> used in edit mode. </summary>
  [Browsable (false)]
  public ListControl ListControl
  {
    get { return _listControl; }
  }

  /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary>
  ///   Validation message if the null item is selected but a valid selection is required.
  /// </summary>
  /// <remarks> 
  ///   Use this property to automatically assign a validation message by 
  ///   <see cref="Rubicon.Web.UI.Globalization.ResourceDispatcher"/>. 
  /// </remarks>
  [Description("Validation message if the null item is selected but a valid selection is required.")]
  [Category ("Validator")]
  [DefaultValue("")]
  public string NullItemErrorMessage
  {
    get { return _notNullItemValidator.ErrorMessage; }
    set { _notNullItemValidator.ErrorMessage = value; }
  }
}

}
